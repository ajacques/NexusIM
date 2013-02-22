using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using InstantMessage.Events;
using InstantMessage.Protocols.XMPP.Messages;
using InstantMessage.Protocols.XMPP.Messages.Jingle;
using InstantMessage.Protocols.XMPP.Messages.Message;

namespace InstantMessage.Protocols.XMPP
{
	[IMNetwork("xmpp")]
	public class XmppProtocol : IMProtocol, IAudioVideoCapableProtocol
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

			iqProcessors = new SortedDictionary<string, ProcessIqMessage>();
			iqProcessors.Add("jabber:iq:version:query", HandleGetClientVersion);
			iqProcessors.Add("urn:xmpp:time:time", HandleGetClientTime);
			iqProcessors.Add(XmppNamespaces.DiscoInfo + ":query", HandleDiscoInfo);
			iqProcessors.Add(XmppNamespaces.Jingle + ":jingle", HandleJingleInvite);

			discoFeatures = new SortedSet<string>();
			//discoFeatures.Add("urn:xmp:jingle:apps:rtp:rtp-hdrext:0");
			discoFeatures.Add("urn:xmpp:jingle:1");
			//discoFeatures.Add("urn:xmpp:jingle:transports:raw-udp:1");
			discoFeatures.Add("urn:xmpp:jingle:transports:ice-udp:1");
			discoFeatures.Add("urn:xmpp:jingle:apps:rtp:1");
			discoFeatures.Add("urn:xmpp:jingle:apps:rtp:audio");
			discoFeatures.Add("urn:xmpp:jingle:apps:rtp:video");
			discoFeatures.Add("urn:ietf:rfc:3264");
			//discoFeatures.Add("http://jabber.org/protocol/jinglenodes");
			//discoFeatures.Add("urn:xmpp:jingle:apps:rtp:zrtp:1");
			discoFeatures.Add("http://jabber.org/protocol/disco#info");
			discoFeatures.Add("urn:xmpp:jingle:transfer:0");


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
		public void SendMessage(XmppContact contact, string message)
		{
			ChatMessageMessage msg = new ChatMessageMessage(contact.Jid, message);
			msg.Id = correlator.GetNextId();

			WriteMessage(msg);
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
					IqMessage iq = (IqMessage)message;
					switch (iq.Type)
					{
						case IqMessage.IqType.set:
						case IqMessage.IqType.get:
							HandleUnsolicitedIqMessage(iq);
							break;
						case IqMessage.IqType.result:
							correlator.TryHandleResponse(message as IqMessage);
							break;
					}
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
			msg.CapsHash = GenerateCapsHash();

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
		private void SendPing(Jid target)
		{
			WriteMessage(new PingMessage(target), ProcessPingResponse, DateTime.Now);
		}
		private string GenerateCapsHash()
		{
			StringBuilder sb = new StringBuilder();

			foreach (var feature in discoFeatures)
			{
				sb.Append(feature);
				sb.Append('<');
			}

			byte[] hash;
			using (SHA1 sha = new SHA1Managed())
			{
				hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
			}

			return Convert.ToBase64String(hash);
		}

		// IQ Response Handlers
		private void HandleRosterResponse(IqMessage message, object userState)
		{
			GetRosterMessage.RosterListResponse resp = (GetRosterMessage.RosterListResponse)message;

			foreach (RosterItem roster in resp.RosterItems)
			{
				XmppContact buddy = new XmppContact(this, roster.Jid);
				buddy.Nickname = roster.Name;

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

			mConnected = true;
			ProtocolStatus = IMProtocolStatus.Online;
			OnLogin();
		}
		private void ProcessPingResponse(IqMessage message, object userState)
		{
			DateTime startTime = (DateTime)userState;

			if (message.Source == null)
			{
				lastMeasuredRTT = DateTime.Now - startTime;

				Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "Ping Latency (RTT to server): {0:N1}ms", lastMeasuredRTT.TotalMilliseconds));
			} else { // User targeted
				TimeSpan rtt = DateTime.Now - startTime;

				Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "Ping Latency (RTT to {0}): {1:N1}ms", message.Source, lastMeasuredRTT.TotalMilliseconds));
			}
		}
		private void HandleUnsolicitedIqMessage(IqMessage message)
		{
			string key = String.Format("{0}:{1}", message.Namespace, message.LocalName);

			ProcessIqMessage processor;
			if (iqProcessors.TryGetValue(key, out processor))
			{
				processor(message);
			}
		}
		private void HandleGetClientVersion(IqMessage message)
		{
			XmlDocument xml = new XmlDocument();
			XmlElement root = xml.CreateElement("query", "jabber:iq:version");
			xml.AppendChild(root);
			XmlElement name = xml.CreateElement("name", "jabber:iq:version");
			root.AppendChild(name);
			XmlElement version = xml.CreateElement("version", "jabber:iq:version");
			root.AppendChild(version);
			XmlElement os = xml.CreateElement("os", "jabber:iq:version");
			root.AppendChild(os);

			name.AppendChild(xml.CreateTextNode("NexusIM"));
			version.AppendChild(xml.CreateTextNode("Unknown"));
			os.AppendChild(xml.CreateTextNode(Environment.OSVersion.ToString()));

			IqFragmentMessage frag = IqFragmentMessage.CreateInReplyTo(xml, message);

			WriteMessage(frag);
		}
		private void HandleGetClientTime(IqMessage message)
		{
			ReplyWith(message, new ClientTimeMessage.Response(DateTime.Now));
		}
		private void HandleDiscoInfo(IqMessage message)
		{
			EntityDiscoveryMessage.Request msg = (EntityDiscoveryMessage.Request)message;
			EntityDiscoveryMessage.Response response = msg.Respond();

			foreach (var feature in discoFeatures)
			{
				response.AddFeature(feature);
			}
			
			ReplyWith(msg, response);
		}
		private void HandleJingleInvite(IqMessage message)
		{
			WriteMessage(message.Ack()); // Acknowledge that we received the request

			if (IncomingCall != null)
			{
				JingleInviteMessage.AttemptMessage msg = (JingleInviteMessage.AttemptMessage)message;
				XmppContact contact = FindContact(message.Source);
				
				XmppIncomingCallEventArgs args = new XmppIncomingCallEventArgs(contact);
				args.Id = msg.SessionId;

				IJingleDescriptionType rtp_type;
				if (msg.DescriptionNodes.TryGetValue("audio", out rtp_type))
				{
					JingleRtpDescription rtp = (JingleRtpDescription)rtp_type;

					foreach (var payloadType in rtp.PayloadTypes)
					{
						args.IncomingAudioPayloadTypes.Add(payloadType);
					}
				}

				JingleTransportDescription transport = (JingleTransportDescription)msg.TransportNodes.First();

				foreach (var candidate in transport.Candidates)
				{
					args.IncomingTransportCandidates.Add(candidate);
				}

				IncomingCall(this, args);

				if (args.Accepted)
				{
					JingleInviteMessage.AcceptMessage acceptmsg = new JingleInviteMessage.AcceptMessage();
					acceptmsg.SessionId = msg.SessionId;
					JingleRtpDescription rtpDescipt = new JingleRtpDescription();
					rtpDescipt.MediaType = "audio";

					foreach (var payload in args.AcceptedPayloadTypes)
					{
						rtpDescipt.PayloadTypes.Add((JingleRtpDescription.JinglePayloadType)payload);
					}

					acceptmsg.DescriptionNodes.Add("audio", rtpDescipt);

					JingleTransportDescription ourtransport = new JingleTransportDescription();

					foreach (var trans in args.OurTransportCandidates)
					{
						ourtransport.Candidates.Add((XmppSdpCandidate)trans);
					}

					acceptmsg.TransportNodes.Add(ourtransport);

					ReplyWith(msg, acceptmsg);
				}
			}
		}

		internal void WriteMessage(XmppMessage message)
		{
			xmppStream.WriteMessage(message);
		}
		internal void WriteMessage(IqMessage message, HandleResponse respHandler, object userState)
		{
			message.Source = new Jid(Username, Server, Resource);

			if (message.Type == IqMessage.IqType.get || message.Type == IqMessage.IqType.set)
			{
				message.Id = correlator.GetNextId();
				correlator.CreateRequest(message, respHandler, userState);
			}

			WriteMessage(message as XmppMessage);
		}
		internal void WriteMessage(IqMessage message)
		{
			WriteMessage(message, IdentityHandler, null);
		}
		internal void ReplyWith(IqMessage original, IqMessage reply)
		{
			reply.Id = original.Id;
			reply.To = original.Source;

			WriteMessage(reply);
		}
		internal void ReplyWith(IqMessage original, IqMessage reply, HandleResponse response, object userState)
		{
			reply.Id = original.Id;
			reply.To = original.Source;

			WriteMessage(reply, response, userState);
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
		private delegate void ProcessIqMessage(IqMessage message);

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

		// Events
		public event EventHandler<IncomingCallEventArgs> IncomingCall;

		// Variables
		// Profiling
		private Stopwatch connectTimer;
		private TimeSpan lastMeasuredRTT;
		// Misc.
		private IHostnameResolver hostResolver;
		private bool enableTls = true;
		private IDictionary<Type, ProcessMessage> messageProcessors;
		private IDictionary<string, ProcessIqMessage> iqProcessors;
		private bool mRunMsgThread;
		private Thread mBGMessageThread;
		private System.Timers.Timer pingTimer;
		private Socket networkSocket;
		private XmppStream xmppStream;
		private MessageCorrelator correlator;
		private IAuthStrategy authStrategy;
		private SortedSet<string> discoFeatures;

		// Consts
		private static HandleResponse IdentityHandler;
	}
}