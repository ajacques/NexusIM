using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Security.Cryptography;
using System.Web;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.ComponentModel;

namespace InstantMessage
{
	public class IMYahooProtocol : IMProtocol
	{
		public IMYahooProtocol()
		{
			protocolType = "Yahoo";
			mProtocolTypeShort = "yahoo";
			supportsBuzz = true;
			supportsMUC = true;
			supportsIntroMsg = true;
			supportsUserInvisiblity = true;
		}
		public override void Login()
		{
			base.Login();
			ServicePointManager.Expect100Continue = false;

			// Do this the proper way.. thread it so the entire app doesn't lag when loading because of us
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://vcs.msg.yahoo.com/capacity");
			request.BeginGetResponse(new AsyncCallback(this.beginAuthenticate), request);
			mEnabled = true;
			status = IMProtocolStatus.ONLINE;
		}
		public override void Disconnect()
		{
			CleanupBuddyList();

			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_pager_logoff;
			packet.Session = session;

			if (authenticated)
				sendPacket(packet);
			
			socket.Close();
		}
		public override void SendMessage(string friendName, string message)
		{
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
			pkt.AddParameter("14", Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(newmessage)));
			//pkt.AddParameter("429", "0000000" + converstationCount[friendName] + "c35AF042C"); // <-- Not sure what this does, but it seems to have to do with how many messages I've sent.. Maybe a delivery confirmation?
			pkt.AddParameter("450", "0");

			sendPacket(pkt);
		}
		public override void SendMessage(IMBuddy buddy, string message, bool usesms)
		{
			YPacket pkt = new YPacket();

			string newmessage = message;

			if (message.Length > 800)
			{
				newmessage = message.Substring(0, 800);
				SendMessage(buddy, message.Substring(801), usesms);
			}

			if (usesms || buddy.IsMobileContact)
			{
				pkt.Service = YahooServices.ymsg_sms_message;
				pkt.Session = session;
				pkt.AddParameter("1", mUsername);
				pkt.AddParameter("5", buddy.SMSNumber.Replace("+", ""));
				pkt.AddParameter("14", message);
				pkt.AddParameter("68", buddy.Options["smscarrier"]);
			} else {
				pkt.Service = YahooServices.ymsg_message;
				pkt.Session = session;
				pkt.AddParameter("1", mUsername);
				pkt.AddParameter("5", buddy.Username);
				pkt.AddParameter("97", "1");
				pkt.AddParameter("63", ";0");
				pkt.AddParameter("64", "0");
				pkt.AddParameter("206", "0");
				pkt.AddParameter("14", message);
				pkt.AddParameter("450", "0");
			}

			sendPacket(pkt);
		}
		public override void IsTyping(string uname, bool isTyping)
		{
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_notify;
			packet.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x16 };
			packet.AddParameter("49", "TYPING");
			packet.AddParameter("1", mUsername);
			packet.AddParameter("14", Encoding.Default.GetString(new byte[] { 20 }));
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
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (newstatus == mStatus)
				return;

