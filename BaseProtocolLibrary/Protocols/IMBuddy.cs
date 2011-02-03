using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Runtime.Serialization;
using InstantMessage.Events;

namespace InstantMessage
{
	/// <summary>
	/// Stores information about a single contact
	/// </summary>
	[Serializable]
	public sealed partial class IMBuddy : IComparable<IMBuddy>, IContact, INotifyPropertyChanged, IHasPresence
	{
		public IMBuddy()
		{
			mProtocol = new IMProtocol();
		}
		/// <summary>
		/// Creates a new buddy
		/// </summary>
		/// <param name="protocol">Pointer to the protocol this buddy belongs to</param>
		/// <param name="username">This buddy's username</param>
		public IMBuddy(IMProtocol protocol, string username)
		{
			mUsername = username;
			mProtocol = protocol;
			mGuid = Guid.NewGuid();
		}
		/// <summary>
		/// Sends a message to this user
		/// </summary>
		/// <param name="message">Message contents</param>
		public void SendMessage(string message)
		{
			mProtocol.SendMessage(mUsername, message);
		}
		/// <summary>
		/// Used by the IMProtocol class to show a message that the user received from this buddy
		/// </summary>
		/// <param name="messages">What message to display</param>
		internal void InvokeReceiveMessage(string message)
		{
			if (onReceiveMessage != null)
				onReceiveMessage(this, new IMMessageEventArgs(this, message));
		}
		internal void ShowIsTypingMessage(bool isTyping)
		{
			//if (!windowOpen && Convert.ToBoolean(Settings.GetCustomSetting("psychicchat", "False")))
				//showWindow(false);
		}
		internal void ReceiveBuzz()
		{
			//if (!windowOpen)
				//showWindow(false);
		}

		public override string ToString()
		{
			return "Username: " + mUsername + " - Account: " + mProtocol.Protocol + " - " + mProtocol.Username;
		}

		// Properties
		/// <summary>
		/// Returns the Avatar of this buddy
		/// </summary>
		public BuddyAvatar Avatar
		{
			get {
				return mAvatar;
			}
			set {
				mAvatar = value;
				NotifyPropertyChanged("Avatar");
			}
		}
		/// <summary>
		/// Gets the protocol this buddy is associated with
		/// </summary>
		public IMProtocol Protocol
		{
			get	{
				return mProtocol;
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
				isOnline = value;
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
			internal set {
				if (value != mNickname)
				{
					mNickname = value;
					NotifyPropertyChanged("Nickname");
					NotifyPropertyChanged("DisplayName");
				}
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
		internal bool IsInternalBuddy
		{
			get {
				return mIsInternalUsage;
			}
			set {
				mIsInternalUsage = value;
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
			get	{
				return mNickname;
			}
			set	{
				mNickname = value;
			}
		}
		public IDictionary<string, string> Options
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
				if (value != mUsername)
				{
					mUsername = value;

					NotifyPropertyChanged("Username");
					NotifyPropertyChanged("DisplayName");
				}				
			}
		}
		public DateTime LocalTime
		{
			get	{
				return DateTime.Now;
			}
		}
		public string StatusMessage
		{
			get {
				return mStatusMessage;
			}
			set {
				if (mStatusMessage != value)
				{
					mStatusMessage = value;
					mStatusChange = DateTime.Now;

					NotifyPropertyChanged("StatusMessage");
				}				
			}
		}
		/// <summary>
		/// Returns when this buddy last changed their status
		/// </summary>
		public DateTime StatusChange
		{
			get	{
				return mStatusChange;
			}
		}
		public string Group
		{
			get {
				return mGroup;
			}
			set {
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
				if (mProtocol.ProtocolStatus == IMProtocolStatus.Online)
					mStatusChange = DateTime.Now;
				NotifyPropertyChanged("Status");

				if (onGlobalBuddyStatusChange != null)
					onGlobalBuddyStatusChange(this, null);
				if (onStatusChange != null)
					onStatusChange(this, null);
			}
		}
		/// <summary>
		/// The guid is used to identify this buddy
		/// </summary>
		public Guid Guid
		{
			get {
				return mGuid;
			}
		}
		public UserVisibilityStatus VisibilityStatus
		{
			get {
				return mVisibilityStatus;
			}
			set {
				mVisibilityStatus = value;
				mProtocol.SetPerUserVisibilityStatus(this.Username, value);
			}
		}

		public int CompareTo(IMBuddy right)
		{
			return mGuid.CompareTo(right.mGuid);
		}

		public static IMBuddy FromUsername(string username, IMProtocol source)
		{
			return (from p in source.ContactList where p.Username == username select new { p }).FirstOrDefault().p;
		}

		private void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		// Events
		/// <summary>
		/// Subscribe to this event to receive updates whenever any buddy has a status change.
		/// </summary>
		public static event EventHandler onGlobalBuddyStatusChange;
		public event EventHandler onSignOut;
		public event EventHandler onStatusChange;
		public event EventHandler onWindowOpen;
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<IMMessageEventArgs> onReceiveMessage;
		
		// Variables
		internal UserVisibilityStatus mVisibilityStatus;
		private string mUsername = "";
		private string mNickname = "";
		private BuddyAvatar mAvatar;
		private IMProtocol mProtocol = null;
		private bool isOnline = false;
		private bool mBlistItemVisible = false;
		private bool mIsMobileContact = false;
		private bool mIsInternalUsage = false;
		private bool mIsOnBuddyList = true;
		private Guid mGuid;
		private IMBuddyStatus mStatus = IMBuddyStatus.Offline;
		private string mStatusMessage = "";
		private DateTime mStatusChange;
		private string mGroup = "";
		private string mSMSnumber = "";
		private int mMessageLength = -1;
		private string buddyImageKey = "";
		private Dictionary<string, string> data = new Dictionary<string, string>();
	}
}