using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using InstantMessage.Events;

namespace InstantMessage
{
	[IMNetwork("yahoo")]
	public sealed partial class IMYahooProtocol : IMProtocol
	{
		public IMYahooProtocol()
		{
			protocolType = "Yahoo";
			mProtocolTypeShort = "yahoo";
			supportsBuzz = true;
			supportsMUC = true;
			supportsIntroMsg = true;
			supportsUserInvisiblity = true;
			mLoginWaitHandle = new ManualResetEvent(false);
		}
		public override void BeginLogin()
		{
			if (status != IMProtocolStatus.Offline)
				return;

			status = IMProtocolStatus.Connecting;
			base.BeginLogin();
			mLoginWaitHandle.Reset();

			Trace.WriteLine("Yahoo: Starting Login");

			// Do this the proper way.. thread it so the entire app doesn't lag when loading because of us
			beginAuthenticate();

			mEnabled = true;
		}
		public override void Disconnect()
		{
			CleanupBuddyList();

			if (status == IMProtocolStatus.Online) // Check to see if we were even connected.
			{
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_pager_logoff;
				packet.Session = session;

				if (authenticated)
					sendPacket(packet);

				status = IMProtocolStatus.Offline;
				mConnected = false;
				
				try {
					socket.Close();
				} catch (Exception) {}
			} else if (status == IMProtocolStatus.Connecting) {

			}
		}
		public override void SendMessage(string friendName, string message)
		{
			try {
				base.SendMessage(friendName, message);
			} catch (Exception) {
				return;
			}

			YPacket pkt = new YPacket();
			string newmessage = message;

			if (message.Length > 800)
			{
				newmessage = message.Substring(0, 800);
				SendMessage(friendName, message.Substring(801));
			}
			
			pkt.Service = YahooServices.ymsg_message;
			pkt.Session = session;
			pkt.AddParameter("1", mUsername);
			pkt.AddParameter("5", friendName);
			pkt.AddParameter("97", "1");
			pkt.AddParameter("63", ";0");
			pkt.AddParameter("64", "0");
			pkt.AddParameter("206", "0");
			pkt.AddParameter("14", newmessage);
			//pkt.AddParameter("429", "0000000" + converstationCount[friendName] + "c35AF042C"); // <-- Not sure what this does, but it seems to have to do with how many messages I've sent.. Maybe a delivery confirmation?
			pkt.AddParameter("450", "0");

			sendPacket(pkt);
		}
		public override void IsTyping(string uname, bool isTyping)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_notify;
			packet.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x16 };
			packet.AddParameter("49", "TYPING");
			packet.AddParameter("1", mUsername);
			packet.AddParameter("14", Encoding.UTF8.GetString(new byte[] { 20 }, 0, 1));
			if (isTyping)
				packet.AddParameter("13", "1");
			else
				packet.AddParameter("13", "0");
			packet.AddParameter("5", uname);

