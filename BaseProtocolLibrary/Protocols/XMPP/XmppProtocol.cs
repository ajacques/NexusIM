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
using System.Xml;
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
			messageProcessors.Add(typeof(PresenceMessage), ProcessPresenceMessage);

			protocolType = "XMPP";
			mProtocolTypeShort = "xmpp";
			Resource = "Test";
			connectTimer = new Stopwatch();
			pingTimer = new System.Timers.Timer(30000);
			pingTimer.Elapsed += DoSendPingTick;
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
		private void DoSendPingTick(object sender, EventArgs args)
		{
			SendPing();
		}

		private void SendPresenceUpdate(int priority)
		{
			PresenceMessage msg = new PresenceMessage();
			msg.Priority = priority;

			WriteMessage(msg);
		}
		private void RequestRosterUpdate()
		{
			WriteMessage(new GetRosterMessage(), HandleRosterResponse, null);
		}
		private XmppContact FindContact(Jid jid)
		{
			foreach (var contact in buddylist)
			{
				XmppContact c = (XmppContact)contact.Value;
				if (c.Jid.Equals(jid, true))
				{
					return c;
				}
			}

			return null;
		}
		private void SendPing()
		{
			WriteMessage(new PingMessage(), ProcessPingResponse, DateTime.Now);
		}

		// IQ Response Handlers
		private void HandleRosterResponse(IqMessage message, object userState)
		{
			GetRosterMessage.RosterListResponse resp = (GetRosterMessage.RosterListResponse)message;

			foreach (RosterItem roster in resp.RosterItems)
			{
				XmppContact buddy = new XmppContact(this, roster.Jid);

				buddylist.Add(buddy);
			}

			SendPresenceUpdate(5);
		}
		private void ProcessVcardResponse(IqMessage message, object userState)
		{
			IqFragmentMessage fragment = (IqFragmentMessage)message;

			IDictionary<string, string> vcardParts = new Dictionary<string, string>();

			foreach (XmlElement element in fragment.Document.DocumentElement)
			{
				vcardParts.Add(element.Name, element.InnerText);
			}
		}
		private void ProcessResourceBindResponce(IqMessage message, object userState)
		{
			BindToResourceMessage.ResponseMessage resp = (BindToResourceMessage.ResponseMessage)message;

			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement("session", XmppNamespaces.Sessions));

			WriteMessage(IqFragmentMessage.CreateNew(doc));
			RequestRosterUpdate();
		}
		private void ProcessPingResponse(IqMessage message, object userState)
		{
			DateTime startTime = (DateTime)userState;
			lastMeasuredRTT = DateTime.Now - startTime;

			Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "Ping Latency (RTT to server): {0:N1}ms", lastMeasuredRTT.TotalMilliseconds));
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

				WriteMessage(new BindToResourceMessage(Resource), ProcessResourceBindResponce, null);
				WriteMessage(new FetchVCardMessage(), ProcessVcardResponse, null);
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
			pingTimer.Start();

			connectTimer.Stop();
			Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "XMPP: Connect to Authenticated took: {0:N0}ms", connectTimer.Elapsed.TotalMilliseconds));

			base.OnLogin();
		}
		private void ProcessSaslFailureMessage(XmppMessage message)
		{
			triggerBadCredentialsError();
			mRunMsgThread = false;
		}
		private void ProcessPresenceMessage(XmppMessage message)
		{
			PresenceMessage msg = (PresenceMessage)message;

			XmppContact contact = FindContact(msg.From);

			if (contact != null)
			{
				contact.Status = IMBuddyStatus.Available;
			}
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
		private TimeSpan lastMeasuredRTT;
		// Misc.
		private IHostnameResolver hostResolver;
		private bool enableTls = false;
		private IDictionary<Type, ProcessMessage> messageProcessors;
		private bool mRunMsgThread;
		private Thread mBGMessageThread;
		private System.Timers.Timer pingTimer;
		private Socket networkSocket;
		private XmppStream xmppStream;
		private MessageCorrelator correlator;
		private IAuthStrategy authStrategy;

		// Consts
		private static HandleResponse IdentityHandler;
	}
}