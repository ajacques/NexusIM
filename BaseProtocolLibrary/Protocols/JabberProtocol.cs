using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Security;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using jabber;
using jabber.client;
using jabber.connection;
using jabber.protocol;
using jabber.protocol.client;
using InstantMessage.Events;

namespace InstantMessage
{
	public class IMJabberProtocol : IMProtocol
	{
		public IMJabberProtocol() 
		{
			protocolType = "Jabber";
			mProtocolTypeShort = "jabber";
			mResource = "test";
			supportsBuzz = true;
			mLoginWaitHandle = new ManualResetEvent(false);
		}
		public override void BeginLogin()
		{
			mLoginWaitHandle.Reset();

			if (jc == null)
			{
				jc = new JabberClient();
				roster = new RosterManager();
				disco = new DiscoManager();
				caps = new CapsManager();
				presence = new PresenceManager();
				pubsub = new PubSubManager();
				muc = new ConferenceManager();

				jc.OnMessage += new MessageHandler(onMessage);
				jc.OnAuthenticate += new bedrock.ObjectHandler(onAuthenticate);
				jc.OnError += new bedrock.ExceptionHandler(onClientError);
				jc.OnAuthError += new ProtocolHandler(onAuthError);
				jc.OnInvalidCertificate += new RemoteCertificateValidationCallback(onInvalidCertificate);
				jc.OnReadText += new bedrock.TextHandler(onReadText);
				jc.OnIQ += new IQHandler(this.onIQ);
				jc.OnPresence += new PresenceHandler(onPresence);
				jc.AutoStartCompression = true;
				jc.AutoStartTLS = true;
				jc.AutoPresence = true;

				roster.Stream = jc;
				roster.OnRosterEnd += new bedrock.ObjectHandler(onRosterEnd);
				roster.OnRosterItem += new RosterItemHandler(onRosterItem);

				caps.Stream = jc;
				//caps.DiscoManager = disco;
				caps.FileName = "capscache.xml";
				caps.Node = "http://www.adrensoftware.com/tools/nexusim.php";
				caps.AddFeature(URI.DISCO_INFO);
				caps.AddFeature(URI.DISCO_ITEMS);
				caps.AddFeature("urn:xmpp:attention:0"); // Buzz!
				caps.AddFeature("urn:xmpp:time"); // Entity Time
				caps.AddFeature("urn:xmpp:ip");
				caps.AddFeature("urn:xmpp:locationquery");
				caps.AddFeature("http://jabber.org/protocol/chatstates");

				disco.Stream = jc;

				presence.Stream = jc;
				presence.CapsManager = caps;

				caps.DiscoManager = disco;

				pubsub.Stream = jc;

				muc.Stream = jc;
			}

			string[] split = Username.Split(new char[] { '@' });

			jc.User = split[0]; //"penguin";
			jc.Server = split[1]; //"wippien.com";
			jc.Password = Password; //"RlfWWczb";
			jc.Resource = mResource;

			jc.Connect();
			jc.Login();
			mEnabled = true;
			status = IMProtocolStatus.CONNECTING;
		}
		public override void Disconnect()
		{
			jc.Close();
			CleanupBuddyList();
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (!mEnabled)
				return; // If it's not even enabled

			if (newstatus == mStatus)
				return;

			if (newstatus == IMStatus.OFFLINE)
			{
				Disconnect();
			} else if (newstatus == IMStatus.AVAILABLE && !jc.IsAuthenticated) {
				BeginLogin();
			} else if (newstatus == IMStatus.AVAILABLE) {
				XmlDocument doc = new XmlDocument();
				XmlElement presence = doc.CreateElement("presence");
				XmlElement xStatus = doc.CreateElement("newstatus");
				presence.SetAttribute("from", mUsername);
				xStatus.InnerText = "";
				presence.AppendChild(xStatus);
				jc.Write(presence);
			} else if (newstatus == IMStatus.IDLE && status == IMProtocolStatus.ONLINE) {
				XmlDocument doc = new XmlDocument();
				XmlElement presence = doc.CreateElement("presence");
				XmlElement xStatus = doc.CreateElement("newstatus");
				XmlElement xShow = doc.CreateElement("show");
				presence.SetAttribute("from", mUsername);
				xStatus.InnerText = "Idle since " + DateTime.Now.ToString("t");
				presence.AppendChild(xStatus);
				xShow.InnerText = "away";
				presence.AppendChild(xShow);

				jc.Write(presence);
			} else if (newstatus == IMStatus.AWAY)	{
				XmlDocument doc = new XmlDocument();
				XmlElement presence = doc.CreateElement("presence");
				XmlElement xStatus = doc.CreateElement("newstatus");
				XmlElement xShow = doc.CreateElement("show");
				presence.SetAttribute("from", mUsername);
				xStatus.InnerText = "Away from Keyboard";
				presence.AppendChild(xStatus);
				xShow.InnerText = "away";
				presence.AppendChild(xShow);

				jc.Write(presence);
			} else if (newstatus == IMStatus.INVISIBLE) {
				if (IsOnlineStatus(mStatus))
					Disconnect();
			}
			mStatus = newstatus;
		}
		public override void AddFriend(string name, string nickname, string group)
		{
			string[] groups = {group};
			jc.Subscribe(new JID(name), nickname, groups);
		}
		public override void SendMessage(string friendName, string message)
		{
			if (jc.IsAuthenticated)
			{
				jc.Message(friendName, message);
			}
		}
		protected override void ChangeAvatar()
		{
			throw new NotImplementedException();
		}
		public override void IsTyping(string uname, bool isTyping)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("message");
			root.SetAttribute("from", mUsername);
			root.SetAttribute("to", uname);
			root.SetAttribute("type", "chat");
			XmlElement elem = null;
			if (isTyping)
			{
				elem = doc.CreateElement("composing", "http://jabber.org/protocol/chatstates");
			} else {
				elem = doc.CreateElement("inactive", "http://jabber.org/protocol/chatstates");
			}
			root.AppendChild(elem);

