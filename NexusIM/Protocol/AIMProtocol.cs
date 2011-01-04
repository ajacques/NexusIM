using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using csammisrun.OscarLib;
using NexusIM.Managers;

namespace InstantMessage
{
	public class IMAimProtocol : IMProtocol
	{
		public IMAimProtocol()
		{
			mStatus = IMStatus.OFFLINE;
			status = IMProtocolStatus.OFFLINE;
			protocolType = "AIM";
			mProtocolTypeShort = "aim";
			supportsIntroMsg = true;
			supportsMUC = true;
		}
		public override void BeginLogin()
		{
			base.BeginLogin();
			// Create the session
			connection = new Session(mUsername, mPassword);

			// Set Callbacks
			connection.LoginCompleted += new LoginCompletedHandler(connection_LoginCompleted);
			connection.LoginFailed += new LoginFailedHandler(connection_LoginFailed);
			connection.BuddyItemReceived += new BuddyItemReceivedHandler(connection_BuddyItemReceived);
			connection.ErrorMessage += new ErrorMessageHandler(connection_ErrorMessage);
			connection.ContactListFinished += new ContactListFinishedHandler(connection_ContactListFinished);
			connection.Statuses.UserStatusReceived += new UserStatusReceivedHandler(connection_UserStatusReceived);
			connection.Statuses.UserOffline += new UserOfflineHandler(connection_UserOffline);
			connection.Messages.MessageReceived += new MessageReceivedHandler(Messages_MessageReceived);
			connection.Messages.TypingNotification += new TypingNotificationEventHandler(connection_TypingNotification);
			connection.Messages.OfflineMessagesReceived += new OfflineMessagesReceivedEventHandler(Messages_OfflineMessagesReceived);
			connection.ChatInvitationReceived += new ChatInvitationReceivedHandler(connection_ChatInvitationReceived);
			connection.ChatRooms.ChatRoomJoined += new ChatRoomJoinedHandler(ChatRooms_ChatRoomJoined);

			// Set random settings
			connection.ClientCapabilities = Capabilities.Chat | Capabilities.UTF8;
			connection.ClientIdentification.ClientName = "NexusIM v1.0";

			// Now we login
			connection.Logon("login.messaging.aol.com", 5190);

			mEnabled = true;
		}
		public override void Disconnect()
		{
			// Sign off
			connection.Logoff();
			connection = null;

			triggerOnDisconnect(this, new IMDisconnectEventArgs(IMDisconnectEventArgs.DisconnectReason.User));
			status = IMProtocolStatus.OFFLINE;
			mStatus = IMStatus.OFFLINE;

			CleanupBuddyList();
		}
		public override void AddFriend(string name, string nickname, string group)
		{
			ushort groupid = connection.SSI.GetGroupByName(group).ID;
			connection.AddBuddy(name, groupid, nickname, "", "", "", "", true, "");
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (newstatus == mStatus)
				return; // Quit wasting my debugging time - get rid of this worthless request

			if (!mEnabled)
				return;

			if (newstatus == IMStatus.OFFLINE)
			{
				Disconnect();
				mEnabled = false;
				status = IMProtocolStatus.OFFLINE;
			} else if (newstatus == IMStatus.IDLE && IsOnlineStatusToOthers(mStatus) && mStatus != IMStatus.AWAY && status == IMProtocolStatus.ONLINE)
				connection.Statuses.SetIdleTime(1);
			else if (newstatus == IMStatus.AWAY && status == IMProtocolStatus.ONLINE)
				connection.Statuses.SetAwayMessage("Away from Keyboard");
			else if (newstatus == IMStatus.AVAILABLE)
			{
				if (!IsOnlineStatus(mStatus))
					BeginLogin();
				else if (connection != null && status == IMProtocolStatus.ONLINE)
					connection.Statuses.DropAwayMessage();
			}
			if (IsOnlineStatus(newstatus))
				mEnabled = true;
			else
				mEnabled = false;

			mStatus = newstatus;
		}
		public override void SendMessage(string friendName, string message)
		{
			connection.Messages.SendMessage(friendName, message, OutgoingMessageFlags.DeliverOffline);
		}
		public override void IsTyping(string uname, bool isTyping)
		{
			if (isTyping)
				connection.Messages.SendTypingNotification(uname, TypingNotification.TypingStarted);
			else
				connection.Messages.SendTypingNotification(uname, TypingNotification.TypingFinished);
		}
		public override string GetServerString(string username)
		{
			return "login.messaging.aol.com";
		}
		public override void CreateMUCRoom(string name)
		{
			connection.ChatRooms.CreateChatRoom(name, System.Globalization.CultureInfo.CurrentCulture, System.Text.Encoding.UTF8);
		}
		public override IChatRoom JoinChatRoom(string room)
		{
			throw new NotImplementedException();
			connection.ChatRooms.JoinChatRoom(chatRoomInviteKeys[room]);
		}
		public override void SendMessageToRoom(string roomName, string message)
		{
			chatrooms[roomName].SendMessage(message);
		}
		public override void LeaveChatRoom(string room)
		{
			chatrooms[room].LeaveRoom();
		}
		public override void InviteToChatRoom(string username, string room)
		{
		}

