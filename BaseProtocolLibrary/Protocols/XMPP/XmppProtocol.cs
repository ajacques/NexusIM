using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using InstantMessage.Events;

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

			Stopwatch sw = new Stopwatch();
			sw.Start();
			networkSocket.BeginConnect(ep, new AsyncCallback(OnSocketConnect), sw);
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
				Stopwatch sw = (Stopwatch)a.AsyncState;
				sw.Stop();
				Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "XMPP: TCP Handshake completed in {0}", sw.Elapsed));
			}
			xmppStream = new XmppStream(new NetworkStream(networkSocket));

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
		internal void WriteMessage(IqMessage message)
		{
			message.Id = correlator.GetNextId();
			message.Source = new Jid(Username, Server);
			WriteMessage(message as XmppMessage);
		}

		// Message Processors
		private void ProcessStreamInitMessage(XmppMessage message)
		{
			StreamInitMessage msg = message as StreamInitMessage;
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
			}
		}
		private void ProcessTlsProceedMessage(XmppMessage message)
		{
			xmppStream.ActivateTLS(Server);
			WriteMessage(new StreamInitMessage(Server));
		}
		private void ProcessSaslChallengeMessage(XmppMessage message)
		{
			SaslChallengeMessage msg = message as SaslChallengeMessage;

			authStrategy.PerformStep(msg);
		}
		private void ProcessSaslSuccessMessage(XmppMessage message)
		{
			authStrategy = null;
			xmppStream.InitReader();
			xmppStream.ResetWriterState();
			WriteMessage(new StreamInitMessage(Server));

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
		private IHostnameResolver hostResolver;
		private bool enableTls = false;
		private IDictionary<Type, ProcessMessage> messageProcessors;
		private bool mRunMsgThread;
		private Thread mBGMessageThread;
		private Socket networkSocket;
		private XmppStream xmppStream;
		private MessageCorrelator correlator;
		private IAuthStrategy authStrategy;
	}
}