			sendPacket(packet);
		}
		public override void BuzzFriend(string uname)
		{
			SendMessage(uname, "<ding>");
		}
		public override string GetServerString(string username)
		{
			return "scs.msg.yahoo.com";
		}
		protected override void OnStatusChange(IMStatus oldStatus, IMStatus newStatus)
		{
			// Visibility Changes
			if (oldStatus != IMStatus.Invisible && newStatus == IMStatus.Invisible) // Going invisible
			{
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.Session = session;
				p1.AddParameter("13", "2");

				sendPacket(p1);

				foreach (var b in buddylist)
					b.mVisibilityStatus = UserVisibilityStatus.Offline; // Access the variable directly to prevent the class from telling us to switch stuff
			} else if (oldStatus == IMStatus.Invisible && newStatus != IMStatus.Invisible) {
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.Session = session;
				p1.AddParameter("13", "1");

				sendPacket(p1);
			}

			YPacket packet = generateStatusPacket(newStatus, mStatusMessage);

			sendPacket(packet);
		}
		public override void AddFriend(string name, string nickname, string group, string introMsg)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_add_buddy;
			packet.AddParameter("14", introMsg);
			packet.AddParameter("65", group);
			packet.AddParameter("97", "1");
			packet.AddParameter("216", String.Empty);
			packet.AddParameter("254", String.Empty);
			//packet.AddParameter("216", "First");
			//packet.AddParameter("254", "Last");
			packet.AddParameter("1", mUsername);
			packet.AddParameter("302", "319");
			packet.AddParameter("300", "319");
			packet.AddParameter("7", name);
			packet.AddParameter("301", "319");
			packet.AddParameter("303", "319");

			sendPacket(packet);

			var buddies = from IMBuddy b in buddylist where b.Username == name select new { b };
			foreach (var budd in buddies)
			{
				buddylist.Remove(budd.b);
			}

			IMBuddy buddy = new IMBuddy(this, name);
			buddy.StatusMessage = "Add request pending";
			buddy.Status = IMBuddyStatus.Offline;
			buddy.Group = group;
			buddylist.Add(buddy);

			if (!addbuddygroups.ContainsKey(name))
				addbuddygroups.Add(name, group);
			else
				addbuddygroups[name] = group;
		}
		public override void RemoveFriend(string uname, string group)
		{
			IMBuddy buddy = IMBuddy.FromUsername(uname, this);
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_remove_buddy;
			packet.Session = session;
			packet.AddParameter("1", mUsername);
			packet.AddParameter("7", uname);
			packet.AddParameter("65", group);

			sendPacket(packet);

			buddylist.Remove(buddy);

			if (false)
			{
				XDocument xml = new XDocument();
				xml.Declaration = new XDeclaration("1.0", "utf-8", "");

				XElement ab = new XElement("ab");
				XElement ct = new XElement("ct");

				ab.Add(new XAttribute("k", mUsername));
				ab.Add(new XAttribute("cc", 1));

				ct.Add(new XAttribute("d", 1));
				ct.Add(new XAttribute("yi", uname));
				ct.Add(new XAttribute("pr", 0));
				ct.Add(new XAttribute("id", buddy.Options["yid"]));

				ab.Add(ct);
				xml.Root.Add(ab);
			}
		}
		public override void SetPerUserVisibilityStatus(string buddy, UserVisibilityStatus status)
		{
			if (status == UserVisibilityStatus.Permanently_Offline)
			{
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_stealth_permanent;
				packet.AddParameter("1", mUsername);
				packet.AddParameter("31", "1");
				packet.AddParameter("13", "2");
				packet.AddParameter("302", "319");
				packet.AddParameter("300", "319");
				packet.AddParameter("7", buddy);
				packet.AddParameter("301", "319");
				packet.AddParameter("303", "319");

				sendPacket(packet);
			} else if (status == UserVisibilityStatus.Online) {
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_stealth_permanent;
				packet.AddParameter("1", mUsername);
				packet.AddParameter("31", "2");
				packet.AddParameter("13", "1");
				packet.AddParameter("302", "319");
				packet.AddParameter("300", "319");
				packet.AddParameter("7", buddy);
				packet.AddParameter("301", "319");
				packet.AddParameter("303", "319");

				sendPacket(packet);

				packet.Service = YahooServices.ymsg_stealth_session;
				packet.Parameter["31"] = "1";

				sendPacket(packet);

				YPacket spacket = new YPacket();
				spacket.Service = YahooServices.ymsg_status_update;
				spacket.AddParameter("10", "0");
				spacket.AddParameter("19", "");
				spacket.AddParameter("97", "1");

				sendPacket(spacket);
			}
		}
		public void StartIMVironment(string username, YahooIMVironment type)
		{
			if (type == YahooIMVironment.Doodle)
			{
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_file_transfer;
				packet.Session = session;
				packet.AddParameter("49", "IMVIRONMENT");
				packet.AddParameter("1", mUsername);
				packet.AddParameter("14", "null");
				packet.AddParameter("13", "4");
				packet.AddParameter("5", username);
				packet.AddParameter("63", "doodle;107");
				packet.AddParameter("64", "1");

				sendPacket(packet);

				packet.Parameter["4"] = "";
				packet.Parameter["13"] = "0";
				packet.Parameter["64"] = "0";

				sendPacket(packet);
			}
		}
		public override void InviteToChatRoom(string username, string room, string inviteText)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_conference_invitation;
			packet.AddParameter("1", mUsername);
			packet.AddParameter("50", mUsername);
			packet.AddParameter("57", room);
			packet.AddParameter("58", inviteText);
			packet.AddParameter("97", "1");
			packet.AddParameter("52", username);
			packet.AddParameter("13", "0");

			sendPacket(packet);
		}
		public override void ReplyToBuddyAddRequest(string username, bool isAdded)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_buddy_auth;
			packet.AddParameter("1", mUsername);
			packet.AddParameter("5", username);
			if (isAdded)
			{
				packet.AddParameter("13", "1");
			} else {
				packet.AddParameter("13", "2");
				packet.AddParameter("97", "1");
				packet.AddParameter("14", "");
			}

			sendPacket(packet);
		}
		public override void HandleProtocolCMDArg(string input)
		{
			// Break it down
			string method = input.Substring(6, input.IndexOf("?") - 6);

			if (method == "sendim")
			{
				string user = "";
				string message = "";
				if (input.Contains("&"))
				{
					user = input.Substring(input.IndexOf("?") + 1, input.IndexOf("&") - (input.IndexOf("?") + 1));
				} else {
					user = input.Substring(input.IndexOf("?") + 1);
				}
				if (input.Contains("&m="))
				{
					message = input.Substring(input.IndexOf("&m=") + 3);
					message = message.Replace("+", " ");
				}

				var buddy = from IMBuddy b in buddylist where b.Username == user select new { b };

				if (buddy.Count() > 0)
				{
				} else {
					IMBuddy nbuddy = new IMBuddy(this, user);
				}
			}
		}
		public override void SetStatusMessage(string message)
		{
			YPacket packet = generateStatusPacket(mStatus, message);
			packet.AddParameter("97", "1");
			packet.AddParameter("47", "0");
			packet.AddParameter("187", "0");

			sendPacket(packet);
		}

		// Packet Utility Functions
		private YPacket generateStatusPacket(IMStatus status, string statusMessage)
		{
			YPacket returnMe = new YPacket();
			returnMe.Service = YahooServices.ymsg_status_update;

			switch (status)
			{
				case IMStatus.Available:
					returnMe.AddParameter("10", "99");
					break;
				case IMStatus.Away:
					returnMe.AddParameter("10", "1");
					break;
				case IMStatus.Idle:
					returnMe.AddParameter("10", "99");
					break;
				case IMStatus.Busy:
					returnMe.AddParameter("10", "2");
					break;
			}

			returnMe.AddParameter("19", statusMessage == null ? "" : statusMessage);
			returnMe.AddParameter("97", "1");

			return returnMe;
		}
		private void HandleStatusData(IMBuddy buddy, YPacket packet)
		{
			if (!packet.Parameter.ContainsKey("10"))
				return;

			if (buddy.StatusMessage == "Idle")
				buddy.StatusMessage = String.Empty;

			if (packet.Parameter["10"] == "2") // Busy
			{
				buddy.Status = IMBuddyStatus.Busy;
			} else if (packet.Parameter["10"] == "999") { // Idle
				buddy.Status = IMBuddyStatus.Idle;
				if (buddy.StatusMessage == String.Empty)
					buddy.StatusMessage = "Idle";
			} else if (packet.Parameter["10"] == "0" || packet.Parameter["10"] == "99") { // Available
				buddy.Status = IMBuddyStatus.Available;
			} else if (packet.Parameter["10"] == "99") {
				buddy.Status = IMBuddyStatus.Away;
				buddy.StatusMessage = packet.Parameter["19"];
			}

			if (packet.Parameter.ContainsKey("19"))
			{
				buddy.StatusMessage = packet.Parameter["19"];
			} else {
				buddy.StatusMessage = "";
			}
		}

		private string yahoo64encode(byte[] data)
		{
			string returnVal = Convert.ToBase64String(data);

			returnVal = returnVal.Replace("+", ".");
			returnVal = returnVal.Replace("/", "_");
			returnVal = returnVal.Replace("=", "-");

			return returnVal;
		}
		private string yahoo64encode(string data)
		{
			return yahoo64encode(Encoding.UTF8.GetBytes(data));
		}
		/// <summary>
		/// Generates the Authentication hash used in the login process
		/// </summary>
		/// <param name="crumb">Crumb key from the web login page</param>
		/// <param name="challenge">Challenge key sent from the yahoo messenger servers.</param>
		/// <returns>String to be sent in the authentication response packet</returns>
		private string generateAuthHash(string crumb, string challenge)
		{
			byte[] bs = Encoding.UTF8.GetBytes(crumb + challenge);
#if SILVERLIGHT
			return MD5Core.GetHashString(crumb + challenge);
#else
			MD5CryptoServiceProvider md5sp = new MD5CryptoServiceProvider();
			bs = md5sp.ComputeHash(bs);
#endif
			StringBuilder s = new StringBuilder();
			foreach (byte b in bs)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			string md5 = s.ToString();
			//string returnVal = base64encode(md5);
			string returnVal = yahoo64encode(bs);

			return returnVal;
		}

		private void readDataAsync(IAsyncResult e)
		{
			try	{
				client.GetStream().EndRead(e);
			} catch (InvalidOperationException f) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR, f.Message));
				return;
			}

			string recvdata = GetPacketData();
			string[] sections = customSplit(recvdata, "YMSG"); // Sometimes multiple packets get stuffed into one. Break them apart

			for (int c = 0; c < sections.Length; c++)
			{
				if (sections[c].Length <= 20)
					continue;

				YPacket packet = YPacket.FromPacket("YMSG" + sections[c]);

				if (packet.Parameter.Count == 0)
					continue;

				string[] parameters = customSplit(sections[c].Substring(16), "À€");
				int serviceId = Convert.ToInt32(sections[c][7]);

				if (packet.StatusByte == new byte[] { 0xff, 0xff, 0xff, 0xff })
				{
					triggerOnDisconnect(this, new IMDisconnectEventArgs(DisconnectReason.Unknown));
					CleanupBuddyList();
					return;
				}

				if (packet.Service == YahooServices.ymsg_list_v15)
				{
					HandleListv15Packet(ref parameters);
				} else if (packet.Service == YahooServices.ymsg_status_v15) { // Status
					HandleStatusPacket(ref parameters);
				} else if (packet.Service == YahooServices.ymsg_message) { // Instant Message
					HandleMessagePacket(packet);
				} else if (packet.Service == YahooServices.ymsg_notify) {
					HandleNotifyPacket(packet);
				} else if (packet.Service == YahooServices.ymsg_pager_logoff) {
					HandlePagerLogoffPacket(packet);
				} else if (packet.Service == YahooServices.ymsg_newmail) {
					// 9 : Number of emails?
					HandleNewMessagePacket(packet);
				} else if (packet.Service == YahooServices.ymsg_sms_message) {
					HandleSMSMessagePacket(packet);
				} else if (packet.Service == YahooServices.ymsg_buddy_auth) {
					// 4 : From
					// 5 : Us
					// 14 : Request Message
					// 216 : From's First Name
					// 254 : From's Last Name
					HandleBuddyAuthRequest(packet);
				} else if (packet.Service == YahooServices.ymsg_status_update) {
					HandleStatusUpdatePacket(packet);
				} else if (packet.Service == YahooServices.ymsg_conference_invitation) {
					HandleConferenceInvitePacket(packet);
				} else if (packet.Service == YahooServices.ymsg_picture) {
					HandlePicturePacket(packet);
				}
			}

			if (status == IMProtocolStatus.Connecting)
			{
				status = IMProtocolStatus.Online;
				triggerOnLogin(null);
				mLoginWaitHandle.Set();
			}

			dataqueue = new byte[1024];
			try {
				receivePacket(new AsyncCallback(readDataAsync));
			} catch (Exception) {
				//Login();
			}
		}

		private string GetPacketData()
		{
			string pktdata = dEncoding.GetString(dataqueue, 0, dataqueue.Length);
			pktdata = pktdata.Trim(new char[] { '\0' });
			pktdata = pktdata.PadRight(20, '\0'); // Make sure we didn't chop off the packet part
			return pktdata;
		}
		protected static string[] customSplit(string inputstr, string separator)
		{
			List<string> splits = new List<string>();
			if (inputstr.Contains(separator))
			{
				int last = 0;
						
				while (true)
				{
					if (inputstr.StartsWith(separator) && last == 0)
					{
						last = separator.Length;
					}
					if (!inputstr.Substring(last).Contains(separator))
					{
						splits.Add(inputstr.Substring(last));
						break;
					}
					splits.Add(inputstr.Substring(last, inputstr.IndexOf(separator, last) - last));
					last = inputstr.IndexOf(separator, last) + separator.Length;
				}
			}

			return splits.ToArray();
		}
		private void HandleConferenceInvitePacket(YPacket packet)
		{
			triggerOnChatRoomInvite(new IMRoomInviteEventArgs(packet.Parameter["50"], packet.Parameter["50"], packet.Parameter["58"]));
		}
		private void HandleStatusUpdatePacket(YPacket packet)
		{
			HandleStatusData(IMBuddy.FromUsername(packet.Parameter["7"], this), packet);
		}
		private void HandleSMSMessagePacket(YPacket packet)
		{
			IMBuddy buddy = IMBuddy.FromUsername("+" + packet.Parameter["4"], this);
			var item = (from CarrierInfo i in mCarriers let y = packet.Parameter["68"] where i.carrierid == y select new { i.maxchars, i.humanname }).FirstOrDefault();
			buddy.MaxMessageLength = item.maxchars;
			buddy.InvokeReceiveMessage(packet.Parameter["68"]);
			if (!buddy.Options.ContainsKey("smscarrier"))
			{
				buddy.Options.Add("smscarrier", item.humanname);
				buddy.Options.Add("smscarriername", packet.Parameter["68"]);
			}
		}
		private void HandlePagerLogoffPacket(YPacket packet)
		{
			if (packet.Status == Encoding.UTF8.GetString(new byte[] { 0xff, 0xff, 0xff, 0xff }, 0, 4)) // We got disconnected
			{
				if (packet.Parameter["66"] == "42")
					triggerOnDisconnect(this, new IMDisconnectEventArgs(DisconnectReason.OtherClient));
				else
					triggerOnDisconnect(this, new IMDisconnectEventArgs(DisconnectReason.Unknown));
				status = IMProtocolStatus.Offline;
				CleanupBuddyList();
				Disconnect();
			} else {
				IMBuddy buddy = IMBuddy.FromUsername(packet.Parameter["7"], this);
				buddy.Online = false;
				buddy.Status = IMBuddyStatus.Offline;
				triggerContactStatusChange(new IMFriendEventArgs(buddy));
			}
		}
		private void HandleListv15Packet(ref string[] parameters)
		{
			status = IMProtocolStatus.Connecting; // Make sure we are still in a connecting state to prevent triggering the notification

			string currentgroup = "";
			IMBuddy buddy = null;

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i] == "65")
				{
					currentgroup = parameters[i + 1];
				} else if (parameters[i] == "7") {
					buddy = new IMBuddy(this, parameters[i + 1]);
					buddy.Group = currentgroup;
					buddy.mVisibilityStatus = UserVisibilityStatus.Online;
					buddylist.Add(buddy);
				} else if (parameters[i] == "317") {
					if (parameters[i + 1] == "2")
					{
						buddy.mVisibilityStatus = UserVisibilityStatus.Permanently_Offline;
					}
				} else if (parameters[i] == "223") {
					buddy.StatusMessage = "Add request pending";
				}
				// A Parameter 317 = 2 Means you show up as invisible to that person
				// A Parameter 442 Means you are a yahoo power user.
				// A Parameter 223 = 1 Means you are currently waiting for authorization
			}
		}
		private void HandleStatusPacket(ref string[] parameters)
		{
			if (!yaddrbookdld)
				YAddrBookDownload();

			IMBuddy buddy = null;
			List<YPacket> packets = new List<YPacket>();
			bool starttracking = false;
			YPacket pkt = null;

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i] == "241")
				{
					status = IMProtocolStatus.Connecting;
				} else if (parameters[i] == "7") {
					starttracking = true;
					if (pkt != null)
					{
						packets.Add(pkt);
					}
					pkt = new YPacket();
				}
				if (starttracking)
				{
					if (parameters.Length > i + 1)
						pkt.AddParameter(parameters[i], parameters[i + 1]);
					else
						packets.Add(pkt);
				}
				i++;
			}

			foreach (YPacket packet in packets)
			{
				if (packet.Parameter.ContainsKey("7"))
				{
					buddy = IMBuddy.FromUsername(packet.Parameter["7"], this);
					buddy.Online = true;
					buddy.Status = IMBuddyStatus.Available;
					HandleStatusData(buddy, packet);
					triggerContactStatusChange(new IMFriendEventArgs(buddy));
				}
				if (packet.Parameter.ContainsKey("197")) // Yahoo! Avatars code
				{
					buddy.Avatar = BuddyAvatar.FromUrl("http://img.avatars.yahoo.com/users/" + packet.Parameter["197"] + ".medium.png", packet.Parameter["197"]);
				} else if (packet.Parameter.ContainsKey("192"))	{ // Standard Avatar Hash - We must manually request the image data
					YPacket picpack = new YPacket();
					picpack.Service = YahooServices.ymsg_picture;
					picpack.Session = session;
					picpack.AddParameter("1", mUsername);
					picpack.AddParameter("5", packet.Parameter["7"]);
					picpack.AddParameter("13", "1");
					sendPacket(picpack);
				}
				if (packet.Parameter.ContainsKey("442")) // Yahoo Power Users thing - Just an aesthetic thing
				{
					string powericon = "";
					if (packet.Parameter["442"] == "1")
					{
						powericon = "crown";
					}
				}
			}
		}
		private void HandleMessagePacket(YPacket packet)
		{
			IMBuddy buddy = null;
			try {
				buddy = IMBuddy.FromUsername(packet.Parameter["4"], this);
			} catch (Exception) {
				buddy = new IMBuddy(this, packet.Parameter["4"]);
				buddy.IsOnBuddyList = false;
			}
			if (packet.Parameter["14"] == "<ding>")
				buddy.ReceiveBuzz();
			else {
				// Apply some transforms to the text first
				string newmsg = packet.Parameter["14"];
				newmsg = Regex.Replace(newmsg, "<((font face=\\?\"([a-zA-Z ]*)\\?\")|(fade (((#([a-z0-9]*)),?)*)))>", ""); // Remove font definitions

				// Use weird, messed up way to support yahoo UTF-8 messages
				buddy.InvokeReceiveMessage(newmsg);
				buddy.ShowIsTypingMessage(false);
				triggerOnMessageReceive(new IMMessageEventArgs(buddy, newmsg));
			}

			// Tell the Yahoo servers we got this message
			if (packet.Parameter.ContainsKey("429"))
			{
				YPacket resppkt = new YPacket();
				resppkt.Service = YahooServices.ymsg_message_reply;
				resppkt.AddParameter("1", mUsername);
				resppkt.AddParameter("5", packet.Parameter["4"]);
				resppkt.AddParameter("302", "430");
				resppkt.AddParameter("430", packet.Parameter["429"]);
				resppkt.AddParameter("303", "430");
				resppkt.AddParameter("450", "0");

				sendPacket(resppkt);
			}
		}
		private void HandleNotifyPacket(YPacket packet)
		{
			IMBuddy buddy = null;
			try {
				buddy = IMBuddy.FromUsername(packet.Parameter["4"], this);
			} catch (Exception) {
				buddy = new IMBuddy(this, packet.Parameter["4"]);
				buddy.IsOnBuddyList = false;
			}
			if (packet.Parameter["49"] == "TYPING")
			{
				if (packet.Parameter["13"] == "1")
					buddy.ShowIsTypingMessage(true);
				else
					buddy.ShowIsTypingMessage(false);
			}
		}
		private void HandlePicturePacket(YPacket packet)
		{
			if (packet.Parameter.ContainsKey("13") && packet.Parameter["13"] == "2" && packet.Parameter.ContainsKey("192"))
			{
				IMBuddy buddy = IMBuddy.FromUsername(packet.Parameter["4"], this);
				buddy.Avatar = BuddyAvatar.FromUrl(packet.Parameter["20"], packet.Parameter["192"]);
			}
		}
		/// <summary>
		/// BuddyAuthRequest is triggered whenever an add buddy request is received or a response is received
		/// </summary>
		private void HandleBuddyAuthRequest(YPacket packet)
		{
			if (packet.Parameter.ContainsKey("13")) { // Key 13 is a response to a buddy add request
				if (packet.Parameter["13"] == "1")
				{
					IMBuddy buddy = IMBuddy.FromUsername(packet.Parameter["4"], this);
					buddy.StatusMessage = "";
				}
				addbuddygroups.Remove(packet.Parameter["4"]);
			} else {
				if (packet.Parameter.ContainsKey("14"))
					triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameter["4"], packet.Parameter["14"], packet.Parameter["216"] + " " + packet.Parameter["254"]));
				else {
					if (packet.Parameter.ContainsKey("216"))
						triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameter["4"], packet.Parameter["216"] + " " + packet.Parameter["254"]));
					else
						triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameter["4"], ""));
				}
			}
		}
		/// <summary>
		/// This is triggered whenever you receive an email in your yahoo accounts
		/// </summary>
		private void HandleNewMessagePacket(YPacket packet)
		{
			if (packet.Parameter["9"] != "0") // Are there any email
			{
				if (packet.Parameter.ContainsKey("42"))
				{
					triggerOnEmailReceive(this, new IMEmailEventArgs(packet.Parameter["42"], packet.Parameter["43"], packet.Parameter["18"]));
				} else {
					//Notifications.ShowNotification("You have mail : ");
				}
			}
		}

		private void OnGetYIPAddress(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			string streamBuf = "";

			if (response.StatusCode == HttpStatusCode.OK)
			{
				StreamReader reader = new StreamReader(response.GetResponseStream());
				streamBuf = reader.ReadToEnd();
				reader.Close();
			} else {
				Trace.WriteLine("Yahoo: Http server returned " + response.StatusCode.ToString() + " while getting CS_IP (" + response.StatusDescription + ")");
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.Unknown));
				return;
			}

			int start = streamBuf.IndexOf("CS_IP_ADDRESS") + 14;
			int end = (streamBuf.Length - start) - 2;

			connectServer = streamBuf.Substring(start, end);
			Trace.WriteLine("Yahoo: YMSG Communication Server: " + connectServer);

			if (token == "")
			{
				try	{
					token = mConfig["token"];
				} catch (KeyNotFoundException) {}
			}

			if (String.IsNullOrEmpty(token))
			{
				Trace.WriteLine("Yahoo: Token invalid, requesting new token");
				HttpWebRequest rqst = (HttpWebRequest)WebRequest.Create(string.Format("https://login.yahoo.com/config/pwtoken_get?src=ymsgr&login={0}&passwd={1}", Username, Password));

				receiveHttpData(rqst, new EventHandler<PacketEventArgs>(OnGetYToken));
			} else {
				Trace.WriteLine("Yahoo: Token valid, continuing");
				DoGetYCookies();
			}			
		}

		private string generateUrl(string addition)
		{
			if (urlstartaddr != "")
				return urlstartaddr + System.Web.HttpUtility.UrlEncode(addition);
			else
				return addition;
		}

		private void OnGetYCookies(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string streamBuf = reader.ReadToEnd();
			int validfor = 0;

			// Parsing
			int result = Convert.ToInt32(Regex.Match(streamBuf, @"^([0-9]*)").Groups[1].Value);
			if (result == 0)
			{
				Match key = Regex.Match(streamBuf, @"(v=[^;]*)");
				Match key2 = Regex.Match(streamBuf, @"(z=[^;]*)");
				Match key3 = Regex.Match(streamBuf, @"crumb=([\S]*)");
				authtoken = key.Groups[1].Value.ToString();
				authtoken2 = key2.Groups[1].Value.ToString();
				crumb = key3.Groups[1].Value.ToString();
				Match valid = Regex.Match(streamBuf, "cookievalidfor=([0-9]*)");
				validfor = Convert.ToInt32(valid.Groups[1].Value);
			} else if (result == 100) {
				beginAuthenticate();
				return;
			}

			mConfig["authtoken1"] = authtoken;
			mConfig["authtoken2"] = authtoken2;
			mConfig["authcrumb"] = crumb;
			mConfig["tokenexpires"] = DateTime.UtcNow.AddSeconds(validfor).ToUnixEpoch().ToString();

			FinishAuth();
		}

		private void OnGetYToken(object sender, PacketEventArgs e)
		{
			string streamBuf = e.PacketData;

			// Parsing
			int result = Convert.ToInt32(Regex.Match(streamBuf, @"^([0-9]*)").Groups[1].Value);
			if (result == 0)
			{
				Match key = Regex.Match(streamBuf, @"ymsgr=([\S]*)");
				token = key.Groups[1].Value.ToString();

				Trace.WriteLine("Yahoo: Have auth token (" + token.Substring(0, 20) + "...)");

				if (savepassword)
					mConfig.Add("token", token);
			} else if (result == 1212) { // Invalid Credentials
				Trace.WriteLine("Yahoo: OnGetYToken got Invalid credentials Error. Handling");
				triggerBadCredentialsError();
				return;
			} else if (result == 1213) { // Security lock from too many invalid logins
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.LIMIT_REACHED, "Security Lock from too many invalid login attempts."));
				return;
			} else if (result == 1235) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_USERNAME));
				return;
			} else if (result == 1236) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.LIMIT_REACHED));
				return;
			}

			DoGetYCookies();
		}

		private void DoGetYCookies()
		{
			int expires = 0;
			try	{
				authtoken = mConfig["authtoken1"];
				authtoken2 = mConfig["authtoken2"];
				crumb = mConfig["authcrumb"];
				expires = Convert.ToInt32(mConfig["tokenexpires"]);
			} catch (KeyNotFoundException) {}

			int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

			// Check to see if we are missing any tokens or if the tokens have expired
			if (String.IsNullOrEmpty(authtoken) || String.IsNullOrEmpty(authtoken2) || String.IsNullOrEmpty(crumb) || epoch > expires)
			{
				Trace.WriteLineIf(epoch > expires, "Yahoo: Saved Auth2 tokens expired... Requesting");
				Trace.WriteLineIf(epoch < expires, "Yahoo: Saved Auth2 tokens invalid... Requesting");

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateUrl("https://login.yahoo.com/config/pwtoken_login?src=ymsgr&token=" + token));
				request.BeginGetResponse(new AsyncCallback(OnGetYCookies), request);
			} else {
				Trace.WriteLine("Yahoo: All Authentication tokens are ready.");
				FinishAuth();
			}
		}

		private void FinishAuth()
		{
			// These tokens are later used for address book downloading and other yahoo specific details
			logincookies.Add("Y", authtoken);
			logincookies.Add("T", authtoken2);

			authtoken += "; path=/; domain=.yahoo.com";
			authtoken2 += "; path=/; domain=.yahoo.com";

			client = new TcpClient();

			Trace.WriteLine("Yahoo: Attempting connection to YMSG server");

			client.Connect(IPAddress.Parse(connectServer), 5050);

			nWriter = new StreamWriter(client.GetStream());
			nReader = new StreamReader(client.GetStream());

			nWriter.AutoFlush = true; // We want all data to get sent immediately after we write it to the stream

			//if (client.Connected)
				OnYServerConnect();
		}

		/// <summary>
		/// Step 1.
		/// Begins authentication process of connecting to the yahoo servers
		/// by getting the ip address to use to connect to.
		/// </summary>
		private void beginAuthenticate()
		{
			logincookies.Clear(); // Just in-case this is the second time we are signing in

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://vcs.msg.yahoo.com/capacity");
			request.ServicePoint.Expect100Continue = false; // Yahoo doesn't like these

			try	{
				request.BeginGetResponse(new AsyncCallback(OnGetYIPAddress), request);
			} catch (WebException e) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR, e.Message));
			}
		}
		
		// Begin Packet Stuff
		// Part 1
		private void OnYServerConnect()
		{
			YPacket verpkt = new YPacket();
			verpkt.Service = YahooServices.ymsg_verify;

			sendPacket(verpkt);

			receivePacket(new AsyncCallback(SendInitAuthPkt));
		}

		private void SendInitAuthPkt(IAsyncResult e)
		{
			client.GetStream().EndRead(e);
			// Send Authentication Packet
			YPacket authpkt = new YPacket();
			authpkt.Service = YahooServices.ymsg_authentication;
			authpkt.AddParameter("1", mUsername);

			sendPacket(authpkt);

			receivePacket(new AsyncCallback(OnGetAuthRespPacket));

		}
		private void OnGetAuthRespPacket(IAsyncResult e)
		{
			client.GetStream().EndRead(e);
			string authreturn = GetPacketData();

			YPacket p = YPacket.FromPacket(authreturn);
			session = p.Session;
			string challenge = p.Parameter["94"];

			YPacket authpkt = new YPacket();
			authpkt.Service = YahooServices.ymsg_authentication_response;
			if (mStatus == IMStatus.Invisible)
				authpkt.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x0C }; // <-- Sign is as invisible
			else
				authpkt.StatusByte = new byte[] { 0x5A, 0x55, 0xAA, 0x55 };
			authpkt.Session = session;
			authpkt.AddParameter("1", mUsername);
			authpkt.AddParameter("0", mUsername);
			authpkt.AddParameter("277", authtoken);
			authpkt.AddParameter("278", authtoken2);
			authpkt.AddParameter("307", generateAuthHash(crumb, challenge));
			authpkt.AddParameter("244", "8388543"); // This *might* be a features list
			authpkt.AddParameter("2", mUsername);
			byte[] unknownbyte = new byte[] { 0x42, 0x09, 0x30, 0x6c, 0x37, 0x64, 0x61, 0x32, 0x74, 0x35, 0x34, 0x37, 0x63, 0x75, 0x32, 0x26, 0x62, 0x3d, 0x33, 0x26, 0x73, 0x3d, 0x38, 0x6e };
			authpkt.AddParameter("59", dEncoding.GetString(unknownbyte, 0, unknownbyte.Length));
			authpkt.AddParameter("98", "us");
			authpkt.AddParameter("135", "10.0.0.525"); // Version Info

			sendPacket(authpkt);
			
			receivePacket(new AsyncCallback(readDataAsync));

			//socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(this.readDataAsync), this);

			Timer keepalive = new Timer(new TimerCallback(this.sendKeepAlive), null, 60 * 1000, 60 * 1000);
			authenticated = true;

			Trace.WriteLine("Yahoo: YMSG Authentication should be complete");

			lock (queuedpackets) // We have to add this here because for some reason, the queuepackets list got modified one time while it was connecting.. How?
			{
				foreach (YPacket packet in queuedpackets)
				{
					sendPacket(packet);
				}
				queuedpackets.Clear();
			}
		}