			if (!mEnabled)
				return;

			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus))
			{
				mStatus = newstatus;
				Login();
				return;
			} else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus)) {
				Disconnect();
			}

			if (IsOnlineStatus(newstatus))
				mEnabled = true;
			else
				mEnabled = false;

			if (IsOnlineStatusToOthers(mStatus) && newstatus == IMStatus.INVISIBLE)
			{
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.Session = session;
				p1.AddParameter("13", "2");

				sendPacket(p1);
			} else if (!IsOnlineStatusToOthers(mStatus) && newstatus != IMStatus.INVISIBLE) {
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_visibility_toggle;
				p1.Session = session;
				p1.AddParameter("13", "1");

				sendPacket(p1);
			}

			if (newstatus == IMStatus.AVAILABLE && status == IMProtocolStatus.ONLINE)
			{
				if (!(mIsIdle && mStatus == IMStatus.INVISIBLE))
				{
					YPacket p1 = new YPacket();
					p1.Service = YahooServices.ymsg_status_update;
					p1.Session = session;
					p1.AddParameter("10", "0");
					p1.AddParameter("19", String.Empty);
					p1.AddParameter("97", "1");

					sendPacket(p1);
				} else {
					mIsIdle = false;
				}
			} else if (newstatus == IMStatus.BUSY) {
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_status_update;
				p1.Session = session;
				p1.AddParameter("10", "2");
				p1.AddParameter("19", "");
				p1.AddParameter("97", "1");

				sendPacket(p1);
			}
			else if (newstatus == IMStatus.AWAY)
			{
				YPacket p1 = new YPacket();
				p1.Service = YahooServices.ymsg_status_update;
				p1.Session = session;
				p1.AddParameter("10", "1");
				p1.AddParameter("19", "");
				p1.AddParameter("97", "1");

				sendPacket(p1);
			} else if (newstatus == IMStatus.IDLE && status == IMProtocolStatus.ONLINE) {
				if (mStatus != IMStatus.INVISIBLE)
				{
					YPacket p1 = new YPacket();
					p1.Service = YahooServices.ymsg_status_update;
					p1.AddParameter("10", "999");
					p1.AddParameter("19", "");
					p1.AddParameter("97", "1");
					p1.AddParameter("47", "2");

					sendPacket(p1);
				} else {
					mIsIdle = true;
				}
			}

			mStatus = newstatus;
		}
		public override bool IsOnlineStatusToOthers(IMStatus status)
		{
			if (status == IMStatus.INVISIBLE || status == IMStatus.OFFLINE)
				return false;
			else
				return true;
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
				budd.b.ContactItemVisible = false;
				buddylist.Remove(budd.b);
			}

			IMBuddy buddy = new IMBuddy(this, name);
			buddy.StatusMessage = "Add request pending";
			buddy.Status = IMBuddyStatus.OFFLINE;
			buddy.Group = group;
			buddy.Populate();
			buddylist.Add(buddy);

			if (!addbuddygroups.ContainsKey(name))
				addbuddygroups.Add(name, group);
			else
				addbuddygroups[name] = group;
		}
		public override void RemoveFriend(string uname, string group)
		{
			IMBuddy buddy = AccountManager.GetByName(uname, this);
			YPacket packet = new YPacket();
			packet.Service = YahooServices.ymsg_remove_buddy;
			packet.Session = session;
			packet.AddParameter("1", mUsername);
			packet.AddParameter("7", uname);
			packet.AddParameter("65", group);

			sendPacket(packet);

			buddy.ContactItemVisible = false;
			buddylist.Remove(buddy);

			if (false)
			{
				XmlDocument xml = new XmlDocument();
				xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", ""));
				XmlElement ab = xml.CreateElement("ab");
				XmlElement ct = xml.CreateElement("ct");
				ab.SetAttribute("k", mUsername);
				ab.SetAttribute("cc", "1");
				ct.SetAttribute("d", "1");
				ct.SetAttribute("yi", uname);
				ct.SetAttribute("pr", "0");
				ct.SetAttribute("id", buddy.Options["yid"]);
				ab.AppendChild(ct);
				xml.AppendChild(ab);

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://address.yahoo.com/yab/us?v=XM&prog=ymsgr&.intl=us&sync=1&tags=short&noclear=1&useutf8=1&legenc=codepage-1252");
				request.CookieContainer = logincookies;
				request.Method = "POST";
				request.ContentLength = xml.OuterXml.Length;
				Stream poststream = request.GetRequestStream();
				StreamWriter postwriter = new StreamWriter(poststream);
				postwriter.Write(xml.OuterXml);
				string streamBuf = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();
			}
		}
		public override void SetPerUserVisibilityStatus(string buddy, UserVisibilityStatus status)
		{
			if (status == UserVisibilityStatus.PERMANENTLY_OFFLINE)
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

				socket.Send(packet.ToBytes());
			} else if (status == UserVisibilityStatus.ONLINE) {
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

				socket.Send(packet.ToBytes());

				packet.Service = YahooServices.ymsg_stealth_session;
				packet.Parameter["31"] = "1";

				socket.Send(packet.ToBytes());

				YPacket spacket = new YPacket();
				spacket.Service = YahooServices.ymsg_status_update;
				spacket.AddParameter("10", "0");
				spacket.AddParameter("19", "");
				spacket.AddParameter("97", "1");

				socket.Send(spacket.ToBytes());
			}
		}
		public override void JoinChatRoom(string room)
		{
			if (roominvites.ContainsKey(room))
			{

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
					buddy.First().b.showWindow(true);
				} else {
					IMBuddy nbuddy = new IMBuddy(this, user);
					frmChatWindow win = new frmChatWindow();
					win.Buddy = nbuddy;
					win.Show();
				}
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
			return yahoo64encode(Encoding.Default.GetBytes(data));
		}
		/// <summary>
		/// Generates the Authentication hash used in the login process
		/// </summary>
		/// <param name="crumb">Crumb key from the web login page</param>
		/// <param name="challenge">Challenge key sent from the yahoo messenger servers.</param>
		/// <returns>String to be sent in the authentication response packet</returns>
		private string generateAuthHash(string crumb, string challenge)
		{
			MD5CryptoServiceProvider md5SP = new MD5CryptoServiceProvider();
			byte[] bs = Encoding.UTF8.GetBytes(crumb + challenge);
			bs = md5SP.ComputeHash(bs);
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
			string recvdata = Encoding.Default.GetString(dataqueue);
			string[] sections = recvdata.Split(new string[] { "YMSG" }, StringSplitOptions.None);

			for (int c = 1; c < sections.Length; c++)
			{
				YPacket packet = YPacket.FromPacket("YMSG" + sections[c]);

				if (packet.Parameter.Count == 0)
					continue;

				string[] parameters = sections[c].Split(new string[] { "À€" }, StringSplitOptions.None);
				int serviceId = Convert.ToInt32(sections[c][7]);

				// ymsg_list
				// 442 : 1 is the power user crown 3 is the trophy

				if (packet.Status == Encoding.Default.GetString(new byte[] { 0xff, 0xff, 0xff, 0xff }))
				{
					triggerOnDisconnect(this, new IMDisconnectEventArgs(IMDisconnectEventArgs.DisconnectReason.Unknown));
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
				} else if (packet.Service == YahooServices.ymsg_picture) {
					// 4 : From
					// 5 : Us
					// 20 : Image URL
					// 192 : Image Hash?
					HandlePicturePacket(packet);
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
				} else if (packet.Service == YahooServices.ymsg_conference_logon) {
					HandleConferenceLogonPacket(packet);
				}
			}

			dataqueue = new byte[1024];
			try {
				socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(this.readDataAsync), this);
			} catch (Exception ex) {
				status = IMProtocolStatus.ERROR;
				//Login();
			}
		}

		private void HandleConferenceLogonPacket(YPacket packet)
		{
			if (joinedrooms.ContainsKey(packet.Parameter["57"]))
			{
				ChatRoomContainer room = joinedrooms[packet.Parameter["57"]];
				room.Occupants.Add(packet.Parameter["53"]);
				room.ChatWindow.UpdateRoom();
			}
		}
		private void HandleWebcamPacket(YPacket packet)
		{

		}
		private void HandleStatusData(IMBuddy buddy, YPacket packet)
		{
			if (packet.Parameter["10"] == "2") // Busy
			{
				buddy.Status = IMBuddyStatus.BUSY;
			} else if (packet.Parameter["10"] == "999") { // Idle
				buddy.Status = IMBuddyStatus.IDLE;
			} else if (packet.Parameter["10"] == "0") { // Available
				buddy.Status = IMBuddyStatus.ONLINE;
			} else if (packet.Parameter["10"] == "99") {
				buddy.Status = IMBuddyStatus.AFK;
				buddy.StatusMessage = packet.Parameter["19"];
			}

			if (packet.Parameter.ContainsKey("19"))
			{
				buddy.StatusMessage = packet.Parameter["19"];
			} else {
				buddy.StatusMessage = "";
			}
		}
		private void HandleConferenceInvitePacket(YPacket packet)
		{
			triggerOnChatRoomInvite(new IMRoomInviteEventArgs(packet.Parameter["50"], packet.Parameter["50"], packet.Parameter["58"]));
		}
		private void HandleStatusUpdatePacket(YPacket packet)
		{
			HandleStatusData(AccountManager.GetByName(packet.Parameter["7"], this), packet);
		}
		private void HandleSMSMessagePacket(YPacket packet)
		{
			IMBuddy buddy = AccountManager.GetByName("+" + packet.Parameter["4"]);
			var item = (from CarrierInfo i in carriers let y = packet.Parameter["68"] where i.carrierid == y select new { i.maxchars, i.humanname }).FirstOrDefault();
			buddy.MaxMessageLength = item.maxchars;
			buddy.ShowRecvMessage(packet.Parameter["68"]);
			if (!buddy.Options.ContainsKey("smscarrier"))
			{
				buddy.Options.Add("smscarrier", item.humanname);
				buddy.Options.Add("smscarriername", packet.Parameter["68"]);
			}
		}
		private void HandlePagerLogoffPacket(YPacket packet)
		{
			if (packet.Status == Encoding.Default.GetString(new byte[] { 0xff, 0xff, 0xff, 0xff })) // We got disconnected
			{
				if (packet.Parameter["66"] == "42")
					triggerOnDisconnect(this, new IMDisconnectEventArgs(IMDisconnectEventArgs.DisconnectReason.OtherClient));
				else
					triggerOnDisconnect(this, new IMDisconnectEventArgs(IMDisconnectEventArgs.DisconnectReason.Unknown));
				status = IMProtocolStatus.OFFLINE;
				CleanupBuddyList();
				Disconnect();
			} else {
				IMBuddy buddy = AccountManager.GetByName(packet.Parameter["7"], this);
				buddy.Online = false;
				buddy.Status = IMBuddyStatus.OFFLINE;
				triggerOnFriendSignOut(new IMFriendEventArgs(buddy));
			}
		}
		private void HandleListv15Packet(ref string[] parameters)
		{
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
					try	{
						buddy.BuddyImage = Image.FromFile(Path.GetTempPath() + Settings.GetContactSetting(buddy, "buddyimage", "default.png"));
					} catch (Exception)	{
						if (Settings.GetContactSetting(buddy, "buddyimage", "default.png") != "default.png")
						{
							Settings.DeleteContactSetting(buddy, "buddyimagehash");
						}
					}
					buddy.Populate();
					buddy.Options.Add("InvisiStatus", "Online");
					buddylist.Add(buddy);
				} else if (parameters[i] == "317") {
					if (parameters[i + 1] == "2")
					{
						buddy.Options["InvisiStatus"] = "PermInvisi";
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
				if (parameters[i] == "7")
				{
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
					buddy = AccountManager.GetByName(packet.Parameter["7"], this);
					buddy.Online = true;
					buddy.Status = IMBuddyStatus.ONLINE;
					triggerOnFriendSignIn(new IMFriendEventArgs(buddy));
					buddy.UpdateListItem();
					HandleStatusData(buddy, packet);
				}
				if (packet.Parameter.ContainsKey("192") && !packet.Parameter.ContainsKey("197"))
				{
					if (Settings.GetContactSetting(buddy, "buddyimagehash", "0") != packet.Parameter["192"])
					{
						YPacket picpack = new YPacket();
						picpack.Service = YahooServices.ymsg_picture;
						picpack.Session = session;
						picpack.AddParameter("1", mUsername);
						picpack.AddParameter("5", packet.Parameter["7"]);
						picpack.AddParameter("13", "1");
						socket.Send(Encoding.Default.GetBytes(picpack.Packet));
					}
				}
				if (packet.Parameter.ContainsKey("197")) // Yahoo! Avatars code
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://img.avatars.yahoo.com/users/" + packet.Parameter["197"] + ".medium.png");
					request.BeginGetResponse(new AsyncCallback(this.onYAvatarMediumPngDownload), new PacketAsync(request, packet));
				}
				if (packet.Parameter.ContainsKey("442")) // Yahoo Power Users thing - Just an aesthetic thing
				{
					string powericon = "";

					if (packet.Parameter["442"] == "1")
					{
						powericon = "crown";
					}
					System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
					Stream file = thisExe.GetManifestResourceStream("NexusIM.Resources." + powericon + ".png");
					buddy.BuddyTagImage = Image.FromStream(file);
				}
			}
		}
		private void HandleMessagePacket(YPacket packet)
		{
			IMBuddy buddy = null;
			try {
				buddy = AccountManager.GetByName(packet.Parameter["4"]);
			} catch (Exception) {
				buddy = new IMBuddy(this, packet.Parameter["4"]);
				buddy.IsOnBuddyList = false;
			}
			if (packet.Parameter["14"] == "<ding>")
				buddy.Buzz();
			else {
				// Apply some transforms to the text first
				RTF.RTFBuilder builder = new RTF.RTFBuilder();
				string msg = packet.Parameter["14"];
				MatchCollection matches = Regex.Matches(msg, "<((font face=\\\"([a-zA-Z ]*)\\\")|(fade (((#([a-z0-9]*)),?)*)))>", RegexOptions.IgnoreCase);
				for (int i = 0; i < matches.Count; i++)
				{
					string messagedata = "";
					if (i + 1 < matches.Count)
					{
						int startindex = matches[i].Index + matches[i].Length;
						messagedata = msg.Substring(startindex, matches[i + 1].Index - startindex);
					} else {
						messagedata = msg.Substring(matches[i].Index + matches[i].Length);
					}
					builder.Font(matches[i].Groups[3].Value).Append(messagedata);
				}

				// Use weird, messed up way to support yahoo UTF-8 messages
				buddy.ShowCustomMessage(new ChatMessage(buddy.Username, false, builder, DateTime.Now));
				buddy.ShowIsTypingMessage(false);
				triggerOnMessageReceive(this, new IMMessageEventArgs(buddy, mUsername, Encoding.UTF8.GetString(Encoding.Default.GetBytes(packet.Parameter["14"]))));
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

				socket.Send(resppkt.ToBytes());
			}
		}
		private void HandleNotifyPacket(YPacket packet)
		{
			IMBuddy buddy = null;
			try {
				buddy = AccountManager.GetByName(packet.Parameter["4"]);
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
			if (packet.Parameter["13"] == "2")
			{
				if (packet.Parameter["192"] == Settings.GetContactSetting(packet.Parameter["4"], this, "buddyimagehash", ""))
				{
					try {
						AccountManager.GetByName(packet.Parameter["4"], this).BuddyImage = Bitmap.FromFile(Settings.GetContactSetting(packet.Parameter["4"], this, "buddyimage", ""));
					} catch (Exception)	{
						Settings.DeleteContactSetting(packet.Parameter["4"], this, "buddyimagehash");
					}
				} else {
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(packet.Parameter["20"]);
					request.BeginGetResponse(new AsyncCallback(this.onBuddyImageDownload), new PacketAsync(request, packet));
					Settings.SetContactSetting(packet.Parameter["4"], this, "buddyimagehash", packet.Parameter["192"]);
				}
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
					IMBuddy buddy = AccountManager.GetByName(packet.Parameter["4"], this);
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
					Notifications.ShowNotification("You have mail : ");
				}
			}
		}

		/// <summary>
		/// Begins authentication process of connecting to the yahoo servers
		/// </summary>
		private void beginAuthenticate(IAsyncResult e)
		{
			ServicePointManager.Expect100Continue = false;
			string connectServer = "";
			{
				HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)e.AsyncState).GetResponse();
				Stream stream = response.GetResponseStream();

				StreamReader readStream = new StreamReader(stream);
				string streamBuf = readStream.ReadToEnd();

				int start = streamBuf.IndexOf("CS_IP_ADDRESS") + 14;
				int end = (streamBuf.Length - start) - 2;

				connectServer = streamBuf.Substring(start, end);
			}

			string token = Settings.GetAccountSetting(this, "token", "");

			if (token == "")
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://login.yahoo.com/config/pwtoken_get?src=ymsgr&login=" + mUsername + "&passwd=" + mPassword);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();

				// Download
				StreamReader readStream = new StreamReader(stream);
				string streamBuf = readStream.ReadToEnd();

				// Parsing
				int result = Convert.ToInt32(Regex.Match(streamBuf, @"^([0-9]*)").Groups[1].Value);
				if (result == 0)
				{
					Match key = Regex.Match(streamBuf, @"ymsgr=([\S]*)");
					token = key.Groups[1].Value.ToString();
					if (savepassword)
						Settings.SetAccountSetting(this, "token", token);
				} else if (result == 1212) { // Invalid Credentials
					triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_PASSWORD));
					return;
				}		
			}

			string authtoken = Settings.GetAccountSetting(this, "authtoken1", "");
			string authtoken2 = Settings.GetAccountSetting(this, "authtoken2", "");
			string crumb = Settings.GetAccountSetting(this, "authcrumb", "");
			int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

			// Check to see if we are missing any tokens or if the tokens have expired
			if (authtoken == "" || authtoken2 == "" || crumb == "" || epoch < Convert.ToInt32(Settings.GetAccountSetting(this, "tokenexpires", "0")))
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://login.yahoo.com/config/pwtoken_login?src=ymsgr&token=" + token);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();

				// Download
				StreamReader readStream = new StreamReader(stream);
				string streamBuf = readStream.ReadToEnd();

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
				} else if (result == 100) {
					HttpWebRequest loginrequest = (HttpWebRequest)WebRequest.Create("http://vcs.msg.yahoo.com/capacity");
					loginrequest.BeginGetResponse(new AsyncCallback(this.beginAuthenticate), request);
					return;
				}

				if (savepassword)
				{
					Settings.SetAccountSetting(this, "authtoken1", authtoken);
					Settings.SetAccountSetting(this, "authtoken2", authtoken2);
					Settings.SetAccountSetting(this, "authcrumb", crumb);
					Settings.SetAccountSetting(this, "tokenexpires", (epoch + 86400).ToString());
				}
			}

			logincookies.Add(new Cookie("Y", authtoken, "/", ".yahoo.com"));
			logincookies.Add(new Cookie("T", authtoken2, "/", ".yahoo.com"));

			authtoken += "; path=/; domain=.yahoo.com";
			authtoken2 += "; path=/; domain=.yahoo.com";

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			socket.Connect(connectServer, 5050);

			// Send Authentication Packet
			{
				YPacket authpkt = new YPacket();
				authpkt.Service = YahooServices.ymsg_authentication;
				authpkt.AddParameter("1", mUsername);
				socket.Send(Encoding.Default.GetBytes(authpkt.Packet));
			}

			string challenge = "";

			// Authentication Response Packet
			{
				byte[] authreturnbytes = new byte[126];
				socket.Receive(authreturnbytes);
				string authreturn = Encoding.Default.GetString(authreturnbytes);

				YPacket p = YPacket.FromPacket(authreturn);
				session = p.Session;
				challenge = p.Parameter["94"];
			}

			{
				YPacket authpkt = new YPacket();
				authpkt.Service = YahooServices.ymsg_authentication_response;
				if (mStatus == IMStatus.INVISIBLE)
					authpkt.StatusByte = new byte[] { 0x00, 0x00, 0x00, 0x0C }; // <-- Sign is as invisible
				authpkt.Session = session;
				authpkt.AddParameter("1", mUsername);
				authpkt.AddParameter("0", mUsername);
				authpkt.AddParameter("277", authtoken);
				authpkt.AddParameter("278", authtoken2);
				authpkt.AddParameter("307", generateAuthHash(crumb, challenge));
				authpkt.AddParameter("244", "4194239");
				authpkt.AddParameter("2", mUsername);
				//authpkt.AddParameter("2", "1");
				authpkt.AddParameter("59", Encoding.Default.GetString(new byte[] { 0x42, 0x09, 0x30, 0x6c, 0x37, 0x64, 0x61, 0x32, 0x74, 0x35, 0x34, 0x37, 0x63, 0x75, 0x32, 0x26, 0x62, 0x3d, 0x33, 0x26, 0x73, 0x3d, 0x38, 0x6e }));
				authpkt.AddParameter("98", "us");
				authpkt.AddParameter("135", "9.0.0.2162");

				socket.Send(Encoding.Default.GetBytes(authpkt.Packet + "À€2À€1À€"));
			}

			status = IMProtocolStatus.ONLINE;
			if (mStatus == IMStatus.OFFLINE)
				mStatus = IMStatus.AVAILABLE;

			socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(this.readDataAsync), this);

			Timer keepalive = new Timer();
			keepalive.Interval = (60 * 1000); // 60 Seconds
			keepalive.Tick += new EventHandler(sendKeepAlive);
			MethodInvoker invoker = new MethodInvoker(delegate() { keepalive.Start(); });
			frmMain.Instance.Invoke(invoker);
			authenticated = true;

			foreach (YPacket packet in queuedpackets)
			{
				sendPacket(packet);
			}
			queuedpackets.Clear();
		}

		// Callbacks
		/// <summary>
		/// Handles the download of buddy images from the yahoo servers
		/// </summary>
		private void onBuddyImageDownload(IAsyncResult e)
		{
			PacketAsync iStruct = (PacketAsync)e.AsyncState;
			Stream imgStream = iStruct.request.GetResponse().GetResponseStream();
			Image image = Bitmap.FromStream(imgStream);

			AccountManager.GetByName(iStruct.packet.Parameter["4"], this).BuddyImage = image;
			image.Save(Path.GetTempPath() + "\\" + iStruct.packet.Parameter["4"] + ".png");
			Settings.SetContactSetting(iStruct.packet.Parameter["4"], this, "buddyimage", iStruct.packet.Parameter["4"] + ".png");
		}
		/// <summary>
		/// Handles the download of Yahoo! Avatar avatars
		/// </summary>
		private void onYAvatarMediumPngDownload(IAsyncResult e)
		{
			PacketAsync iStruct = (PacketAsync)e.AsyncState;
			Stream imgStream = iStruct.request.GetResponse().GetResponseStream();
			Image image = Bitmap.FromStream(imgStream);

			AccountManager.GetByName(iStruct.packet.Parameter["7"], this).BuddyImage = image;
			image.Save(Path.GetTempPath() + "\\" + iStruct.packet.Parameter["7"] + ".png");
			Settings.SetContactSetting(iStruct.packet.Parameter["7"], this, "buddyimage", iStruct.packet.Parameter["4"] + ".png");
		}
		private void onYAddressBookDownload(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream book = response.GetResponseStream();
				string bxml = (new StreamReader(book)).ReadToEnd();
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(bxml);

				IEnumerator contacts = xml.DocumentElement.GetEnumerator();
				while (contacts.MoveNext())
				{
					XmlElement elem = (XmlElement)contacts.Current;

					IMBuddy buddy = null;
					try	{
						buddy = AccountManager.GetByName(elem.Attributes["yi"].Value, this);

					} catch (Exception)	{
						if (elem.HasAttribute("yi"))
						{
							buddy = new IMBuddy(this, elem.Attributes["yi"].Value);
							buddy.Group = "Address Book";
							buddy.Populate();
							buddylist.Add(buddy);
						}
						if (elem.HasAttribute("mo"))
						{
							buddy = new IMBuddy(this, elem.Attributes["mo"].Value);
							buddy.Group = "Mobile";
							buddy.IsMobileContact = true;
							buddy.Populate();
							buddylist.Add(buddy);
						}
					}
					if (elem.HasAttribute("nn")) // Nickname
					{
						buddy.Nickname = elem.Attributes["nn"].Value;
					} else {
						string nick = "";
						if (elem.HasAttribute("fn"))
						{
							nick = elem.Attributes["fn"].Value;
						}
						if (elem.HasAttribute("ln"))
						{
							if (nick != "")
							{
								nick += " ";
							}
							nick += elem.Attributes["ln"].Value;
						}
						buddy.Nickname = nick;
					}
					if (elem.HasAttribute("id") && !buddy.Options.ContainsKey("yid"))
					{
						buddy.Options.Add("yid", elem.Attributes["id"].Value);
					}
					if (elem.HasAttribute("mo"))
					{
						buddy.SMSNumber = elem.Attributes["mo"].Value;
						if (elem.HasAttribute("vm"))
						{
							var item = (from CarrierInfo i in carriers let y = elem.Attributes["vm"].Value where i.carrierid == y select new { i.maxchars, i.humanname }).FirstOrDefault();
							buddy.MaxMessageLength = item.maxchars;
							buddy.Options.Add("smscarrier", elem.Attributes["vm"].Value);
							buddy.Options.Add("smscarriername", item.humanname);
						} else {
							buddy.MaxMessageLength = 145;
						}
					}
					buddy.UpdateListItem();
					buddylist.Add(buddy);
				}
			} else {
				request = (HttpWebRequest)WebRequest.Create("http://address.yahoo.com/yap/us?v=XM&prog=ymsgr&prog-ver=9.0.0.2162&useutf8=1&legenc=codepage-1252");
				request.CookieContainer = logincookies;
				request.BeginGetResponse(new AsyncCallback(this.onYAddressBookDownload), request);
			}
		}
		private void onSMSCarrierDownload(IAsyncResult e)
		{
			Stream xmlstream = ((HttpWebRequest)e.AsyncState).GetResponse().GetResponseStream();
			string sxml = (new StreamReader(xmlstream)).ReadToEnd();

			// Write to temp
			FileStream temp = File.OpenWrite(Path.GetTempPath() + "\\yahooim_smsid_cache.xml");
			(new StreamWriter(temp)).Write(sxml);
			temp.Close();

			parseSMSCarrierXml(sxml);
		}
		private void parseSMSCarrierXml(string tcontents)
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(tcontents);

			XPathNavigator nav = xml.CreateNavigator();

			XPathNodeIterator carriersx = nav.Select("/content/feed[@name='sms']/sms/carrierslist/*");

			while (carriersx.MoveNext())
			{
				XPathNavigator carrier = (XPathNavigator)carriersx.Current;

				CarrierInfo info = new CarrierInfo();
				info.carrierid = carrier.GetAttribute("id", "");
				info.maxchars = Convert.ToInt32(carrier.GetAttribute("maxchars", ""));
				info.humanname = carrier.GetAttribute("name", "");

				carriers.Add(info);
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://address.yahoo.com/yap/us?v=XM&prog=ymsgr&prog-ver=9.0.0.2162&useutf8=1&legenc=codepage-1252");
			request.CookieContainer = logincookies;
			request.BeginGetResponse(new AsyncCallback(this.onYAddressBookDownload), request);
		}

		private void YAddrBookDownload()
		{
			if (File.Exists(Path.GetTempPath() + "\\yahooim_smsid_cache.xml"))
			{
			}
			HttpWebRequest smsids = (HttpWebRequest)WebRequest.Create("http://insider.msg.yahoo.com/ycontent/?&sms=0&fmt=2.0&intl=us&os=win&ver=9.0.0.2162");
			smsids.BeginGetResponse(new AsyncCallback(this.onSMSCarrierDownload), smsids);

			yaddrbookdld = true;
		}
		private void sendKeepAlive(object sender, EventArgs e)
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

			socket.Send(packet.ToBytes());
		}
		private void sendPacket(YPacket packet)
		{
			if (socket == null)
				return;

			packet.Session = session;

			try	{
				socket.Send(packet.ToBytes());
			} catch (Exception) {
				if (AccountManager.IsConnectedToInternet()) { // If this is true, then the network experienced a slight hiccup, or the network adapter we were using is nolonger available
					Login();
					queuedpackets.Add(packet);
				} else {
					triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR));
					status = IMProtocolStatus.ERROR;
					authenticated = false;
				}
			}
		}
		
		// Protocol Specific Variables
		private byte[] dataqueue = new byte[1024];
		private Dictionary<string, int> converstationCount = new Dictionary<string, int>();
		private string session = "";
		private Socket socket = null;
		private bool yaddrbookdld = false;
		private bool authenticated = false;
		private CookieContainer logincookies = new CookieContainer();
		private List<CarrierInfo> carriers = new List<CarrierInfo>();
		private Dictionary<string, string> roominvites = new Dictionary<string, string>();
		private Dictionary<string, ChatRoomContainer> joinedrooms = new Dictionary<string, ChatRoomContainer>();
		private Dictionary<string, string> addbuddygroups = new Dictionary<string, string>(); // Remembers what group the buddy goes into
		private List<YPacket> queuedpackets = new List<YPacket>();

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
			ymsg_p2p_file_transfer = 0x4d,
			ymsg_notify = 0x4b,
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
			ymsg_picture_update = 0xc1,
			ymsg_status_update = 0xc6,
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
				packet.ServiceByte = Encoding.Default.GetBytes(packetdata.Substring(10, 2));
				packet.StatusByte = Encoding.Default.GetBytes(packetdata.Substring(12, 4));
				packet.SessionByte = Encoding.Default.GetBytes(packetdata.Substring(16, 4));

				IEnumerator paramEnum = packetdata.Substring(20, packetdata.Length - 20).Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).GetEnumerator();
				while (paramEnum.MoveNext())
				{
					string param = (string)paramEnum.Current;
					if (paramEnum.MoveNext())
					{
						string value = (string)paramEnum.Current;
						packet.AddParameter(param, value);
					} else {
						break;
					}
				}

				return packet;
			}
			public static YPacket FromWebPacket(string packetdata)
			{
				YPacket packet = new YPacket();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(packetdata);
				byte[] test = new byte[] { 0, 0, 0, 0 };

				packet.Service = (YahooServices)Convert.ToInt32(doc.DocumentElement.GetAttribute("Command"));

				string[] paramList = doc.DocumentElement.InnerText.Split(new string[] { "^$" }, StringSplitOptions.RemoveEmptyEntries);

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
				return Encoding.Default.GetBytes(Packet);
			}

			public string Packet
			{
				get {
					pktdata = CalculateContents();
					byte[] lens = BitConverter.GetBytes(pktdata.Length);
					byte[] lens2 = new byte[] { lens[1], lens[0] };

					return "YMSG" + Encoding.Default.GetString(version) + nils + nils + Encoding.Default.GetString(lens2) + Encoding.Default.GetString(service) + Encoding.Default.GetString(status) + Encoding.Default.GetString(session) + pktdata;
				}
			}
			public string Contents
			{
				get
				{
					return CalculateContents();
				}
				set
				{
					pktdata = value;
				}
			}
			public string Session
			{
				get
				{
					return Encoding.Default.GetString(session);
				}
				set
				{
					session = Encoding.Default.GetBytes(value);
				}
			}
			public byte[] SessionByte
			{
				get
				{
					return session;
				}
				set
				{
					session = value;
				}
			}
			public string Status
			{
				get
				{
					return Encoding.Default.GetString(status);
				}
				set
				{
					status = Encoding.Default.GetBytes(value);
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
				get
				{
					return (YahooServices)service[1];
				}
				set
				{
					service[1] = (byte)value;
				}
			}
			public byte[] ServiceByte
			{
				get
				{
					return service;
				}
				set
				{
					service = value;
				}
			}
			public Dictionary<string, string> Parameter
			{
				get
				{
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
					returnVal += current.Key + separator + current.Value + separator;
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
			private byte[] version = new byte[] { 0x00, 0x10 };
			public static string separator = "À€";
			private string nils = Encoding.Default.GetString(new byte[] { 0x00 });
			private Dictionary<string, string> parameters = new Dictionary<string, string>();
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
		private struct CarrierInfo
		{
			public string carrierid;
			public int maxchars;
			public string humanname;
		}
		public enum YahooIMVironment
		{
			Doodle
		}
	}
}