			jc.Write(root.OuterXml);
		}
		public override void SetStatusMessage(string status)
		{
			throw new NotImplementedException();
		}
		public override void BuzzFriend(string uname)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("message");
			XmlElement elem = doc.CreateElement("attention", "url:xmpp:attention:0");
			root.SetAttribute("from", mUsername);
			root.SetAttribute("to", uname);
			root.SetAttribute("type", "headline");
			root.AppendChild(elem);

			jc.Write(root);
		}
		public override string GetServerString(string username)
		{
			if (username.Contains("@"))
			{
				Char[] chars = { '@' };
				string[] Lusr = username.Split(chars);
				return Lusr[1] + ":5222";
			} else {
				return "";
			}
		}
		public override bool IsOnlineStatus(IMStatus status)
		{
			if (status == IMStatus.OFFLINE || status == IMStatus.INVISIBLE)
				return false;
			else
				return true;
		}
		public override bool IsOnlineStatusToOthers(IMStatus status)
		{
			return IsOnlineStatus(status);
		}

		// Properties
		public string Resource
		{
			get {
				return mResource;
			}
			set {
				mResource = value;
			}
		}

		// Callbacks
		private void onReadText(object sender, string txt)
		{
		}
		private void onMessage(object sender, Message smsg)
		{
			smsg = smsg;
		}
		private void onAuthenticate(object sender)
		{
			jc.Presence(PresenceType.available, "", "", 0);
			IMBuddy buddy = new IMBuddy(this, mUsername);
			buddy.IsInternalBuddy = true;
			buddylist.Add(buddy);
			mConnected = true;
			status = IMProtocolStatus.ONLINE;
			mLoginWaitHandle.Set();
		}
		private void onClientError(object sender, System.Exception e)
		{
			triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR));
			status = IMProtocolStatus.ERROR;		
		}
		private void onSessionChange(object sender, jabber.JID jid)
		{
			jid = jid;
		}
		private void onRosterEnd(object sender)
		{
			RosterManager roster = ((RosterManager)sender);
		}
		private void onRosterItem(object sender, jabber.protocol.iq.Item it)
		{
			RosterManager roster = ((RosterManager)sender);
			Presence jidStatus = presence[it.JID];
			bool isOnline = false;

			try {
				string status = jidStatus.Status;
				isOnline = true;
			} catch (Exception) {
				// Worthless exception I don't care about it
			}

			try {
				IMBuddy.FromUsername(it.JID.ToString(), this);
			} catch (Exception) {
				IMBuddy buddy = new IMBuddy(this, it.JID.ToString());
				buddy.Group = "Buddies";

				buddylist.Add(buddy);

				buddy.Online = isOnline;
			}
			blistChange = true;
		}
		private void onAuthError(object sender, System.Xml.XmlElement error)
		{
			triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_PASSWORD));
			status = IMProtocolStatus.ERROR;
		}
		private bool onInvalidCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		private void onIQ(object sender, IQ iq)
		{
			if (iq.Type == IQType.get)
			{
				if (iq.InnerXml.Contains("disco#info")) // Quick fix
				{
					iq = iq;
				}
			}
		}
		private void onPresence(object sender, Presence presencePacket)
		{
			if (presencePacket.Status != null)
			{
				IMBuddy buddy = IMBuddy.FromUsername(presencePacket.From.Bare, this);
				if (!buddy.IsInternalBuddy)
				{
					buddy.Online = true;
				}
			}
		}

		// Variables
		private JabberClient jc;
		private RosterManager roster;
		private PresenceManager presence;
		private DiscoManager disco;
		private CapsManager caps;
		private PubSubManager pubsub;
		private ConferenceManager muc;
		private string mResource;
	}
}