#if PocketPC || WINDOWS
		private void OnPacketReceive(IAsyncResult e)
		{
			EventHandler<PacketEventArgs> callback = (EventHandler<PacketEventArgs>)e.AsyncState;
			string pktdata = dEncoding.GetString(dataqueue, 0, dataqueue.Length);
#else
		private void OnPacketReceive(object sender, SocketAsyncEventArgs e)
		{
			EventHandler<PacketEventArgs> callback = (EventHandler<PacketEventArgs>)e.UserToken;
			string pktdata = dEncoding.GetString(e.Buffer, 0, e.Buffer.Length);
		
#endif
			pktdata = pktdata.Trim(new char[] { '\0' });
			pktdata = pktdata.PadRight(20, '\0'); // Make sure we didn't chop off the packet part
			YPacket pkt = YPacket.FromPacket(pktdata);

			if (pkt.StatusByte == new byte[] { 0xff, 0xff, 0xff, 0xff })
			{

			}

			callback(this, new PacketEventArgs(pktdata));
		}

		private void OnAfterYAddressBookDl(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream book = response.GetResponseStream();
				string bxml = (new StreamReader(book)).ReadToEnd();
				XDocument xml = XDocument.Parse(bxml);

				IEnumerator contacts = xml.Root.Descendants().GetEnumerator();
				while (contacts.MoveNext())
				{
					XElement elem = (XElement)contacts.Current;

					IMBuddy buddy = null;
					try
					{
						buddy = IMBuddy.FromUsername(elem.Attribute("yi").Value, this);

					} catch (Exception)	{
						if (elem.Attribute("yi") != null)
						{
							buddy = new IMBuddy(this, elem.Attribute("yi").Value);
							buddy.Group = "Address Book";
							buddylist.Add(buddy);
						}
						if (elem.Attribute("mo") != null)
						{
							buddy = new IMBuddy(this, elem.Attribute("mo").Value);
							buddy.Group = "Mobile";
							buddy.IsMobileContact = true;
							buddylist.Add(buddy);
						}
					}
					if (elem.Attribute("nn") != null) // Nickname
					{
						buddy.Nickname = elem.Attribute("nn").Value;
					} else {
						string nick = "";
						if (elem.Attribute("fn") != null)
						{
							nick = elem.Attribute("fn").Value;
						}
						if (elem.Attribute("ln") != null)
						{
							if (nick != "")
							{
								nick += " ";
							}
							nick += elem.Attribute("ln").Value;
						}
						buddy.Nickname = nick;
					}
					if (elem.Attribute("id") != null && !buddy.Options.ContainsKey("yid"))
					{
						buddy.Options.Add("yid", elem.Attribute("id").Value);
					}
					if (elem.Attribute("mo") != null)
					{
						buddy.SMSNumber = elem.Attribute("mo").Value;
						if (elem.Attribute("vm") != null)
						{
							buddy.Options.Add("smscarrierid", elem.Attribute("vm").Value);
						} else {
							buddy.MaxMessageLength = 145;
						}
					}
					//buddylist.Add(buddy);
				}
			}
			updateMobileCarrierInfo();
		}

		// Callbacks
		private void onYAddressBookDownload()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlstartaddr + mAddressBookUrl);
			request.CookieContainer = new CookieContainer();
			request.CookieContainer.Add(new Uri("http://yahoo.com"), new Cookie("Y", logincookies["Y"], "/", ".yahoo.com"));
			request.CookieContainer.Add(new Uri("http://yahoo.com"), new Cookie("T", logincookies["T"], "/", ".yahoo.com"));
			request.BeginGetResponse(new AsyncCallback(OnAfterYAddressBookDl), request);
		}

