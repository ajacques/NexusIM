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

namespace InstantMessage.Protocols.Yahoo
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
		public override string GetServerString(string username)
		{
			return "scs.msg.yahoo.com";
		}

		// Basic State Functions
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

				if (authenticated)
					sendPacket(packet);

				status = IMProtocolStatus.Offline;
				mConnected = false;
			} else if (status == IMProtocolStatus.Connecting) {

			}
		}

		// Basic Messaging Functions
		public override void SendMessage(string userName, string message)
		{
			try {
				base.SendMessage(userName, message);
			} catch (Exception) {
				return;
			}

			YPacket pkt = new YPacket();
			string newmessage = message;

			if (message.Length > 800)
			{
				newmessage = message.Substring(0, 800);
				SendMessage(userName, message.Substring(801));
			}
			
			pkt.Service = YahooServices.ymsg_message;
			pkt.AddParameter(1, mUsername);
			pkt.AddParameter(5, userName);
			pkt.AddParameter(97, "1");
			pkt.AddParameter(63, ";0");
			pkt.AddParameter(64, "0");
			pkt.AddParameter(206, "0");
			pkt.AddParameter(14, newmessage);
			//pkt.AddParameter(429, "0000000" + converstationCount[friendName] + "c35AF042C"); // <-- Not sure what this does, but it seems to have to do with how many messages I've sent.. Maybe a delivery confirmation?
			pkt.AddParameter(450, "0");

			sendPacket(pkt);
		}
		public override void IsTyping(string uname, bool isTyping)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_notify;
			packet.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x16 };
			packet.AddParameter(49, "TYPING");
			packet.AddParameter(1, mUsername);
			packet.AddParameter(14, Encoding.UTF8.GetString(new byte[] { 20 }, 0, 1));
			if (isTyping)
				packet.AddParameter(13, "1");
			else
				packet.AddParameter(13, "0");
			packet.AddParameter(5, uname);

			sendPacket(packet);
		}
		public override void BuzzFriend(string uname)
		{
			SendMessage(uname, "<ding>");
		}

		// Contact List Management Functions		
		public override void AddFriend(string name, string nickname, string group, string introMsg)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_add_buddy;
			packet.AddParameter(14, introMsg);
			packet.AddParameter(65, group);
			packet.AddParameter(97, "1");
			packet.AddParameter(216, String.Empty);
			packet.AddParameter(254, String.Empty);
			//packet.AddParameter(216, "First");
			//packet.AddParameter(254, "Last");
			packet.AddParameter(1, mUsername);
			packet.AddParameter(302, "319");
			packet.AddParameter(300, "319");
			packet.AddParameter(7, name);
			packet.AddParameter(301, "319");
			packet.AddParameter(303, "319");

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
		public override void ReplyToBuddyAddRequest(string username, bool isAdded)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_buddy_auth;
			packet.AddParameter(1, mUsername);
			packet.AddParameter(5, username);
			if (isAdded)
			{
				packet.AddParameter(13, "1");
			} else {
				packet.AddParameter(13, "2");
				packet.AddParameter(97, "1");
				packet.AddParameter(14, "");
			}

			sendPacket(packet);
		}

		public override void InviteToChatRoom(string username, string room, string inviteText)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_conference_invitation;
			packet.AddParameter(1, mUsername);
			packet.AddParameter(50, mUsername);
			packet.AddParameter(57, room);
			packet.AddParameter(58, inviteText);
			packet.AddParameter(97, "1");
			packet.AddParameter(52, username);
			packet.AddParameter(13, "0");

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

		// Advanced Presence Functions
		public override void SetPerUserVisibilityStatus(string buddy, UserVisibilityStatus status)
		{
			if (status == UserVisibilityStatus.Permanently_Offline)
			{
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_stealth_permanent;
				packet.AddParameter(1, mUsername);
				packet.AddParameter(31, "1");
				packet.AddParameter(13, "2");
				packet.AddParameter(302, "319");
				packet.AddParameter(300, "319");
				packet.AddParameter(7, buddy);
				packet.AddParameter(301, "319");
				packet.AddParameter(303, "319");

				sendPacket(packet);
			} else if (status == UserVisibilityStatus.Online) {
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_stealth_permanent;
				packet.AddParameter(1, mUsername);
				packet.AddParameter(31, "2");
				packet.AddParameter(13, "1");
				packet.AddParameter(302, "319");
				packet.AddParameter(300, "319");
				packet.AddParameter(7, buddy);
				packet.AddParameter(301, "319");
				packet.AddParameter(303, "319");

				sendPacket(packet);

				packet.Service = YahooServices.ymsg_stealth_session;
				packet.Parameters[31] = "1";

				sendPacket(packet);

				YPacket spacket = new YPacket();
				spacket.Service = YahooServices.ymsg_status_update;
				spacket.AddParameter(10, "0");
				spacket.AddParameter(19, "");
				spacket.AddParameter(97, "1");

				sendPacket(spacket);
			}
		}

		// Presence functions
		public override void SetStatusMessage(string message)
		{
			YPacket packet = generateStatusPacket(mStatus, message);
			packet.AddParameter(97, "1");
			packet.AddParameter(47, "0");
			packet.AddParameter(187, "0");

			sendPacket(packet);
		}
		protected override void OnStatusChange(IMStatus oldStatus, IMStatus newStatus)
		{
			// Visibility Changes
			if (oldStatus != IMStatus.Invisible && newStatus == IMStatus.Invisible) // Going invisible
			{
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.AddParameter(13, "2");

				sendPacket(p1);

				//foreach (var b in buddylist)
				//	b.mVisibilityStatus = UserVisibilityStatus.Offline; // Access the variable directly to prevent the class from telling us to switch stuff
			} else if (oldStatus == IMStatus.Invisible && newStatus != IMStatus.Invisible) {
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.AddParameter(13, "1");

				sendPacket(p1);
			}

			YPacket packet = generateStatusPacket(newStatus, mStatusMessage);

			sendPacket(packet);
		}

		// Protocol Specific functions
		public void StartIMVironment(string username, YahooIMVironment type)
		{
			if (type == YahooIMVironment.Doodle)
			{
				YPacket packet = new YPacket();
				packet.Service = YahooServices.ymsg_file_transfer;
				packet.AddParameter(49, "IMVIRONMENT");
				packet.AddParameter(1, mUsername);
				packet.AddParameter(14, "null");
				packet.AddParameter(13, "4");
				packet.AddParameter(5, username);
				packet.AddParameter(63, "doodle;107");
				packet.AddParameter(64, "1");

				sendPacket(packet);

				packet.Parameters[4] = "";
				packet.Parameters[13] = "0";
				packet.Parameters[64] = "0";

				sendPacket(packet);
			}
		}

		#region Utility Functions

		// Packet Utility Functions
		private YPacket generateStatusPacket(IMStatus status, string statusMessage)
		{
			YPacket returnMe = new YPacket();
			returnMe.Service = YahooServices.ymsg_status_update;

			switch (status)
			{
				case IMStatus.Available:
					returnMe.AddParameter(10, "99");
					break;
				case IMStatus.Away:
					returnMe.AddParameter(10, "1");
					break;
				case IMStatus.Idle:
					returnMe.AddParameter(10, "99");
					break;
				case IMStatus.Busy:
					returnMe.AddParameter(10, "2");
					break;
			}

			returnMe.AddParameter(19, statusMessage == null ? "" : statusMessage);
			returnMe.AddParameter(97, "1");

			return returnMe;
		}
		private void HandleStatusData(IMBuddy buddy, YPacket packet)
		{
			HandleStatusData(buddy, Convert.ToInt32(packet.Parameters[10]));
		}
		private void HandleStatusData(IMBuddy buddy, int stid)
		{
			switch (stid)
			{
				case 0:
					buddy.Status = IMBuddyStatus.Available;
					break;
				case 2:
					buddy.Status = IMBuddyStatus.Busy;
					break;
				case 99:
					buddy.Status = IMBuddyStatus.Away;
					break;
				case 999:
					buddy.Status = IMBuddyStatus.Idle;
					break;
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
			return yahoo64encode(dEncoding.GetBytes(data));
		}
		
		private void readDataAsync(IAsyncResult e)
		{
			int bytesRead;
			try	{
				bytesRead = nsStream.EndRead(e);
			} catch (InvalidOperationException f) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR, f.Message));
				return;
			}

			List<ArraySegment<byte>> blocks = new List<ArraySegment<byte>>();

			for (int i = 0; i <= bytesRead - 20;) // Don't increment i because we automatically do it down on the i += block.Length line
			{
				if (BitConverter.IsLittleEndian)
					Array.Reverse(dataqueue, i + 8, 2); // Int is backwards, reverse it
				short length = BitConverter.ToInt16(dataqueue, i + 8);
				ArraySegment<byte> block = new ArraySegment<byte>(dataqueue, i, 20 + length);
				blocks.Add(block);
				i += 20 + length;

				if (bytesRead - i < 20 && bytesRead == dataqueue.Length && nsStream.DataAvailable)
				{

				}
			}
		
			int len = blocks.Count;
			foreach (ArraySegment<byte> block in blocks)
			{			
				YPacket packet = YPacket.FromPacket(dataqueue, block.Offset, block.Count);

				if (packet.Parameters.Count == 0)
					continue;

				if (packet.StatusByte == new byte[] { 0xff, 0xff, 0xff, 0xff })
				{
					triggerOnDisconnect(new IMDisconnectEventArgs(DisconnectReason.Unknown));
					CleanupBuddyList();
					return;
				}

				if (packet.Service == YahooServices.ymsg_list_v15)
					HandleListv15Packet(packet);
				else if (packet.Service == YahooServices.ymsg_status_v15) // Status
					HandleStatusPacket(packet);
				else if (packet.Service == YahooServices.ymsg_message) // Instant Message
					HandleMessagePacket(packet);
				else if (packet.Service == YahooServices.ymsg_notify)
					HandleNotifyPacket(packet);
				else if (packet.Service == YahooServices.ymsg_pager_logoff)
					HandlePagerLogoffPacket(packet);
				else if (packet.Service == YahooServices.ymsg_newmail)
					// 9 : Number of emails?
					HandleNewMessagePacket(packet);
				else if (packet.Service == YahooServices.ymsg_sms_message)
					HandleSMSMessagePacket(packet);
				else if (packet.Service == YahooServices.ymsg_buddy_auth)
					// 4 : From
					// 5 : Us
					// 14 : Request Message
					// 216 : From's First Name
					// 254 : From's Last Name
					HandleBuddyAuthRequest(packet);
				else if (packet.Service == YahooServices.ymsg_status_update)
					HandleStatusUpdatePacket(packet);
				else if (packet.Service == YahooServices.ymsg_conference_invitation)
					HandleConferenceInvitePacket(packet);
				else if (packet.Service == YahooServices.ymsg_picture)
					HandlePicturePacket(packet);
			}

			if (status == IMProtocolStatus.Connecting)
			{
				status = IMProtocolStatus.Online;
				Connected = true;
				triggerOnLogin(null);
				mLoginWaitHandle.Set();
			}

			try {
				nsStream.BeginRead(dataqueue, 0, dataqueue.Length, new AsyncCallback(readDataAsync), null);
			} catch (Exception) {
				//Login();
			}
		}

		#endregion

		#region Packet Handlers

		private static ComplexChatMessage ParseMessage(string rawData)
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			IMRun chatRun = new IMRun();

			for (int i = 0; i < rawData.Length; i++)
			{
				if (rawData[i] == '<') // Check to see if we've found a possible font declaration
				{
					if (i + 5 < rawData.Length && rawData.Substring(i, 5) == "<font") // Now test to see if it's part of a font declaration
					{
						int endMark = rawData.IndexOf('>', i); // Get the end of the font declaration

						if (endMark != -1) // Make sure we have an end
						{
							int facePosition = rawData.IndexOf("face=", i, endMark - i);

							if (facePosition != -1)
							{
								facePosition += 6;

								if (facePosition >= endMark) // Malformed test
									continue;

								string fontName = rawData.Substring(facePosition, rawData.IndexOf('"', facePosition, endMark - facePosition) - facePosition);

								if (String.IsNullOrEmpty(chatRun.Body))
									chatRun.FontFamily = fontName;
								else {
									msg.Inlines.Add(chatRun);
									chatRun = new IMRun();
									chatRun.FontFamily = fontName;
								}
							}

							int sizePosition = rawData.IndexOf("size=", i, endMark - i);
							if (sizePosition != -1)
							{
								sizePosition += 6;

								if (sizePosition >= endMark) // Malformed test
									continue;

								string strFontSize = rawData.Substring(sizePosition, rawData.IndexOf('"', sizePosition, endMark - sizePosition) - sizePosition);
								int fontSize;

								if (Int32.TryParse(strFontSize, out fontSize))
								{
									if (String.IsNullOrEmpty(chatRun.Body))
										chatRun.FontSize = fontSize;
									else {
										msg.Inlines.Add(chatRun);
										chatRun = new IMRun();
										chatRun.FontSize = fontSize;
									}
								}
							}
							
							i = endMark + 1;
						}
					}
				}

				chatRun.Body += rawData[i];
			}

			msg.Inlines.Add(chatRun);

			return msg;
		}
		private void HandleConferenceInvitePacket(YPacket packet)
		{
			triggerOnChatRoomInvite(new IMRoomInviteEventArgs(packet.Parameters[50], packet.Parameters[50], packet.Parameters[58]));
		}
		private void HandleStatusUpdatePacket(YPacket packet)
		{
			HandleStatusData(IMBuddy.FromUsername(packet.Parameters[7], this), packet);
		}
		private void HandleSMSMessagePacket(YPacket packet)
		{
			IMBuddy buddy = IMBuddy.FromUsername("+" + packet.Parameters[4], this);
			var item = (from CarrierInfo i in mCarriers let y = packet.Parameters[68] where i.carrierid == y select new { i.maxchars, i.humanname }).FirstOrDefault();
			buddy.MaxMessageLength = item.maxchars;
			buddy.InvokeReceiveMessage(packet.Parameters[68]);
			if (!buddy.Options.ContainsKey("smscarrier"))
			{
				buddy.Options.Add("smscarrier", item.humanname);
				buddy.Options.Add("smscarriername", packet.Parameters[68]);
			}
		}
		private void HandlePagerLogoffPacket(YPacket packet)
		{
			if (packet.Status == Encoding.UTF8.GetString(new byte[] { 0xff, 0xff, 0xff, 0xff }, 0, 4)) // We got disconnected
			{
				if (packet.Parameters[66] == "42")
					triggerOnDisconnect(new IMDisconnectEventArgs(DisconnectReason.OtherClient));
				else
					triggerOnDisconnect(new IMDisconnectEventArgs(DisconnectReason.Unknown));
				status = IMProtocolStatus.Offline;
				CleanupBuddyList();
				Disconnect();
			} else {
				IMBuddy buddy = IMBuddy.FromUsername(packet.Parameters[7], this);
				buddy.Status = IMBuddyStatus.Offline;
				triggerContactStatusChange(new IMFriendEventArgs(buddy));
			}
		}
		private void HandleListv15Packet(YPacket packet)
		{
			status = IMProtocolStatus.Connecting; // Make sure we are still in a connecting state to prevent triggering the notification

			string currentgroup = "";
			IMBuddy buddy = null;

			foreach (KeyValuePair<int, string> parameter in packet.Parameters)
			{
				if (parameter.Key == 65)
				{
					currentgroup = parameter.Value;
				} else if (parameter.Key == 7) {
					buddy = new IMBuddy(this, parameter.Value);
					buddy.Group = currentgroup;
					buddy.mVisibilityStatus = UserVisibilityStatus.Online;
					buddylist.Add(buddy);
				} else if (parameter.Key == 317) {
					if (parameter.Key == 2)
					{
						buddy.mVisibilityStatus = UserVisibilityStatus.Permanently_Offline;
					}
				} else if (parameter.Key == 223) {
					buddy.StatusMessage = "Add request pending";
				}
				// A Parameter 317 = 2 Means you show up as invisible to that person
				// A Parameter 442 Means you are a yahoo power user.
				// A Parameter 223 = 1 Means you are currently waiting for authorization
			}
		}
		private void HandleStatusPacket(YPacket inpacket)
		{
			if (!yaddrbookdld)
				YAddrBookDownload();

			IMBuddy contact = null;

			foreach (KeyValuePair<int, string> packet in inpacket.Parameters)
			{
				if (packet.Key == 7)
					contact = (IMBuddy)ContactList[packet.Value];
				else if (packet.Key == 10)
					HandleStatusData(contact, Convert.ToInt32(inpacket.Parameters[10]));
				else if (packet.Key == 197)
					contact.Avatar = BuddyAvatar.FromUrl("http://img.avatars.yahoo.com/users/" + packet.Value + ".medium.png", packet.Value);
				else if (packet.Key == 192)	{
					YPacket picpack = new YPacket();
					picpack.Service = YahooServices.ymsg_picture;
					picpack.AddParameter(1, mUsername);
					picpack.AddParameter(5, packet.Value);
					picpack.AddParameter(13, "1");
					sendPacket(picpack);
				}
			}
		}
		private void HandleMessagePacket(YPacket packet)
		{
			IContact buddy = null;
			try {
				buddy = ContactList[packet.Parameters[4]];
			} catch (Exception) {
				buddy = new IMBuddy(this, packet.Parameters[4]);
			}

			// Apply some transforms to the text first
			string newmsg = packet.Parameters[14];
			ComplexChatMessage msg = ParseMessage(newmsg);
				
			//buddy.InvokeReceiveMessage(newmsg);
			//buddy.ShowIsTypingMessage(false);
			triggerOnMessageReceive(new IMMessageEventArgs(buddy, newmsg) { ComplexMessage = msg });

			// Tell the Yahoo servers we got this message
			if (packet.Parameters.ContainsKey(429))
			{
				YPacket resppkt = new YPacket();
				resppkt.Service = YahooServices.ymsg_message_reply;
				resppkt.AddParameter(1, mUsername);
				resppkt.AddParameter(5, packet.Parameters[4]);
				resppkt.AddParameter(302, "430");
				resppkt.AddParameter(430, packet.Parameters[429]);
				resppkt.AddParameter(303, "430");
				resppkt.AddParameter(450, "0");

				sendPacket(resppkt);
			}
		}
		private void HandleNotifyPacket(YPacket packet)
		{
			IMBuddy buddy = null;
			try {
				buddy = IMBuddy.FromUsername(packet.Parameters[4], this);
			} catch (Exception) {
				buddy = new IMBuddy(this, packet.Parameters[4]);
				buddy.IsOnBuddyList = false;
			}
			if (packet.Parameters[49] == "TYPING")
			{
				if (packet.Parameters[13] == "1")
					buddy.ShowIsTypingMessage(true);
				else
					buddy.ShowIsTypingMessage(false);
			}
		}
		private void HandlePicturePacket(YPacket packet)
		{
			if (packet.Parameters.ContainsKey(13) && packet.Parameters[13] == "2" && packet.Parameters.ContainsKey(192))
			{
				IMBuddy buddy = IMBuddy.FromUsername(packet.Parameters[4], this);
				buddy.Avatar = BuddyAvatar.FromUrl(packet.Parameters[20], packet.Parameters[192]);
			}
		}
		/// <summary>
		/// BuddyAuthRequest is triggered whenever an add buddy request is received or a response is received
		/// </summary>
		private void HandleBuddyAuthRequest(YPacket packet)
		{
			if (packet.Parameters.ContainsKey(13)) { // Key 13 is a response to a buddy add request
				if (packet.Parameters[13] == "1")
				{
					IMBuddy buddy = IMBuddy.FromUsername(packet.Parameters[4], this);
					buddy.StatusMessage = "";
				}
				addbuddygroups.Remove(packet.Parameters[4]);
			} else {
				if (packet.Parameters.ContainsKey(14))
					triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameters[4], packet.Parameters[14], packet.Parameters[216] + " " + packet.Parameters[254]));
				else {
					if (packet.Parameters.ContainsKey(216))
						triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameters[4], packet.Parameters[216] + " " + packet.Parameters[254]));
					else
						triggerOnFriendRequest(this, new IMFriendRequestEventArgs(packet.Parameters[4], ""));
				}
			}
		}
		/// <summary>
		/// This is triggered whenever you receive an email in your yahoo accounts
		/// </summary>
		private void HandleNewMessagePacket(YPacket packet)
		{
			if (packet.Parameters[9] != "0") // Are there any emails
			{
				if (packet.Parameters.ContainsKey(42))
				{
					triggerOnEmailReceive(this, new IMEmailEventArgs(packet.Parameters[42], packet.Parameters[43], packet.Parameters[18]));
				} else {
					//Notifications.ShowNotification("You have mail : ");
				}
			}
		}

		#endregion

		#region Connection/Authentication Methods
		
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
		/// <summary>
		/// Step 1.
		/// Begins authentication process of connecting to the yahoo servers
		/// by getting the ip address to use to connect to.
		/// </summary>
		private void beginAuthenticate()
		{
			logincookies.Clear(); // Just in-case this is the second time we are signing in

			WebRequest request = WebRequest.Create("http://vcs.msg.yahoo.com/capacity");
			//request.ServicePoint.Expect100Continue = false; // Yahoo doesn't like these

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

			nsStream.BeginRead(dataqueue, 0, dataqueue.Length, new AsyncCallback(SendInitAuthPkt), null);
		}
		private void SendInitAuthPkt(IAsyncResult e)
		{
			int bytesread = nsStream.EndRead(e);
			// Send Authentication Packet
			YPacket authpkt = new YPacket();
			authpkt.Service = YahooServices.ymsg_authentication;
			authpkt.AddParameter(1, mUsername);

			sendPacket(authpkt);

			nsStream.BeginRead(dataqueue, 0, dataqueue.Length, new AsyncCallback(OnGetAuthRespPacket), null);

		}
		private void OnGetAuthRespPacket(IAsyncResult e)
		{
			int bytesRead = nsStream.EndRead(e);

			YPacket p = YPacket.FromPacket(dataqueue, 0, bytesRead);
			sessionByte = p.SessionByte;
			string challenge = p.Parameters[94];

			YPacket authpkt = new YPacket();
			authpkt.ServiceByte = YahooServices2.ymsg_authentication_response;
			if (mStatus == IMStatus.Invisible)
				authpkt.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x0C }; // <-- Sign is as invisible
			else
				authpkt.StatusByte = new byte[] { 0x5A, 0x55, 0xAA, 0x55 };
			authpkt.AddParameter(1, mUsername);
			authpkt.AddParameter(0, mUsername);
			authpkt.AddParameter(277, authtoken);
			authpkt.AddParameter(278, authtoken2);
			authpkt.AddParameter(307, generateAuthHash(crumb, challenge));
			authpkt.AddParameter(244, "8388543"); // This *might* be a features list
			authpkt.AddParameter(2, mUsername);
			byte[] unknownbyte = new byte[] { 0x42, 0x09, 0x30, 0x6c, 0x37, 0x64, 0x61, 0x32, 0x74, 0x35, 0x34, 0x37, 0x63, 0x75, 0x32, 0x26, 0x62, 0x3d, 0x33, 0x26, 0x73, 0x3d, 0x38, 0x6e };
			authpkt.AddParameter(59, dEncoding.GetString(unknownbyte, 0, unknownbyte.Length));
			authpkt.AddParameter(98, "us");
			authpkt.AddParameter(135, "11.0.0.1751"); // Version Info

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
		private void OnGetYIPAddress(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			string streamBuf;

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

			connectServer = IPAddress.Parse(streamBuf.Substring(start, end));
			Trace.WriteLine("Yahoo: YMSG Communication Server: " + connectServer);

			if (String.IsNullOrEmpty(token))
				mConfig.TryGetValue("token", out token);

			if (String.IsNullOrEmpty(token))
			{
				Trace.WriteLine("Yahoo: Authentication token is invalid, requesting new token");
				WebRequest rqst = WebRequest.Create(string.Format(mAuthTokenGetUrl, Username, Password));
				rqst.BeginGetResponse(new AsyncCallback(OnGetYToken), rqst);
			} else {
				Trace.WriteLine("Yahoo: Authentication token valid, continuing");
				DoGetYCookies();
			}			
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
		private void OnGetYToken(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string streamBuf = reader.ReadToEnd();

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

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://login.yahoo.com/config/pwtoken_login?src=ymsgr&token=" + token);
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

			client.Connect(connectServer, 5050);
			nsStream = client.GetStream();

			OnYServerConnect();
		}

		#endregion			

		#region Y! Address Book Methods
		
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
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mAddressBookUrl);
			request.CookieContainer = new CookieContainer();
			request.CookieContainer.Add(new Uri("http://yahoo.com"), new Cookie("Y", logincookies["Y"], "/", ".yahoo.com"));
			request.CookieContainer.Add(new Uri("http://yahoo.com"), new Cookie("T", logincookies["T"], "/", ".yahoo.com"));
			request.BeginGetResponse(new AsyncCallback(OnAfterYAddressBookDl), request);
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

		#endregion

		private void sendKeepAlive(object state)
		{
			YPacket keepalive = new YPacket();
			keepalive.Service = YahooServices.ymsg_keepalive;
			keepalive.SessionByte = sessionByte;
			keepalive.AddParameter(0, mUsername);

			sendPacket(keepalive);
		}
		private void sendInvisibleCheckPacket(string destination)
		{
			// http://opi.yahoo.com/online?u={username}&m=t&t=1
			YPacket packet = new YPacket();
			packet.ServiceByte = new byte[] { 0x00, 0xc1 };
			packet.SessionByte = sessionByte;
			packet.AddParameter(1, mUsername);
			packet.AddParameter(5, destination);
			packet.AddParameter(206, "1");

			sendPacket(packet);
		}
		private void sendPacket(YPacket packet)
		{
			if (client == null)
				throw new NullReferenceException("Client is null");

			if (sessionByte == null)
				packet.SessionByte = sessionByte;

			byte[] packetdata = packet.ToBytes();

			try	{
				nsStream.Write(packetdata, 0, packetdata.Length);
			} catch (IOException) {
				lock (queuedpackets)
				{
					//queuedpackets.Add(packet);
				}
				
				authenticated = false;
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR));
			}
		}
		private void receivePacket(AsyncCallback callback)
		{
			if (callback != null)
				nsStream.BeginRead(dataqueue, 0, dataqueue.Length, callback, null);
		}

		// Protocol Specific Variables
		private byte[] dataqueue = new byte[2048];
		private byte[] sessionByte;

		// Socket Information
		private TcpClient client;
		private NetworkStream nsStream;
		private IPAddress connectServer;

		// State Information
		private bool yaddrbookdld = false;
		private bool authenticated = false;

		// Authentication Information
		private string token;
		private string authtoken;
		private string authtoken2;
		private string crumb;
		
		// SMS Carrier Information
		private List<CarrierInfo> mCarriers = new List<CarrierInfo>();
		private bool mCompletedCarrierSetup = false;
		
		private Dictionary<string, string> addbuddygroups = new Dictionary<string, string>(); // Remembers what group the buddy goes into
		private Dictionary<string, string> logincookies = new Dictionary<string, string>();
		private List<YPacket> queuedpackets = new List<YPacket>();
		private const string mSMSrequest = "http://insider.msg.yahoo.com/ycontent/?&sms={crc}&intl=us&os=win&ver=10.0.0.1102";
		private const string mAddressBookUrl = "http://address.yahoo.com/yap/us?v=XM&prog=ymsgr&useutf8=1&legenc=codepage-1252";
		private const string mAuthTokenGetUrl = "https://login.yahoo.com/config/pwtoken_get?src=ymsgr&login={0}&passwd={1}";
		private static Encoding dEncoding = Encoding.ASCII;
		
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