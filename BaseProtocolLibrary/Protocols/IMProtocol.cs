using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Xml;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Security.Authentication;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace InstantMessage
{
	/// <summary>
	/// This class is a package class that transports data from the ProtocolLibrary to the UI
	/// </summary>
	[Obsolete("Use IMBuddy + IMProtocol Events", false)]
	public class ContactListItem
	{
		public string DisplayText
		{
			get	{
				return mDisplayText;
			}
			set	{
				mDisplayText = value;
			}
		}
		[Obsolete("Use the Buddy Property", false)]
		public string StatusMessage
		{
			get	{
				return mStatusText;
			}
			set	{
				mStatusText = value;
			}
		}
		public IMBuddyStatus Status
		{
			get	{
				return mStatus;
			}
			set	{
				mStatus = value;
			}
		}
		public IMBuddy Buddy
		{
			get	{
				return mBuddy;
			}
			set	{
				mBuddy = value;
			}
		}
		private string mDisplayText = "";
		private string mStatusText = "";
		private IMBuddy mBuddy;
		private IMBuddyStatus mStatus;
	}

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
		OFFLINE,
		OnThePhone
	}

	/// <summary>
	/// Used for internal purposes to know what status a protocol is in
	/// </summary>
	public enum IMProtocolStatus
	{
		/// <summary>
		/// This account is currently online and connected
		/// </summary>
		ONLINE,
		/// <summary>
		/// This account is currently connecting to the server
		/// </summary>
		CONNECTING,
		/// <summary>
		/// This account is has encountered an error and is no-longer connected
		/// </summary>
		ERROR,
		/// <summary>
		/// This account is not connected
		/// </summary>
		OFFLINE
	}

	/// <summary>
	/// Used for internal purposes to know what status a buddy is
	/// </summary>
	public enum IMBuddyStatus
	{
		Available,
		Busy,
		Away,
		Idle,
		Offline,
	}

	/// <summary>
	/// Used to set how the user shows up to another user
	/// </summary>
	public enum UserVisibilityStatus
	{
		/// <summary>
		/// Show up as online to this user
		/// </summary>
		Online,
		/// <summary>
		/// Show up as offline to this user
		/// </summary>
		Offline,
		/// <summary>
		/// Show up as permanently offline to this user
		/// </summary>
		Permanently_Offline
	}

	/// <summary>
	/// Stores information and handles communication for specific IM networks
	/// </summary>
	public class IMProtocol
	{
		public IMProtocol()
		{
			status = IMProtocolStatus.OFFLINE;
			mStatus = IMStatus.OFFLINE;
			mEnabled = false;
		}
		/// <summary>
		/// Connects and authenticates with the server
		/// </summary>
		public virtual void BeginLogin()
		{
			status = IMProtocolStatus.CONNECTING;
		}
		/// <summary>
		/// Waits for the login process to complete, then continues
		/// </summary>
		public void EndLogin()
		{
			LoginWaitHandle.WaitOne(1000);

			if (mLoginException != null)
			{
				Exception duplicate = mLoginException;
				mLoginException = null;
				throw duplicate;
			}
		}
		/// <summary>
		/// Disconnects from the server
		/// </summary>
		public virtual void Disconnect() {}
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
		[Obsolete("Group stuff will be handled internally by IMProtocol", false)]
		public virtual void RemoveFriend(string uname, string group) {}
		public virtual void RemoveFriend(string uname)
		{
			RemoveFriend(uname, IMBuddy.FromUsername(uname, this).Group);
		}
		/// <summary>
		/// Sends a private message to another user
		/// </summary>
		/// <param name="friendName">What person to send the message to</param>
		/// <param name="message">The contents of the message</param>
		public virtual void SendMessage(string friendName, string message)
		{
			IMSendMessageEventArgs args = new IMSendMessageEventArgs(this, IMBuddy.FromUsername(friendName, this), message);
			triggerOnSendMessage(args);
			if (args.Handled)
				throw new Exception("SendMessage Handled()");
		}
		public virtual void SendMessage(IMBuddy buddy, string message)
		{
			SendMessage(buddy, message, false);
		}
		[Obsolete("Use SendMessage(string, string) or SendMessage(IMBuddy, string) instead. Alternate addresses are handled internally by the protocol", false)]
		public virtual void SendMessage(IMBuddy buddy, string message, bool usesms)
		{
			SendMessage(buddy.Username, message);
		}
		public virtual void SendMessageToRoom(string roomName, string message) {}
		/// <summary>
		/// Changes the users status to reflect their current status
		/// </summary>
		/// <param name="status">What status to switch to</param>
		[Obsolete("This is totally broken. Avoid at all costs!!!!!!", false)]
		public virtual void ChangeStatus(IMStatus newstatus)
		{
			if (!mEnabled)
				return; // If it's not even enabled

			if (newstatus == mStatus)
				return;

			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus))
				BeginLogin();
			else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus))
				Disconnect();

			if (IsOnlineStatus(newstatus))
				mEnabled = true;
			else
				mEnabled = false;

			mStatus = newstatus;
		}
		public virtual void UpdateStatus()
		{
			if (!mEnabled)
				return;

			if (mNewStatus == mStatus)
				return;

			if (IsOnlineStatus(mNewStatus) && !IsOnlineStatus(mStatus))
				BeginLogin();
			else if (!IsOnlineStatus(mNewStatus) && IsOnlineStatus(mStatus))
				Disconnect();
		}
		/// <summary>
		/// Sends a notification to a contact that the user is currently a typing a message to them
		/// </summary>
		/// <param name="uname">Which contact to the send the notification to</param>
		/// <param name="isTyping">Is the user currently typing a message?</param>
		public virtual void IsTyping(string uname, bool isTyping) {}
		public virtual void SetPerUserVisibilityStatus(string buddy, UserVisibilityStatus status)
		{
			throw new NotSupportedException();
		}
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
		public override bool Equals(object obj)
		{
			IMProtocol prot = (IMProtocol)obj;
			if (mUsername == prot.mUsername && protocolType == prot.protocolType)
				return true;
			else
				return false;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + mUsername.GetHashCode() + mProtocolTypeShort.GetHashCode();
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
		public ObservableCollection<IMBuddy> ContactList
		{
			get {
				return buddylist;
			}
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
		/// <summary>
		/// Gets or sets the status that is reported to the im server
		/// </summary>
		public IMStatus Status
		{
			get {
				return mStatus;
			}
			set {
				mNewStatus = value;

				UpdateStatus();
			}
		}
		
		/// <summary>
		/// True if this protocol is currently connected to the server
		/// </summary>
		[Obsolete("Use ProtocolStatus instead", false)]
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
				/*byte[] passArr = Encoding.Default.GetBytes(mPassword);

				ProtectedMemory.Unprotect(passArr, MemoryProtectionScope.SameProcess);

				return Encoding.Default.GetString(passArr).TrimEnd(new char[] { ' ' });*/
				return mPassword;
			}
			set {
				mPassword = value;
				/*string input = value;

				if (input.Length % 16 != 0)
					input = input.PadRight(input.Length + (16 - (input.Length % 16)));

				byte[] passArr = Encoding.Default.GetBytes(input);

				ProtectedMemory.Protect(passArr, MemoryProtectionScope.SameProcess);

				mPassword = Encoding.Default.GetString(passArr);*/
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
		[Obsolete("Shouldn't be handled by the generic library", false)]
		public bool EnableSaving
		{
			get {
				return enableSaving;
			}
			set {
				enableSaving = value;
			}
		}
		public Guid Guid
		{
			get {
				return mGuid;
			}
			set {
				mGuid = value;
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
			var protocol = from t in Assembly.GetCallingAssembly().GetTypes()
						   let attribs = t.GetCustomAttributes(typeof(IMNetworkAttribute), true)
						   where attribs != null && attribs.Length > 0
						   select t;


			name = name.ToLower();
			if (name == "yahoo")
				return new IMYahooProtocol();
			else if (CustomProtocolManager != null)
				return CustomProtocolManager.CreateCustomProtocol(name);
			else
				return null;
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
		/// <summary>
		/// Removes all contact items from the internal list. Used when disconnecting to prepare for the next connection
		/// </summary>
		protected void CleanupBuddyList()
		{
			foreach (IMBuddy buddy in buddylist)
				buddy.ContactItemVisible = false;

			buddylist.Clear();
		}
		[Obsolete("Use IMProtocol Events", false)]
		public static IProtocolManager CustomProtocolManager
		{
			get {
				return mCustomManager;
			}
			set {
				mCustomManager = value;
			}
		}
		public ManualResetEvent LoginWaitHandle
		{
			get {
				return mLoginWaitHandle;
			}
		}
		public ObservableDictionary<string, string> ConfigurationSettings
		{
			get {
				return mConfig;
			}
		}

		// Events
		public static event EventHandler onLogin;
		public static event EventHandler<IMErrorEventArgs> onError;
		public static event EventHandler<IMDisconnectEventArgs> onDisconnect;
		public static event EventHandler<IMFriendRequestEventArgs> onFriendRequest;
		[Obsolete("Use ContactStatusChange", false)]
		public static event EventHandler<IMFriendEventArgs> onFriendSignIn;
		[Obsolete("Use ContactStatusChange", false)]
		public static event EventHandler<IMFriendEventArgs> onFriendSignOut;
		public static event EventHandler<IMRoomInviteEventArgs> onChatRoomInvite;
		public event EventHandler<IMFriendEventArgs> ContactStatusChange;
		public event EventHandler<IMMessageEventArgs> onMessageReceive;
		public static event EventHandler<IMMessageEventArgs> onMessageSend;
		public static event EventHandler<IMEmailEventArgs> onEmailReceive;
		public static event EventHandler<IMSendMessageEventArgs> onSendMessage;

		// Internal Event Triggers
		protected void triggerOnLogin(EventArgs e)
		{
			try	{
				if (onLogin != null)
					onLogin(this, e);
			} catch (Exception ex) {
				
			}
		}
		protected void triggerOnError(IMErrorEventArgs e)
		{
			Trace.TraceError("Protocol Error: " + e.Message + " - " + e.Reason.ToString());
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
		protected void triggerContactStatusChange(IMFriendEventArgs e)
		{
			if (ContactStatusChange != null)
				ContactStatusChange(this, e);
		}
		protected void triggerOnFriendSignOut(IMFriendEventArgs e)
		{
			if (onFriendSignOut != null)
				onFriendSignOut(this, e);
		}
		protected void triggerOnMessageReceive(IMMessageEventArgs e)
		{
			if (onMessageReceive != null)
				onMessageReceive(this, e);
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
		protected void triggerOnSendMessage(IMSendMessageEventArgs e)
		{
			if (onSendMessage != null)
				onSendMessage(this, e);
		}
		protected void triggerBadCredentialsError()
		{
			if (onError != null)
				onError(this, new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_USERNAME, "Invalid login credentials"));

			mLoginException = new InvalidCredentialException();
			mLoginWaitHandle.Set();
		}

		// Classes
		[Obsolete("No", false)]
		protected class PacketEventArgs : EventArgs
		{
			public PacketEventArgs(string data)
			{
				mData = data;
			}
			public string PacketData
			{
				get	{
					return mData;
				}
			}
			private string mData = "";
		}

		// Variables
		protected ObservableDictionary<string, string> mConfig = new ObservableDictionary<string, string>();
		protected ManualResetEvent mLoginWaitHandle;
		protected ObservableCollection<IMBuddy> buddylist = new ObservableCollection<IMBuddy>();
		protected string mUsername;
		protected string mPassword;
		protected string protocolType;
		protected string mProtocolTypeShort;
		protected bool mEnabled;
		protected string mServer;
		protected bool blistChange;
		protected bool mConnected;
		protected string mStatusMessage;
		protected IMStatus mStatus;
		protected IMStatus mNewStatus; // This is a new status awaiting to be applied
		protected bool supportsMUC;
		protected bool supportsBuzz;
		protected bool supportsIntroMsg;
		protected bool supportsUserInvisiblity;
		protected bool needUsername;
		protected bool needPassword;
		protected bool savepassword;
		protected bool enableSaving;
		protected bool mIsIdle;
		protected Guid mGuid;
		protected string mAvatar;
		protected static IProtocolManager mCustomManager;
		protected IMProtocolStatus status;
		protected Exception mLoginException;
	}

	public class ChatRoomContainer
	{
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
		public object ChatWindow
		{
			get {
				return chatWindow;
			}
			set {
				chatWindow = value;
			}
		}

		// Variables
		private object chatWindow = null;
		private string roomName = "";
		private Dictionary<string, IMBuddy> buddyMap = new Dictionary<string, IMBuddy>();
		private List<string> occupants = new List<string>();
	}
}