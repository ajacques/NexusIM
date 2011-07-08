using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using InstantMessage.Events;
using InstantMessage.Protocols;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace InstantMessage
{
	/// <summary>
	/// Used to identify what status to be reported to the protocol servers
	/// </summary>
	public enum IMStatus
	{
		Available,
		Away,
		Busy,
		Idle,
		Invisible
	}

	/// <summary>
	/// Used for internal purposes to know what status a protocol is in
	/// </summary>
	public enum IMProtocolStatus
	{
		/// <summary>
		/// This account is currently online and connected
		/// </summary>
		Online,
		/// <summary>
		/// This account is currently connecting to the server
		/// </summary>
		Connecting,
		/// <summary>
		/// This account is not connected
		/// </summary>
		Offline
	}

	/// <summary>
	/// Used for internal purposes to know what status a buddy is
	/// </summary>
	public enum IMBuddyStatus
	{
		Available,
		Away,
		Busy,
		Idle,
		Offline,
		Unknown,
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
	[IMNetwork("default")]
	public class IMProtocol : INotifyPropertyChanged, IComparable<IMProtocol>, IProtocol
	{
		public IMProtocol()
		{
			mProtocolStatus = IMProtocolStatus.Offline;
			Username = "";
		}
		/// <summary>
		/// Connects and authenticates with the server
		/// </summary>
		public virtual void BeginLogin()
		{
			mProtocolStatus = IMProtocolStatus.Connecting;
			mLoginWaitHandle = new ManualResetEvent(false);
		}
		/// <summary>
		/// Waits for the login process to complete, then continues
		/// </summary>
		public void EndLogin()
		{
			LoginWaitHandle.WaitOne();

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
		public virtual void Disconnect()
		{
			triggerOnDisconnect(new IMDisconnectEventArgs(DisconnectReason.User));

			mProtocolStatus = IMProtocolStatus.Offline;
		}
		/// <summary>
		/// Adds a person to the user's contact list
		/// </summary>
		/// <param name="name">The user to add</param>
		/// <param name="nickname">Nickname to use</param>
		/// <param name="group">Which group to place the buddy into</param>
		public virtual void AddFriend(string name, string nickname, string group) {}
		public virtual void AddFriend(string name, string nickname, string group, string introMsg = null) {}
		public virtual void RemoveFriend(string uname) {}
		/// <summary>
		/// Sends a private message to another user
		/// </summary>
		/// <param name="friendName">What person to send the message to</param>
		/// <param name="message">The contents of the message</param>
		public virtual void SendMessage(string friendName, string message)
		{
		}
		public virtual void SendMessageToRoom(string roomName, string message) {}
		protected virtual void OnStatusChange(IMStatus oldStatus, IMStatus newStatus) {}
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

		protected bool HasPassword()
		{
			return mPasswordEnc != null && !String.IsNullOrEmpty(mPassword);
		}

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
		public int CompareTo(IMProtocol other)
		{
			if (Protocol != other.Protocol)
				return Protocol.CompareTo(other.Protocol);

			return Username.CompareTo(other.Username);
		}
		public ContactCollection ContactList
		{
			get {
				return buddylist;
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
				if (mStatus != value)
				{
					if (ProtocolStatus == IMProtocolStatus.Online)
						OnStatusChange(mStatus, value);
					mStatus = value;
				}				
			}
		}
		
		/// <summary>
		/// True if this protocol is currently connected to the server
		/// </summary>
		public bool Connected
		{
			get;
			protected set;
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
				if (mUsername != value)
				{
					mUsername = value;

					NotifyPropertyChanged("Username");
				}
			}
		}
		/// <summary>
		/// Gets or sets the password used by the protocol
		/// </summary>
		public string Password
		{
			get {
				return mPassword;

				if (mPasswordEnc == null)
					return null;

				ProtectedMemory.Unprotect(mPasswordEnc, MemoryProtectionScope.SameProcess);

				return Encoding.Default.GetString(mPasswordEnc, 0, mPasswordEnc.Length - mPaddingAmount);
				//return mPassword;
			}
			set {
				if (mPassword != value)
				{
					mPassword = value;

					string input = value;

					if (input.Length % 16 != 0)
					{
						mPaddingAmount = (byte)(input.Length + (16 - (input.Length % 16)));
						input = input.PadRight(mPaddingAmount);
					}

					mPasswordEnc = Encoding.Default.GetBytes(input);

					ProtectedMemory.Protect(mPasswordEnc, MemoryProtectionScope.SameProcess);

					//mPassword = Encoding.Default.GetString(passArr);

					NotifyPropertyChanged("Password");
				}		
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
		/// Server string to connect to when logging in
		/// </summary>
		public string Server
		{
			get {
				return mServer;
			}
			set {
				if (mServer != value)
				{
					mServer = value;

					NotifyPropertyChanged("Server");
				}
			}
		}
		public object Tag
		{
			get;
			set;
		}
		
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		/// <summary>
		/// Gets the current internal protocol status
		/// </summary>
		public IMProtocolStatus ProtocolStatus
		{
			get {
				return mProtocolStatus;
			}
		}
#if !SILVERLIGHT
		/// <summary>
		/// Creates a IMProtocol from the passed string
		/// </summary>
		/// <param name="name">The protocol type string</param>
		/// <returns>Your fancy new IMProtocol</returns>
		public static IMProtocol FromString(string name)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					IMNetworkAttribute attrib = type.GetCustomAttributes(typeof(IMNetworkAttribute), true).FirstOrDefault() as IMNetworkAttribute;
					if (attrib != null && attrib.ShortName == name)
						return Activator.CreateInstance(type) as IMProtocol;
				}
			}
			return null;
		}
#endif
		[Obsolete("", false)]
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
			buddylist.Clear();
		}
		public EventWaitHandle LoginWaitHandle
		{
			get {
				return mLoginWaitHandle;
			}
		}
		public IDictionary<string, string> ConfigurationSettings
		{
			get {
				return mConfig;
			}
			set {
				mConfig = value;
			}
		}

		// Events
		public static event EventHandler AnyLoginCompleted;
		public static event EventHandler<IMErrorEventArgs> AnyErrorOccurred;
		public static event EventHandler<IMFriendRequestEventArgs> onFriendRequest;
		public static event EventHandler<IMRoomInviteEventArgs> onChatRoomInvite;
		public static event EventHandler<IMEmailEventArgs> onEmailReceive;
		public static event EventHandler<IMMessageEventArgs> AnyMessageReceived;
		public event EventHandler LoginCompleted;
		public event EventHandler<IMErrorEventArgs> ErrorOccurred;
		public event EventHandler<IMDisconnectEventArgs> onDisconnect;
		public event EventHandler<IMFriendEventArgs> ContactStatusChange;
		public event EventHandler<IMMessageEventArgs> onMessageReceive;
		public event PropertyChangedEventHandler PropertyChanged;

		// Internal Event Triggers
		protected void OnLogin()
		{
			LoginWaitHandle.Set();

			try	{
				if (IMProtocol.AnyLoginCompleted != null)
					IMProtocol.AnyLoginCompleted(this, null);
			} catch (Exception) {}

			try	{
				if (this.LoginCompleted != null)
					this.LoginCompleted(this, null);
			} catch (Exception) {}
		}
		protected void triggerOnError(IMErrorEventArgs e)
		{
			Debug.WriteLine("Protocol Error: " + ToString() + " - " + e.Reason.ToString());
			this.mProtocolStatus = IMProtocolStatus.Offline;

			if (ErrorOccurred != null)
				ErrorOccurred(this, e);
			if (IMProtocol.AnyErrorOccurred != null)
				IMProtocol.AnyErrorOccurred(this, e);
		}
		protected void triggerOnDisconnect(IMDisconnectEventArgs e)
		{
			if (onDisconnect != null)
				onDisconnect(this, e);
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
		protected void triggerOnMessageReceive(IMMessageEventArgs e)
		{
			if (AnyMessageReceived != null)
				AnyMessageReceived(this, e);
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
		protected void triggerBadCredentialsError()
		{
			if (ErrorOccurred != null)
				ErrorOccurred(this, new BadCredentialsEventArgs());

			mLoginWaitHandle.Set();
		}

		// Variables
		protected IDictionary<string, string> mConfig = new Dictionary<string, string>();
		protected ManualResetEvent mLoginWaitHandle;
		protected ContactCollection buddylist = new ContactCollection();
		protected string mUsername;
		protected string mPassword;
		protected byte[] mPasswordEnc;
		protected byte mPaddingAmount;
		protected string protocolType = "Default";
		protected string mProtocolTypeShort = "default";
		protected string mServer;
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
		protected IMProtocolStatus mProtocolStatus;
		protected Exception mLoginException;
	}
}