		// Callbacks
		void connection_ChatInvitationReceived(Session sess, UserInfo sender, string roomname, string message, Encoding encoding, string language, Cookie key)
		{
			//connection.ChatRooms.JoinChatRoom(key);
			//return;
			chatRoomInviteKeys.Add(roomname, key);
			triggerOnChatRoomInvite(new IMRoomInviteEventArgs(sender.ScreenName, roomname, message));
		}
		void ChatRooms_ChatRoomJoined(object sender, ChatRoom newroom)
		{
			chatrooms.Add(newroom.DisplayName, newroom);
			frmMUCChatWindow win = new frmMUCChatWindow();
			win.Protocol = this;
			chatwins.Add(newroom, win);
			newroom.MessageReceived += new MessageReceivedHandler(newroom_MessageReceived);

			MethodInvoker invoker = new MethodInvoker(delegate() { win.Show(); });
			frmMain.Instance.BeginInvoke(invoker);
		}
		void newroom_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			ChatRoom room = (ChatRoom)sender;

			string newmessage = "";
			newmessage = Regex.Replace(e.Message.Message, "</?html>", "", RegexOptions.IgnoreCase);

			chatwins[room].AppendChatMessage(e.Message.ScreenName, newmessage);
		}
		private void connection_LoginCompleted(Session sess)
		{
			triggerOnLogin(new EventArgs());
			status = IMProtocolStatus.ONLINE;
			mStatus = IMStatus.AVAILABLE;

			connection.Messages.RetrieveOfflineMessages();
		}
		private void connection_LoginFailed(Session sess, LoginErrorCode error)
		{
			if (error == LoginErrorCode.WrongPassword || error == LoginErrorCode.InvalidScreennamePassword || error == LoginErrorCode.IncorrectScreennamePassword)
			{
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_PASSWORD));
			} else if (error == LoginErrorCode.RateLimitExceeded) {
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.LIMIT_REACHED, "You have reached the max sign-in limit. NexusIM will automatically sign you back in after a the rate limit has passed. Please wait a few minutes before manually signing-in again."));
			}
			status = IMProtocolStatus.ERROR;
		}
		private void connection_BuddyItemReceived(Session sess, SSIBuddy buddy)
		{
			try {
				IMBuddy mBuddy = new IMBuddy(this, buddy.Name);
				mBuddy.Nickname = buddy.DisplayName;
				mBuddy.Group = sess.SSI.GetGroupByID(buddy.GroupID).Name;
				
				// For mobile contacts
				if (buddy.Name.Substring(0, 1) == "+")
					mBuddy.IsMobileContact = true;
				mBuddy.SMSNumber = buddy.SMS;

				buddylist.Add(mBuddy);
				mBuddy.Populate();
			} catch (Exception) {}
		}
		private void connection_ContactListFinished(Session sess, DateTime time)
		{
			connection.ActivateBuddyList();
		}
		private void connection_ErrorMessage(Session sess, ServerErrorCode code)
		{
			status = IMProtocolStatus.ERROR;
			triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.CONNERROR));
		}
		private void Messages_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			string newmessage = Regex.Replace(e.Message.Message, @"<(.|\n)*?>", string.Empty);

			IMBuddy buddy = AccountManager.GetByName(e.Message.SenderInfo.ScreenName, this);
			buddy.ShowRecvMessage(newmessage);
			buddy.ShowIsTypingMessage(false);
		}
		private void connection_UserStatusReceived(object sender, UserInfo info)
		{
			IMBuddy buddy = AccountManager.GetByName(info.ScreenName, this);

			buddy.StatusMessage = info.AvailableMessage;
			buddy.Online = true;
			buddy.Status = IMBuddyStatus.Available;
			buddy.UpdateListItem();
		}
		private void connection_UserOffline(object sender, UserInfo info)
		{
			IMBuddy buddy = AccountManager.GetByName(info.ScreenName, this);
			buddy.StatusMessage = "";
			buddy.Online = false;
			buddy.Status = IMBuddyStatus.Offline;
			buddy.UpdateListItem();
		}
		private void connection_TypingNotification(object sender, TypingNotificationEventArgs e)
		{
			AccountManager.GetByName(e.ScreenName, this).ShowIsTypingMessage(e.Notification == TypingNotification.TypingStarted);
		}
		private void Messages_OfflineMessagesReceived(object sender, OfflineMessagesReceivedEventArgs e)
		{
			IEnumerator messages = e.ReceivedMessages.GetEnumerator();
			while (messages.MoveNext())
			{
				OfflineIM message = (OfflineIM)messages.Current;
				IMBuddy buddy = AccountManager.GetByName(message.ScreenName);
				buddy.ShowRecvMessage(message.Message);
			}
		}

		private Session connection = null;
		private Dictionary<string, ChatRoom> chatrooms = new Dictionary<string, ChatRoom>();
		private Dictionary<ChatRoom, frmMUCChatWindow> chatwins = new Dictionary<ChatRoom, frmMUCChatWindow>();
		private Dictionary<string, Cookie> chatRoomInviteKeys = new Dictionary<string, Cookie>();
	}
}