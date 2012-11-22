using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using InstantMessage.Events;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	[IMNetwork("xmpp")]
	public class XmppProtocol : IMProtocol
	{
		// Constructors
		public XmppProtocol()
		{
			mBGMessageThread = new Thread(new ThreadStart(MessageLoop));
			mBGMessageThread.Name = "XmppProtocol - Message Loop";
			correlator = new MessageCorrelator();

			messageProcessors = new SortedDictionary<Type, ProcessMessage>(new TypeComparer());
			messageProcessors.Add(typeof(StreamInitMessage), ProcessStreamInitMessage);
			messageProcessors.Add(typeof(StartTlsMessage.ProceedMessage), ProcessTlsProceedMessage);
			messageProcessors.Add(typeof(SaslAuthMessage.SuccessMessage), ProcessSaslSuccessMessage);
			messageProcessors.Add(typeof(SaslAuthMessage.FailureMessage), ProcessSaslFailureMessage);
			messageProcessors.Add(typeof(SaslChallengeMessage), ProcessSaslChallengeMessage);

			protocolType = "XMPP";
			mProtocolTypeShort = "xmpp";
			Resource = "Test";
			connectTimer = new Stopwatch();
		}
		static XmppProtocol()
		{
			IdentityHandler = (response, userState) => { };
		}

		// Public Methods
		public override void BeginLogin()
		{
			base.BeginLogin();

			networkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			networkSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			networkSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 0);

			IPEndPoint ep = hostResolver.Resolve(this);

			Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "XMPP: Beginning connection for account {0} to endpoint {1} (Resolved by: {2})", Username, ep, hostResolver.GetType().FullName));

			SocketPermission permission = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, ep.Address.ToString(), ep.Port);
			permission.Demand();

			connectTimer.Start();
			networkSocket.BeginConnect(ep, new AsyncCallback(OnSocketConnect), null);
		}

		// Private Methods
		private void OnSocketConnect(IAsyncResult a)
		{
			try {
				networkSocket.EndConnect(a);
			} catch (SocketException e) {
				base.triggerOnError(new SocketErrorEventArgs(e));
				return;
			} finally {
				Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "XMPP: TCP Handshake completed in {0:N0}ms", connectTimer.Elapsed.TotalMilliseconds));
			}
			xmppStream = new XmppStream(new NetworkStream(networkSocket), this);

			mRunMsgThread = true;
			mBGMessageThread.Start();
			WriteMessage(new StreamInitMessage(Server));
		}
		private void MessageLoop()
		{
			xmppStream.InitReader();

			while (mRunMsgThread)
			{
				XmppMessage message = xmppStream.ReadMessage();

				if (message == null)
					continue;

				ProcessMessage proc;
				if (messageProcessors.TryGetValue(message.GetType(), out proc))
				{
					proc(message);
				} else if (message is IqMessage) {
					correlator.TryHandleResponse(message as IqMessage);
				}
			}
		}
		internal void WriteMessage(XmppMessage message)
		{
			xmppStream.WriteMessage(message);
		}
		internal void WriteMessage(IqMessage message, HandleResponse respHandler, object userState)
		{
			message.Id = correlator.GetNextId();
			message.Source = new Jid(Username, Server);

			correlator.CreateRequest(message, respHandler, userState);

			WriteMessage(message as XmppMessage);
		}
		internal void WriteMessage(IqMessage message)
		{
			WriteMessage(message, IdentityHandler, null);
		}

		internal bool TriggerTlsVerifyEvent(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			CertErrorEventArgs args = new CertErrorEventArgs(certificate, chain, sslPolicyErrors);

			triggerOnError(args);

			return args.Continue; // Ignore all Ssl errors
		}

		// Message Processors
		private void ProcessStreamInitMessage(XmppMessage message)
		{
			StreamInitMessage msg = (StreamInitMessage)message;
			if (enableTls && msg.Features.Any(f => f is StreamInitMessage.TlsCapableFeature) && !xmppStream.IsEncrypted)
			{
				WriteMessage(new StartTlsMessage());
			} else if (msg.Features.Any(f => f is StreamInitMessage.AuthMechanismFeature)) {
				StreamInitMessage.AuthMechanismFeature authmech = msg.Features.First(s => s is StreamInitMessage.AuthMechanismFeature) as StreamInitMessage.AuthMechanismFeature;

				if (authmech.Mechanisms.Contains("SCRAM-SHA-1"))
				{
					authStrategy = new ScramAuthStrategy();
				} else if (authmech.Mechanisms.Contains("PLAIN")) {
					authStrategy = new PlainAuthStrategy();
				}

				authStrategy.StartAuthentication(this);
			} else if (msg.Features.Any(f => f is StreamInitMessage.BindFeature)) {
				StreamInitMessage.BindFeature bindfeature = msg.Features.First(s => s is StreamInitMessage.BindFeature) as StreamInitMessage.BindFeature;

				WriteMessage(new BindToResourceMessage(Resource));
			}
		}
		private void ProcessTlsProceedMessage(XmppMessage message)
		{
			xmppStream.ActivateTLS(Server);
			WriteMessage(new StreamInitMessage(Server));
		}
		private void ProcessSaslChallengeMessage(XmppMessage message)
		{
			SaslChallengeMessage msg = (SaslChallengeMessage)message;

			authStrategy.PerformStep(msg);
		}
		private void ProcessSaslSuccessMessage(XmppMessage message)
		{
			if (authStrategy != null)
			{
				authStrategy.Finalize(message as SaslAuthMessage.SuccessMessage);
				authStrategy = null;
			}
			xmppStream.InitReader();
			xmppStream.ResetWriterState();
			WriteMessage(new StreamInitMessage(Server));

			connectTimer.Stop();
			Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "XMPP: Connect to Authenticated took: {0:N0}ms", connectTimer.Elapsed.TotalMilliseconds));

			base.OnLogin();
		}
		private void ProcessSaslFailureMessage(XmppMessage message)
		{
			triggerBadCredentialsError();
			mRunMsgThread = false;
		}

		// Nested Classes
		private sealed class TypeComparer : IComparer<Type>
		{
			public int Compare(Type x, Type y)
			{
				return x.GetHashCode().CompareTo(y.GetHashCode());
			}
		}
		
		// Delegates
		private delegate void ProcessMessage(XmppMessage message);

		// Properties
		public string Resource
		{
			get;
			set;
		}
		public IHostnameResolver HostnameResolver
		{
			get {
				return hostResolver;
			}
			set {
				hostResolver = value;
			}
		}

		// Variables
		// Profiling
		private Stopwatch connectTimer;
		// Misc.
		private IHostnameResolver hostResolver;
		private bool enableTls = true;
		private IDictionary<Type, ProcessMessage> messageProcessors;
		private bool mRunMsgThread;
		private Thread mBGMessageThread;
		private Socket networkSocket;
		private XmppStream xmppStream;
		private MessageCorrelator correlator;
		private IAuthStrategy authStrategy;

		// Consts
		private static HandleResponse IdentityHandler;
	}
}