using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Xml;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InstantMessage
{
	/// <summary>
	/// Used to identify what status to be reported to the protocol servers
	/// </summary>
	public enum IMStatus
	{
		AVAILABLE,
		AWAY,
		BUSY,
		IDLE,
		INVISIBLE,
		OFFLINE
	}

	/// <summary>
	/// Used for internal purposes to know what status a protocol is in
	/// </summary>
	public enum IMProtocolStatus
	{
		ONLINE,
		ERROR,
		OFFLINE
	}

	/// <summary>
	/// Used for internal purposes to know what status a buddy is
	/// </summary>
	public enum IMBuddyStatus
	{
		ONLINE,
		OFFLINE,
		BUSY,
		AFK,
		IDLE
	}

	/// <summary>
	/// Used to store data about a protocol or buddy and placed inside a Tag of a control
	/// </summary>
	[Obsolete("I'm sure there's a better way to do this", false)]
	public struct IMDataStruct
	{
		public IMProtocol protocol;
		public IMBuddy buddy;
		public bool isChanged;
	}

	/// <summary>
	/// Stores information about a single contact
	/// </summary>
	public class IMBuddy
	{
		/// <summary>
		/// Creates a new buddy
		/// </summary>
		/// <param name="protocol">Pointer to the protocol this buddy belongs to</param>
		/// <param name="username">This buddy's username</param>
		public IMBuddy(IMProtocol protocol, string username)
		{
			mUsername = username;
			mProtocol = protocol;
		}
		/// <summary>
		/// Opens the chat window for a buddy if it's not already open
		/// </summary>
		/// <param name="userinvoked">False if the window should be minimized after opening</param>
		public void showWindow(bool userinvoked)
		{
			if (!windowOpen)
			{
				if (frmMain.Instance.InvokeRequired)
				{
					MethodInvoker invoker = new MethodInvoker(delegate() { this.showWindow(userinvoked); });
					frmMain.Instance.Invoke(invoker);
				} else {
					chatWindow = new frmChatWindow();
					chatWindow.Buddy = this;
					chatWindow.Protocol = mProtocol;
					if (!userinvoked)
						chatWindow.WindowState = FormWindowState.Minimized;

					// Line up windows next to each other
					IEnumerable<frmChatWindow> result = from IMBuddy s in AccountManager.MergeAllBuddyLists() where s.WindowOpen select s.ChatWindow;
					int farright = 0;
					int bottom = 0;
					int top = 0;

					foreach (frmChatWindow window in result)
					{
						farright += window.Location.X + window.Width;
						if (window.Location.Y > bottom)
							bottom = window.Location.Y;
						if (window.Location.Y < top)
							top = window.Location.Y;
					}

					if (farright != 0)
					{
						if (Screen.PrimaryScreen.Bounds.Width > (farright + chatWindow.Width))
						{
							int y = 0;
							if (Screen.PrimaryScreen.Bounds.Bottom > (bottom + chatWindow.Height))
								y = bottom;
							else if (0 < (top = chatWindow.Height))
								y = top - chatWindow.Height;
							else {
								farright = chatWindow.Location.X;
								y = chatWindow.Location.Y;
							}
							chatWindow.Location = new System.Drawing.Point(farright, y);
						}
					}

					chatWindow.Show();
					windowOpen = true;
				}
			}
		}
		/// <summary>
		/// Sends a message to this user
		/// </summary>
		/// <param name="message">Message contents</param>
		public void sendMessage(string message)
		{
			mProtocol.SendMessage(mUsername, message);
		}
		/// <summary>
		/// Used to create the ListViewItem then insert it into the frmMain Contact List
		/// </summary>
		public void Populate()
		{
			if (frmMain.Instance.InvokeRequired)
			{
					MethodInvoker invoker = new MethodInvoker(delegate() { this.Populate(); });
					frmMain.Instance.Invoke(invoker);
			} else {
				if (mBListItem == null)
				{
					mBListItem = new ListViewItem();

					UpdateListItem();
					frmMain.Instance.AppendBuddyItemMethod(mBListItem);
					mBlistItemVisible = true;

					int row = frmMain.Instance.ContactList.Items.IndexOf(mBListItem);

					Button pB1 = new Button();
					pB1.ImageList = frmMain.Instance.protocolPics;
					pB1.ForeColor = System.Drawing.Color.Transparent;
					pB1.ImageIndex = 0;
					pB1.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
					pB1.Click += new EventHandler(button_Click);

					//frmMain.Instance.ContactList.AddEmbeddedControl(pB1, 2, row);
				}
			}
		}
		/// <summary>
		/// Updates the ListViewItem
		/// </summary>
		public void UpdateListItem()
		{
			if (!mIsManaged)
			{
				if (frmMain.Instance.InvokeRequired)
				{
					MethodInvoker invoker = new MethodInvoker(delegate() { this.UpdateListItem(); });
					frmMain.Instance.Invoke(invoker);
				} else {
					BuddyList contactlist = frmMain.Instance.ContactList;
					if (mBListItem != null && mBListItem.Tag != null)
					{
						mBListItem.Tag = null;
					}
					if (mBListItem == null)
						Populate();
					IMDataStruct data = new IMDataStruct();
					data.protocol = mProtocol;
					data.buddy = this;
					mBListItem.Tag = ((System.Object)data);
					if (mNickname != "")
						mBListItem.Text = mNickname;
					else
						mBListItem.Text = mUsername;

					if (buddyImage != null)
					{
						if (!contactlist.LargeImageList.Images.ContainsKey(buddyImageKey))
							contactlist.LargeImageList.Images.Add(buddyImageKey, buddyImage);

						mBListItem.ImageIndex = contactlist.LargeImageList.Images.IndexOfKey(buddyImageKey);
						if (!isOnline)
						{
							Bitmap bwimage = ConvertToGrayscale((Bitmap)buddyImage);
							contactlist.SetBuddyAvatar(mBListItem, bwimage);
						} else {
							contactlist.SetBuddyAvatar(mBListItem, buddyImage);
						}
					}

					if (!Online)
					{
						if (Convert.ToBoolean(Settings.GetCustomSetting("offlinegroup", "False")))
						{
							if (contactlist.Groups["Offline"] == null)
								contactlist.Groups.Add("Offline", "Offline");
							mBListItem.Group = contactlist.Groups["Offline"];
						} else {
							GroupUpdate();
						}
					} else {
						GroupUpdate();
					}

					contactlist.SetBuddyStatus(mBListItem, Status);

					if (tagImage != null)
					{
						contactlist.AddPicture(mBListItem, tagImage, new Point(5, 3), true);
					}

					if (!mBlistItemVisible && contactlist.Items.Contains(mBListItem))
					{
						mBListItem.Remove();
					} else if (mBlistItemVisible && !contactlist.Items.Contains(mBListItem)) {
						contactlist.Items.Add(mBListItem);
					}

					contactlist.SetStatusMessage(mBListItem, mStatusMessage);

					if (isOnline)
					{
						mBListItem.ToolTipText = "Online\n";
						mBListItem.ForeColor = System.Drawing.Color.Aqua;
					} else {
						mBListItem.ToolTipText = "Offline\n";
						mBListItem.ForeColor = System.Drawing.Color.Black;
					}
					mBListItem.ToolTipText += "Account: " + mProtocol.Username;

					mBlistItemVisible = true;
					if (contactlist.Items.IndexOf(mBListItem) != -1)
						contactlist.RedrawItems(contactlist.Items.IndexOf(mBListItem), contactlist.Items.IndexOf(mBListItem), false);
				}
			}
		}
		/// <summary>
		/// Used by the IMProtocol class to show a message that the user received from this buddy
		/// </summary>
		/// <param name="messages">What message to display</param>
		public void ShowRecvMessage(string messages)
		{
			if (!windowOpen)
			{
				showWindow(false);
			}

			chatWindow.AppendChatMessage(mUsername, messages);
		}
		/// <summary>
		/// Used by the IMProtocol class to show a message that the user received from this buddy.
		/// This version of the function is mostly used to display offline messages
		/// </summary>
		/// <param name="message">What message to display</param>
		/// <param name="recvtime">What time the message was received at. Used for offline messages</param>
		public void ShowRecvMessage(string message, DateTime recvtime)
		{
			if (!windowOpen)
				showWindow(false);

			chatWindow.AppendChatMessage(mUsername, message, recvtime);
		}
		public void ShowCustomMessage(MessageContents msg)
		{
			if (!windowOpen)
				showWindow(false);

			chatWindow.AppendCustomMessage(msg, true);
		}
		public void ShowIsTypingMessage(bool isTyping)
		{
			if (!windowOpen && Convert.ToBoolean(Settings.GetCustomSetting("psychicchat", "False")))
				showWindow(false);

			if (chatWindow != null)
				chatWindow.ChangeTypingStatus(isTyping);
		}
		public void Buzz()
		{
			if (!windowOpen)
				showWindow(false);

			chatWindow.AppendCustomMessage(new BuzzMessage(DisplayName));

			//chatWindow.AppendCustomMessage(mUsername + " has buzzed you");
			chatWindow.ShakeWindow();
		}
		public void RemoveContactItem()
		{
			frmMain.Instance.RemoveBuddyItem(mBListItem);
			mBListItem = null;
			mBlistItemVisible = false;
		}
		private void GroupUpdate()
		{
			if (mBListItem == null)
				return;

			ListView contactlist = frmMain.Instance.ContactList;
			if (contactlist.Groups[mGroup] == null)
				contactlist.Groups.Add(mGroup, mGroup);

			mBListItem.Group = contactlist.Groups[mGroup];
		}
		private void button_Click(object sender, EventArgs e)
		{
			showWindow(true);
		}
		public override string ToString()
		{
			return "Username: " + mUsername + " - Account: " + mProtocol.Protocol + " - " + mProtocol.Username;
		}
		private Bitmap ConvertToGrayscale(Bitmap source)
		{
			Bitmap bm = new Bitmap(source.Width, source.Height);
			
			for (int y = 0; y < bm.Height; y++)
			{
				for (int x = 0; x < bm.Width; x++)
				{
					Color c = source.GetPixel(x, y);
					int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
					bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
				}
			}
			return bm;
		}

		// Properties
		public ListViewItem ContactItem
		{
			get	{
				return mBListItem;
			}
		}
		/// <summary>
		/// Gets the protocol this buddy is associated with
		/// </summary>
		public IMProtocol Protocol
		{
			get {
				return mProtocol;
			}
		}
		public ContactGroup ContactGroup
		{
			get {
				return mContactGroup;
			}
			set	{
				mContactGroup = value;
			}
		}
		/// <summary>
		/// Gets the chat window associated with this buddy
		/// </summary>
		public frmChatWindow ChatWindow
		{
			get
			{
				return chatWindow;
			}
			set
			{
				chatWindow = value;
			}
		}
		/// <summary>
		/// True if the Contact Item (displayed on the contact list) is currently being displayed
		/// </summary>
		public bool ContactItemVisible
		{
			get	{
				return mBlistItemVisible;
			}
			set	{
				mBlistItemVisible = value;
				UpdateListItem();
				
			}
		}
		/// <summary>
		/// True if the ListViewItem is not managed by this class. Used mostly for contact grouping
		/// </summary>
		public bool IsManaged
		{
			get	{
				return mIsManaged;
			}
			set	{
				mIsManaged = value;
			}
		}
		/// <summary>
		/// True if the buddy is currently online/visible
		/// </summary>
		public bool Online
		{
			get {
				return isOnline;
			}
			set {
				if (value)
				{
					if (onSignIn != null)
						onSignIn(this, new EventArgs());
				} else {
					if (onSignOut != null)
						onSignOut(this, new EventArgs());
				}
				isOnline = value;
				UpdateListItem();
			}
		}
		/// <summary>
		/// True if a chat window for this buddy is currently open
		/// </summary>
		public bool WindowOpen
		{
			get
			{
				return windowOpen;
			}
			set
			{
				if (!value)
				{
					chatWindow = null;
				}
				else
				{
					if (onWindowOpen != null)
						onWindowOpen(this, null);
				}
				windowOpen = value;
			}
		}
		public string DisplayName
		{
			get {
				if (mNickname != "")
					return mNickname;
				else
					return mUsername;
			}
		}
		/// <summary>
		/// True if this buddy is a mobile contact (ie. IMs are sent to a phone)
		/// </summary>
		public bool IsMobileContact
		{
			get {
				return mIsMobileContact;
			}
			set {
				mIsMobileContact = value;
			}
		}
		/// <summary>
		/// True if this buddy is an internal-use only buddy. Used by the protocol class
		/// </summary>
		[Obsolete("No longer needed", false)]
		public bool IsInternalBuddy
		{
			get {
				return mIsInternalUsage;
			}
			set {
				mIsInternalUsage = value;
			}
		}
		/// <summary>
		/// True if this buddy can accept messages
		/// </summary>
		public bool AcceptsMessages
		{
			get {
				return mAcceptsMessages;
			}
			set {
				mAcceptsMessages = value;
			}
		}
		/// <summary>
		/// Gets this buddy's avatar image
		/// </summary>
		public Image BuddyImage
		{
			get {
				return buddyImage;
			}
			set {
				buddyImage = value;
				backupImage = (Image)value.Clone();
				UpdateListItem();
			}
		}
		[Obsolete("Use the Options system", false)]
		public string BuddyImageKey
		{
			get {
				return buddyImageKey;
			}
			set {
				buddyImageKey = value;
			}
		}
		/// <summary>
		/// True if this buddy is on the user's buddy list. False if it's a random person
		/// </summary>
		public bool IsOnBuddyList
		{
			get {
				return mIsOnBuddyList;
			}
			set {
				mIsOnBuddyList = value;
			}
		}
		public Image BuddyTagImage
		{
			get {
				return tagImage;
			}
			set {
				tagImage = value;
				UpdateListItem();
			}
		}
		public string SMSNumber
		{
			get {
				return mSMSnumber;
			}
			set {
				mSMSnumber = value;
			}
		}
		/// <summary>
		/// Gets or sets the nickname to use for this buddy
		/// </summary>
		public string Nickname
		{
			get
			{
				return mNickname;
			}
			set
			{
				mNickname = value;
				UpdateListItem();
			}
		}
		public Dictionary<string, string> Options
		{
			get {
				return data;
			}
		}
		/// <summary>
		/// Gets the buddy's username. Set should only be used by the protocol during class construction
		/// </summary>
		public string Username
		{
			get {
				return mUsername;
			}
			set	{
				mUsername = value;
				UpdateListItem();
			}
		}
		public DateTime LocalTime
		{
			get
			{
				if (mProtocol.GetType().Name == "IMJabberProtocol")
				{
				}
				return DateTime.Now;
			}
		}
		public string StatusMessage
		{
			get {
				return mStatusMessage;
			}
			set {
				mStatusMessage = value;
				UpdateListItem();
			}
		}
		public string Group
		{
			get {
				return mGroup;
			}
			set {
				if (value != mGroup)
				{
					UpdateListItem();
				}
				mGroup = value;
			}
		}
		public int MaxMessageLength
		{
			get {
				return mMessageLength;
			}
			set {
				mMessageLength = value;
			}
		}
		public IMBuddyStatus Status
		{
			get {
				return mStatus;
			}
			set {
				mStatus = value;
				UpdateListItem();
			}
		}

		// Events
		public event EventHandler onSignIn;
		public event EventHandler onSignOut;
		public event EventHandler onStatusChange;
		public event EventHandler onWindowOpen;
		
		// Variables
		protected frmChatWindow chatWindow = null;
		protected String mUsername = "";
		protected String mNickname = "";
		protected IMProtocol mProtocol = null;
		protected ListViewItem mBListItem = null;
		protected ContactGroup mContactGroup = null;
		protected bool windowOpen = false; // Is the window currently open
		protected bool isOnline = false;
		protected bool mBlistItemVisible = false;
		protected bool mIsManaged = false;
		protected bool mIsMobileContact = false;
		protected bool mIsInternalUsage = false;
		protected bool mAcceptsMessages = true;
		protected bool mIsOnBuddyList = true;
		protected IMBuddyStatus mStatus = IMBuddyStatus.OFFLINE;
		protected string mStatusMessage = "";
		protected string mGroup = "";
		protected string mSMSnumber = "";
		protected int itemSortIndex = 0;
		protected int mMessageLength = -1;
		protected Image buddyImage = null;
		protected Image tagImage = null;
		protected Image backupImage = null;
		protected string buddyImageKey = "";
		protected Dictionary<string, string> data = new Dictionary<string, string>();
	}

	/// <summary>
	/// Used to pass information about a protocol error back to event handlers for higher-level error processing
	/// </summary>
	public class IMErrorEventArgs : EventArgs
	{
		public IMErrorEventArgs(ErrorReason reason)
		{
			mReason = reason;
		}
		public IMErrorEventArgs(ErrorReason reason, string message)
		{
			mReason = reason;
			mMessage = message;
		}
		public enum ErrorReason
		{
			CONNERROR,
			INVALID_PASSWORD,
			LIMIT_REACHED
		}
		public ErrorReason Reason
		{
			get {
				return mReason;
			}
		}
		public string Message
		{
			get {
				return mMessage;
			}
		}
		private ErrorReason mReason;
		private string mMessage;
	}

	/// <summary>
	/// Used to pass information about the protocol disconnect back to event handlers for higher-level error processing
	/// </summary>
	public class IMDisconnectEventArgs : EventArgs
	{
		public IMDisconnectEventArgs(DisconnectReason reason)
		{
			mReason = reason;
		}
		public enum DisconnectReason
		{
			OtherClient,
			ServerError,
			User,
			Unknown
		}
		public DisconnectReason Reason
		{
			get
			{
				return mReason;
			}
		}
		private DisconnectReason mReason;
	}

	public class IMMessageEventArgs : EventArgs
	{
		public IMMessageEventArgs(string from, string to, string message)
		{
			mSender = AccountManager.GetByName(from);
			mReceiver = to;
			mMessage = message;
		}
		public IMMessageEventArgs(IMBuddy from, string to, string message)
		{
			mSender = from;
			mReceiver = to;
			mMessage = message;
		}
		public IMBuddy Sender
		{
			get {
				return mSender;
			}
		}
		public string Receiver
		{
			get {
				return mReceiver;
			}
		}
		public string Message
		{
			get {
				return mMessage;
			}
		}
		private IMBuddy mSender;
		private string mReceiver = "";
		private string mMessage = "";
	}

	public class IMFriendEventArgs : EventArgs
	{
		public IMFriendEventArgs(IMBuddy buddy)
		{
			mBuddy = buddy;
		}
		public IMBuddy Buddy
		{
			get
			{
				return mBuddy;
			}
		}
		private IMBuddy mBuddy;
	}

	public class IMFriendRequestEventArgs : EventArgs
	{
		public IMFriendRequestEventArgs(string username, string message, string displayname)
		{
			mUsername = username;
			mMessage = message;
			mDisplayName = displayname;
		}
		public IMFriendRequestEventArgs(string username, string displayname)
		{
			mUsername = username;
			mDisplayName = displayname;
		}
		public string Username
		{
			get {
				return mUsername;
			}
		}
		public string Message
		{
			get {
				return mMessage;
			}
		}
		public string DisplayName
		{
			get {
				return mDisplayName;
			}
		}
		private string mUsername = "";
		private string mMessage = "";
		private string mDisplayName = "";
	}

	public class IMEmailEventArgs : EventArgs
	{
		public IMEmailEventArgs(string sender, string displayname, string subject)
		{
			mSender = sender;
			mDisplayName = displayname;
			mSubject = subject;
		}
		public string Sender
		{
			get {
				return mSender;
			}
		}
		public string DisplayName
		{
			get {
				return mDisplayName;
			}
		}
		public string Subject
		{
			get {
				return mSubject;
			}
		}
		private string mSender = "";
		private string mDisplayName = "";
		private string mSubject = "";
	}

	public class IMRoomInviteEventArgs : EventArgs
	{
		public IMRoomInviteEventArgs(string sender, string roomname, string message)
		{
			mSender = sender;
			mRoomName = roomname;
			mMessage = message;
		}
		public string Sender
		{
			get {
				return mSender;
			}
		}
		public string RoomName
		{
			get {
				return mRoomName;
			}
		}
		public string Message
		{
			get {
				return mMessage;
			}
		}
		private string mSender = "";
		private string mRoomName = "";
		private string mMessage = "";
	}

	public enum UserVisibilityStatus
	{
		/// <summary>
		/// Show up as online to this user
		/// </summary>
		ONLINE,
		/// <summary>
		/// Show up as offline to this user
		/// </summary>
		OFFLINE,
		/// <summary>
		/// Show up as permanently offline to this user
		/// </summary>
		PERMANENTLY_OFFLINE
	}

	/// <summary>
	/// Stores information and handles communication for specific IM networks
	/// </summary>
	public abstract class IMProtocol
	{
		public IMProtocol()
		{
			status = IMProtocolStatus.OFFLINE;
			mStatus = IMStatus.OFFLINE;
			mEnabled = false;
			buddylist = new System.Collections.ArrayList();
		}
		// Protocol Interface
		/// <summary>
		/// Connects and authenticates with the server
		/// </summary>
		public virtual void Login()
		{
			if (mPassword == "")
			{
				status = IMProtocolStatus.ERROR;
				frmRequestPassword win = new frmRequestPassword();
				win.Protocol = this;
				win.ShowDialog();
				throw new WarningException("Missing Password. Catch this and continue with other account connections");
			}

			onMessageReceive += new EventHandler<IMMessageEventArgs>(AutoResponder.MessageReceive);
			onChatRoomInvite += new EventHandler<IMRoomInviteEventArgs>(MUCManager.RoomInviteCallback);
			onFriendRequest += new EventHandler<IMFriendRequestEventArgs>(Notifications.protocol_onFriendRequest);
		}
		/// <summary>
		/// Disconnects from the server
		/// </summary>
		public abstract void Disconnect();
		/// <summary>
		/// Adds a person to the user's contact list
		/// </summary>
		/// <param name="name">The user to add</param>
		/// <param name="nickname">Nickname to use</param>
		/// <param name="group">Which group to place the buddy into</param>
		public virtual void AddFriend(string name, string nickname, string group) {}
		public virtual void AddFriend(string name, string nickname, string group, string introMsg) {}
		/// <summary>
		/// Removes a person from the user's contact list
		/// </summary>
		/// <param name="uname">What user to remove</param>
		public virtual void RemoveFriend(string uname, string group) {}
		public virtual void RemoveFriend(string uname)
		{
			RemoveFriend(uname, AccountManager.GetByName(uname, this).Group);
		}
		/// <summary>
		/// Sends a private message to another user
		/// </summary>
		/// <param name="friendName">What person to send the message to</param>
		/// <param name="message">The contents of the message</param>
		public virtual void SendMessage(string friendName, string message) {}
		public virtual void SendMessage(IMBuddy buddy, string message)
		{
			SendMessage(buddy, message, false);
		}
		public virtual void SendMessage(IMBuddy buddy, string message, bool usesms)
		{
			SendMessage(buddy.Username, message);
		}
		public virtual void SendMessageToRoom(string roomName, string message) {}
		/// <summary>
		/// Changes the users status to reflect their current status
		/// </summary>
		/// <param name="status">What status to switch to</param>
		public virtual void ChangeStatus(IMStatus newstatus)
		{
			if (!mEnabled)
				return; // If it's not even enabled

			if (newstatus == mStatus)
				return;

			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus))
				Login();
			else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus))
				Disconnect();

			if (IsOnlineStatus(newstatus))
				mEnabled = true;
			else
				mEnabled = false;

			mStatus = newstatus;
		}
		/// <summary>
		/// Sends a notification to a contact that the user is currently a typing a message to them
		/// </summary>
		/// <param name="uname">Which contact to the send the notification to</param>
		/// <param name="isTyping">Is the user currently typing a message?</param>
		public virtual void IsTyping(string uname, bool isTyping) {}
		public virtual void SetPerUserVisibilityStatus(string buddy, UserVisibilityStatus status) {}
		/// <summary>
		/// Sets a message to show to other users along with the status
		/// </summary>
		/// <param name="status">String message to display</param>
		public virtual void SetStatusMessage(string status) {}
		public virtual void CreateMUCRoom(string name) {}
		/// <summary>
		/// Sends a notification to another user requested his/her attention
		/// </summary>
		/// <param name="uname">Who to send the notification to</param>
		public virtual void BuzzFriend(string uname) {}
		protected virtual void ChangeAvatar() {}
		public virtual string GetServerString(string username)
		{
			return "";
		}
		public virtual void JoinChatRoom(string room) {}
		public virtual void LeaveChatRoom(string room) {}
		public virtual void ReplyToBuddyAddRequest(string username, bool isAdded) {}
		public virtual void HandleProtocolCMDArg(string input) {}
		/// <summary>
		/// Invites a user to a chat room
		/// </summary>
		/// <param name="username">The username to send to invite to</param>
		/// <param name="room">The room to invite the user to</param>
		public virtual void InviteToChatRoom(string username, string room)
		{
			InviteToChatRoom(username, room, "");
		}
		/// <summary>
		/// Invites a user to a chat room with an invitation string
		/// </summary>
		/// <param name="username">The username to send to invite to</param>
		/// <param name="room">The room to invite the user to</param>
		/// <param name="inviteText">The string to display to the recipient</param>
		public virtual void InviteToChatRoom(string username, string room, string inviteText) {}
		public override string ToString()
		{
			if (mUsername != "")
				return protocolType + " - " + mUsername;
			else
				return protocolType;
		}
		/// <summary>
		/// Checks to see if the requested status would cause the current protocol to login to the server
		/// </summary>
		/// <param name="status">What status to test</param>
		/// <returns>True if the requested server is an online status</returns>
		public virtual bool IsOnlineStatus(IMStatus status)
		{
			if (status == IMStatus.OFFLINE)
				return false;
			else
				return true;
		}
		/// <summary>
		/// Checks to see if the requested status would cause the current protocol to show up as online to other people
		/// </summary>
		/// <param name="status">What status to test</param>
		/// <returns>True if the requested server is a non-invisible/offline status</returns>
		public virtual bool IsOnlineStatusToOthers(IMStatus status)
		{
			if (status == IMStatus.OFFLINE)
				return false;
			else
				return true;
		}
		[Obsolete("Use ContactList", false)]
		public Array GetBuddyList()
		{
			blistChange = false;
			return buddylist.ToArray();
		}
		/// <summary>
		/// True if this protocol is currently enabled
		/// </summary>
		public bool Enabled
		{
			get {
				return mEnabled;
			}
			set {
				mEnabled = value;
			}
		}
		public IMStatus Status
		{
			get {
				return mStatus;
			}
		}
		/// <summary>
		/// True if this protocol is currently connected to the server
		/// </summary>
		public bool Connected
		{
			get {
				return mConnected;
			}
		}
		/// <summary>
		/// Gets or sets the username used by the protocol
		/// </summary>
		public string Username
		{
			get {
				return mUsername;
			}
			set {
				mUsername = value;
			}
		}
		/// <summary>
		/// Gets or sets the password used by the protocol
		/// </summary>
		public string Password
		{
			get {
				return mPassword;
			}
			set {
				mPassword = value;
			}
		}
		/// <summary>
		/// Gets the current protocol type in string format
		/// </summary>
		public string Protocol
		{
			get {
				return protocolType;
			}
		}
		public string ShortProtocol
		{
			get {
				return mProtocolTypeShort;
			}
		}
		/// <summary>
		/// Gets or sets the avatar to display to other users
		/// </summary>
		public string Avatar
		{
			get {
				return mAvatar;
			}
			set {
				mAvatar = value;
				ChangeAvatar();
			}
		}
		/// <summary>
		/// True if the current protocol supports multi user chat.
		/// </summary>
		public bool SupportsMultiUserChat
		{
			get {
				return supportsMUC;
			}
		}
		public bool SupportsPerUserVisibility
		{
			get {
				return supportsUserInvisiblity;
			}
		}
		/// <summary>
		/// True if the current protocol supports getting another user's attention
		/// </summary>
		public bool SupportsUserNotify
		{
			get {
				return supportsBuzz;
			}
		}
		public bool SupportsIntroMessage
		{
			get {
				return supportsIntroMsg;
			}
		}
		public string StatusMessage
		{
			get {
				return mStatusMessage;
			}
			set {
				mStatusMessage = value;
				SetStatusMessage(value);
			}
		}
		/// <summary>
		/// True if this account's password will be saved to the user's config file
		/// </summary>
		public bool SavePassword
		{
			get {
				return savepassword;
			}
			set {
				savepassword = value;
			}
		}
		/// <summary>
		/// Server string to connect to when logging in
		/// </summary>
		public string Server
		{
			get {
				return mServer;
			}
			set {
				mServer = value;
			}
		}
		/// <summary>
		/// True if this account type requires a username
		/// </summary>
		public bool RequiresUsername
		{
			get {
				return needUsername;
			}
		}
		/// <summary>
		/// True if this account type requires a password
		/// </summary>
		public bool RequiresPassword
		{
			get {
				return needPassword;
			}
		}
		/// <summary>
		/// True if this protocol will be saved to the user's config file
		/// </summary>
		public bool EnableSaving
		{
			get {
				return enableSaving;
			}
			set {
				enableSaving = value;
			}
		}
		
		/// <summary>
		/// Gets the current internal protocol status
		/// </summary>
		public IMProtocolStatus ProtocolStatus
		{
			get {
				return status;
			}
		}
		/// <summary>
		/// Creates a IMProtocol from the passed string
		/// </summary>
		/// <param name="name">The protocol type string</param>
		/// <returns>Your fancy new IMProtocol</returns>
		public static IMProtocol FromString(string name)
		{
			name = name.ToLower();
			if (name == "yahoo")
			{
				return new IMYahooProtocol();
			} else if (name == "jabber") {
				return new IMJabberProtocol();
			} else if (name == "msn") {
				return new IMMSNProtocol();
			} else if (name == "aim") {
				return new IMAimProtocol();
			} else if (name == "pnm") {
				return new IMPNMProtocol();
			} else if (name == "skype") {
				return new IMSkypeProtocol();
			} else if (name == "facebook") {
				return new IMFacebookProtocol();
			} else if (name == "irc" ) {
				return new IMIRCProtocol();
			} else if (name == "profile") {
				return new IMProfileProtocol();
			} else {
				return null;
			}
		}
		public static string FromProtocolString(string protocol)
		{
			protocol = protocol.ToLower();
			if (protocol == "ymsgr")
				return "yahoo";
			else if (protocol == "irc")
				return "irc";
			else
				return "";
		}
		protected void CleanupBuddyList()
		{
			IEnumerator i = buddylist.GetEnumerator();
			while (i.MoveNext())
			{
				IMBuddy buddy = (IMBuddy)i.Current;
				buddy.RemoveContactItem();
			}
			buddylist.Clear();
		}

		// Events
		public event EventHandler onLogin;
		public event EventHandler<IMErrorEventArgs> onError;
		public event EventHandler<IMDisconnectEventArgs> onDisconnect;
		public event EventHandler<IMFriendRequestEventArgs> onFriendRequest;
		public event EventHandler<IMFriendEventArgs> onFriendSignIn;
		public event EventHandler<IMFriendEventArgs> onFriendSignOut;
		public event EventHandler<IMRoomInviteEventArgs> onChatRoomInvite;
		public event EventHandler<IMMessageEventArgs> onMessageReceive;
		public event EventHandler<IMMessageEventArgs> onMessageSend;
		public event EventHandler<IMEmailEventArgs> onEmailReceive;
		

		// Internal Event Triggers
		protected void triggerOnLogin(EventArgs e)
		{
			if (onLogin != null)
				onLogin(this, e);
		}
		protected void triggerOnError(IMErrorEventArgs e)
		{
			if (onError != null)
				onError(this, e);
		}
		protected void triggerOnDisconnect(object sender, IMDisconnectEventArgs e)
		{
			if (onDisconnect != null)
				onDisconnect(sender, e);
		}
		protected void triggerOnFriendRequest(object sender, IMFriendRequestEventArgs e)
		{
			if (onFriendRequest != null)
				onFriendRequest(sender, e);
		}
		[Obsolete("Use triggerOnFriendSignIn(IMFriendEventArgs) instead", false)]
		protected void triggerOnFriendSignIn(object sender, IMFriendEventArgs e)
		{
			if (onFriendSignIn != null)
				onFriendSignIn(sender, e);
		}
		protected void triggerOnFriendSignIn(IMFriendEventArgs e)
		{
			if (onFriendSignIn != null)
				onFriendSignIn(this, e);
		}
		protected void triggerOnFriendSignOut(IMFriendEventArgs e)
		{
			if (onFriendSignOut != null)
				onFriendSignOut(this, e);
		}
		protected void triggerOnMessageReceive(object sender, IMMessageEventArgs e)
		{
			if (onMessageReceive != null)
				onMessageReceive(sender, e);
		}
		protected void triggerOnEmailReceive(object sender, IMEmailEventArgs e)
		{
			if (onEmailReceive != null)
				onEmailReceive(sender, e);
		}
		protected void triggerOnChatRoomInvite(IMRoomInviteEventArgs e)
		{
			if (onChatRoomInvite != null)
				onChatRoomInvite(this, e);
		}

		// Variables
		protected ArrayList buddylist = new ArrayList();
		protected string mUsername = "";
		protected string mPassword = "";
		protected string protocolType = "";
		protected string mProtocolTypeShort = "";
		protected bool mEnabled;
		protected string mServer = "";
		protected bool blistChange;
		protected bool mConnected;
		protected string mStatusMessage;
		protected IMStatus mStatus;
		protected bool supportsMUC = false;
		protected bool supportsBuzz = false;
		protected bool supportsIntroMsg = false;
		protected bool supportsUserInvisiblity = false;
		protected bool needUsername = true;
		protected bool needPassword = true;
		protected bool savepassword = true;
		protected bool enableSaving = true;
		protected bool mIsIdle = false;
		protected string mAvatar;
		protected IMProtocolStatus status;
	}

	public class ChatRoomContainer
	{
		public ChatRoomContainer(csammisrun.OscarLib.ChatRoom room)
		{
			roomName = room.DisplayName;
			
			foreach (csammisrun.OscarLib.UserInfo user in room.Occupants)
			{
				occupants.Add(user.ScreenName);
			}

		}
		public ChatRoomContainer(string roomname, List<string> people)
		{
			roomName = roomname;
			occupants.InsertRange(0, people);
		}

		public List<string> Occupants
		{
			get {
				return occupants;
			}
		}
		public string RoomName
		{
			get {
				return roomName;
			}
		}
		public frmMUCChatWindow ChatWindow
		{
			get {
				return chatWindow;
			}
			set {
				chatWindow = value;
			}
		}

		// Variables
		private frmMUCChatWindow chatWindow = null;
		private string roomName = "";
		private Dictionary<string, IMBuddy> buddyMap = new Dictionary<string, IMBuddy>();
		private List<string> occupants = new List<string>();
	}
}