#if SILVERLIGHT
		private void OnReceiveHttpData(IAsyncResult e)
		{
			HttpAsync state = (HttpAsync)e.AsyncState;
			HttpWebRequest request = (HttpWebRequest)state.request;
			EventHandler<PacketEventArgs> callback = state.callback;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
#else
		private void OnReceiveHttpData(HttpWebRequest request, EventHandler<PacketEventArgs> callback)
		{
			HttpWebResponse response = null;
			try	{
				response = (HttpWebResponse)request.GetResponse();
			} catch (WebException) {
				return;
			}
#endif
			StreamReader reader = new StreamReader(response.GetResponseStream());

			callback(this, new PacketEventArgs(reader.ReadToEnd()));
		}

		private void SmsDataDownload(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			Stream stream = response.GetResponseStream();

			XmlDocument doc = new XmlDocument();
			doc.Load(stream);

			parseSmsData(doc);
		}
		
		private void updateMobileCarrierInfo()
		{
			if (!mCompletedCarrierSetup)
				return;

			foreach (IMBuddy buddy in buddylist)
			{
				var carrier = (from CarrierInfo c in mCarriers let y = buddy.Options["smscarrierid"] where c.carrierid == y select new { CompanyName = c.humanname, MaxChars = c.maxchars }).First();
				if (carrier != null)
				{
					buddy.MaxMessageLength = carrier.MaxChars;
					buddy.Options.Add("smscarriername", carrier.CompanyName);
				}
			}
		}

		/// <summary>
		/// Parses an xml document containing data on sms carriers into an internal list of all carriers.
		/// </summary>
		/// <param name="doc">XmlDocument either loaded from a cache on the disk or from the internet containing data on the sms carriers.</param>
		private void parseSmsData(XmlDocument doc)
		{
			XmlElement carriers = doc.DocumentElement["sms"]["carrierslist"];

			foreach (XmlElement carrier in carriers)
			{
				CarrierInfo info = new CarrierInfo();
				info.carrierid = carrier.GetAttribute("id");
				info.humanname = carrier.GetAttribute("name");
				info.maxchars = Convert.ToInt32(carrier.GetAttribute("maxchars"));
				mCarriers.Add(info);
			}

			mCompletedCarrierSetup = true;
		}
		/// <summary>
		/// Downloads an xml file from Yahoo! containing information on SMS data
		/// </summary>
		private void YSMSDataDownload()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mSMSrequest.Replace("{crc}", "0"));

			request.BeginGetResponse(new AsyncCallback(SmsDataDownload), request);
		}
		private void YAddrBookDownload()
		{
			Trace.WriteLine("Yahoo: Downloading Yahoo Address Book");
			Thread addrThread = new Thread(new ThreadStart(this.onYAddressBookDownload));
#if !SILVERLIGHT
			addrThread.Priority = ThreadPriority.Lowest;
#endif
			addrThread.Start();
			yaddrbookdld = true;
		}
		private void sendKeepAlive(object state)
		{
			YPacket keepalive = new YPacket();
			keepalive.Service = YahooServices.ymsg_keepalive;
			keepalive.Session = session;
			keepalive.AddParameter("0", mUsername);

			sendPacket(keepalive);
		}
		private void sendInvisibleCheckPacket(string destination)
		{
			// http://opi.yahoo.com/online?u={username}&m=t&t=1
			YPacket packet = new YPacket();
			packet.ServiceByte = new byte[] { 0x00, 0xc1 };
			packet.Session = session;
			packet.AddParameter("1", mUsername);
			packet.AddParameter("5", destination);
			packet.AddParameter("206", "1");

			sendPacket(packet);
		}
		private void sendPacket(YPacket packet)
		{
			if (client == null)
				throw new NullReferenceException("Client is null");

			if (session != "")
				packet.Session = session;

			try	{
				client.GetStream().Write(packet.ToBytes(), 0, packet.ToBytes().Length);
				return;
#if PocketPC || WINDOWS
				socket.Send(packet.ToBytes());
#else
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.SetBuffer(packet.ToBytes(), 0, packet.ToBytes().Length);
				args.SocketFlags = SocketFlags.None;
				args.SendPacketsFlags = 0;
				socket.SendAsync(args);
#endif
			} catch (IOException) {
				lock (queuedpackets)
				{
					queuedpackets.Add(packet);
				}
				
				authenticated = false;
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR));
			}
		}
		private void receivePacket(AsyncCallback callback)
		{
			if (callback != null)
				client.GetStream().BeginRead(dataqueue, 0, dataqueue.Length, callback, null);
		}
		private void receiveHttpData(HttpWebRequest request, EventHandler<PacketEventArgs> callback)
		{
#if SILVERLIGHT
			request.BeginGetResponse(new AsyncCallback(OnReceiveHttpData), new HttpAsync(request, callback));
#else
			OnReceiveHttpData(request, callback);
#endif
		}

		// Protocol Specific Variables
		private byte[] dataqueue = new byte[1024];
		private Dictionary<string, int> converstationCount = new Dictionary<string, int>();
		private string session = "";
		private Socket socket = null;
		private TcpClient client;
		private StreamWriter nWriter;
		private StreamReader nReader;
		private bool yaddrbookdld = false;
		private bool authenticated = false;
		private string token = "";
		private string authtoken = "";
		private string authtoken2 = "";
		private string crumb = "";
		private string connectServer = "";
		private string urlstartaddr = "";
		private bool mCancelConnect; // Set to true to cancel a current authentication attempt
		
		// SMS Carrier Information
		private List<CarrierInfo> mCarriers = new List<CarrierInfo>();
		private bool mCompletedCarrierSetup = false;
		
		private Dictionary<string, string> roominvites = new Dictionary<string, string>();
		private Dictionary<string, string> addbuddygroups = new Dictionary<string, string>(); // Remembers what group the buddy goes into
		private Dictionary<string, string> logincookies = new Dictionary<string, string>();
		private List<YPacket> queuedpackets = new List<YPacket>();
		private string mSMSrequest = "http://insider.msg.yahoo.com/ycontent/?&sms={crc}&intl=us&os=win&ver=10.0.0.1102";
		private string mAddressBookUrl = "http://address.yahoo.com/yap/us?v=XM&prog=ymsgr&prog-ver=10.0.0.1102&useutf8=1&legenc=codepage-1252";
		protected static Encoding dEncoding = Encoding.GetEncoding("windows-1252");

		private enum YahooServices
		{
			ymsg_pager_logoff = 0x02,
			ymsg_message = 0x06,
			ymsg_newmail = 0x0b,
			ymsg_skinname = 0x15,
			ymsg_ping = 0x12,
			ymsg_conference_invitation = 0x18,
			ymsg_conference_logon = 0x19,
			ymsg_conference_additional_invite = 0x1c,
			ymsg_conference_message = 0x1d,
			ymsg_conference_logoff = 0x1b,
			ymsg_notify = 0x4b,
			ymsg_verify = 0x4c,
			ymsg_p2p_file_transfer = 0x4d,
			ymsg_peer2peer = 0x4f,
			ymsg_webcam = 0x50,
			ymsg_authentication_response = 0x54,
			ymsg_list = 0x55,
			ymsg_authentication = 0x57,			
			ymsg_keepalive = 0x82,
			ymsg_add_buddy = 0x83,
			ymsg_remove_buddy = 0x84,
			ymsg_stealth_session = 0xba,
			ymsg_picture = 0xbe,
			ymsg_stealth_permanent = 0xb9,
			ymsg_visibility_toggle = 0xc5,
			ymsg_status_update = 0xc6,
			ymsg_picture_update = 0xc1,
			ymsg_file_transfer = 0xdc,
			ymsg_contact_details = 0xd3,
			ymsg_buddy_auth = 0xd6,
			ymsg_status_v15 = 0xf0,
			ymsg_list_v15 = 0xf1,
			ymsg_message_reply = 0xfb, // Reply saying you got this message
			ymsg_sms_message = 0x2ea
		}

		private class YPacket
		{
			public YPacket()
			{

			}
			/// <summary>
			/// Disassembles a Y! MSG packet from the string and returns a YPacket built from the input
			/// </summary>
			/// <param name="packetdata">String containing the packet data (Ex. "YMSG...")</param>
			/// <returns>YPacket class based on the input</returns>
			public static YPacket FromPacket(string packetdata)
			{
				YPacket packet = new YPacket();

				// Extract all the header information from the packet
				packet.ServiceByte = dEncoding.GetBytes(packetdata.Substring(10, 2));
				packet.StatusByte = dEncoding.GetBytes(packetdata.Substring(12, 4));
				packet.SessionByte = dEncoding.GetBytes(packetdata.Substring(16, 4));

				// Extract the payload
				string payload = packetdata.Substring(20);//, packetdata.Length - 20);
				
				// Split the payload up into fragments
				IEnumerator paramEnum = IMYahooProtocol.customSplit(payload, separator).GetEnumerator();

				while (paramEnum.MoveNext())
				{
					// The even fragments are the keys
					string param = (string)paramEnum.Current;
					if (paramEnum.MoveNext()) // Make sure there is a value for this key
					{
						// The odd fragments are the values
						string value = (string)paramEnum.Current;
						packet.AddParameter(param, value);
					} else
						break;
				}

				return packet;
			}
			public static YPacket FromWebPacket(string packetdata)
			{
				YPacket packet = new YPacket();
				XDocument xml = XDocument.Parse(packetdata);

				byte[] test = new byte[] { 0, 0, 0, 0 };

				packet.Service = (YahooServices)Convert.ToInt32(xml.Root.Attributes("Command"));

				string[] paramList = customSplit(xml.Root.Value,"^$");

				for (int i = 0; i < paramList.Length; i++)
				{
					string key = paramList[i];
					i++;
					packet.AddParameter(key, paramList[i]);
				}

				return packet;
			}

			public byte[] ToBytes()
			{
				return dEncoding.GetBytes(Packet); //Encoding.UTF8.GetBytes(Packet);
			}

			public string Packet
			{
				get {
					pktdata = CalculateContents();
					byte[] lens = BitConverter.GetBytes(pktdata.Length);
					byte[] lens2 = new byte[] { lens[1], lens[0] };

					return "YMSG" + dEncoding.GetString(version, 0, version.Length) + nils + nils + dEncoding.GetString(lens2, 0, lens2.Length) + dEncoding.GetString(service, 0, service.Length) + dEncoding.GetString(status, 0, status.Length) + dEncoding.GetString(session, 0, session.Length) + pktdata;
				}
			}
			public string Contents
			{
				get	{
					return CalculateContents();
				}
				set	{
					pktdata = value;
				}
			}
			public string Session
			{
				get	{
					return dEncoding.GetString(session, 0, session.Length);
				}
				set	{
					session = dEncoding.GetBytes(value);
				}
			}
			public byte[] SessionByte
			{
				get	{
					return session;
				}
				set	{
					session = value;
				}
			}
			public string Status
			{
				get	{
					return Encoding.UTF8.GetString(status, 0, status.Length);
				}
				set	{
					status = Encoding.UTF8.GetBytes(value);
				}
			}
			public byte[] StatusByte
			{
				get {
					return status;
				}
				set {
					status = value;
				}
			}
			public YahooServices Service
			{
				get	{
					return (YahooServices)service[1];
				}
				set	{
					service[1] = (byte)value;
				}
			}
			public byte[] ServiceByte
			{
				get	{
					return service;
				}
				set	{
					service = value;
				}
			}
			public Dictionary<string, string> Parameter
			{
				get	{
					return parameters;
				}
			}
			private string CalculateContents()
			{
				string returnVal = "";
				IEnumerator i = parameters.GetEnumerator();
				while (i.MoveNext())
				{
					KeyValuePair<string, string> current = (KeyValuePair<string, string>)i.Current;
					if (current.Value.Contains(separator))
					{
						foreach (string val in customSplit(current.Value, separator))
						{
							returnVal += current.Key + separator + val + separator;
						}
					} else {
						returnVal += current.Key + separator + current.Value + separator;
					}
				}
				return returnVal;
			}

			public void AddParameter(string key, string value)
			{
				if (!parameters.ContainsKey(key))
					parameters.Add(key, value);
				else {
					parameters[key] += separator + value;
				}
			}

			private string pktdata = "";
			private byte[] service = new byte[] { 0x00, 0x00 };
			private byte[] status = new byte[] { 0x00, 0x00, 0x00, 0x00 };
			private byte[] session = new byte[] { 0x00, 0x00, 0x00, 0x00 };
			private byte[] version = new byte[] { 0x00, 0x11 }; // YMSG17
			public static string separator = "À€"; //dEncoding.GetString(new byte[] { 0xC0, 0x80 }, 0, 2); //"À€"; // "Ŕ€";
			private string nils = dEncoding.GetString(new byte[] { 0x00 }, 0, 1);
			private Dictionary<string, string> parameters = new Dictionary<string, string>();
		}

		private struct HttpAsync
		{
			public HttpAsync(HttpWebRequest f, EventHandler<PacketEventArgs> c)
			{
				request = f;
				callback = c;
			}
			public HttpWebRequest request;
			public EventHandler<PacketEventArgs> callback;
		}
		private struct PacketAsync
		{
			public PacketAsync(HttpWebRequest f, YPacket p)
			{
				request = f;
				packet = p;
			}
			public HttpWebRequest request;
			public YPacket packet;
		}
		public enum YahooIMVironment
		{
			Doodle
		}
	}
	public struct CarrierInfo
	{
		public string carrierid;
		public int maxchars;
		public string humanname;
	}
}