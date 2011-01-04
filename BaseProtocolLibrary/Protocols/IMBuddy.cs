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
using System.Runtime.Serialization;
using InstantMessage.Events;

namespace InstantMessage
{
	/// <summary>
	/// Stores information about a single contact
	/// </summary>
	[Serializable]
	public partial class IMBuddy : IComparable<IMBuddy>, IContact
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
			mGuid = Guid.NewGuid();
		}
		/// <summary>
		/// Opens the chat window for a buddy if it's not already open
		/// </summary>
		/// <param name="userinvoked">False if the window should be minimized after opening</param>
		[Obsolete("Windows are handled by ProtocolManager", false)]
		public void showWindow(bool userinvoked)
		{
			IMProtocol.CustomProtocolManager.OpenBuddyWindow(this, userinvoked);
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
		/// Used to create the ListViewItem then insert it into the frmMain Contact List
		/// </summary>
		public void Populate()
		{
			if (IMProtocol.CustomProtocolManager != null)
				IMProtocol.CustomProtocolManager.AddContactListItem(this);
		}
		/// <summary>
		/// Updates the ListViewItem
		/// </summary>
		public void UpdateListItem()
		{
			if (IMProtocol.CustomProtocolManager != null)
				IMProtocol.CustomProtocolManager.UpdateContactListItem(this);
		}
		/// <summary>
		/// Used by the IMProtocol class to show a message that the user received from this buddy
		/// </summary>
		/// <param name="messages">What message to display</param>
		public void ShowRecvMessage(string message)
		{
			if (onReceiveMessage != null)
				onReceiveMessage(this, new IMMessageEventArgs(this, message));
			if (IMProtocol.CustomProtocolManager != null)
				IMProtocol.CustomProtocolManager.ShowReceivedMessage(this, message);
		}
		public void ShowIsTypingMessage(bool isTyping)
		{
			//if (!windowOpen && Convert.ToBoolean(Settings.GetCustomSetting("psychicchat", "False")))
				//showWindow(false);
		}
		public void Buzz()
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
			}
		}
		public ContactListItem ContactItem
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
			get	{
				return mProtocol;
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
				if (mBlistItemVisible)
					UpdateListItem();
				else {
					if (IMProtocol.CustomProtocolManager != null)
						IMProtocol.CustomProtocolManager.RemoveContactListItem(this);
				}
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
		[Obsolete("Windows are handled by ProtocolManager")]
		public bool WindowOpen
		{
			get {
				return windowOpen;
			}
			set {
				if (!value)
				{
				} else {
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
			get	{
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
				mStatusChange = DateTime.Now;
				UpdateListItem();
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
				if (mProtocol.ProtocolStatus == IMProtocolStatus.ONLINE)
					mStatusChange = DateTime.Now;
				UpdateListItem();

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

		// Events
		/// <summary>
		/// Subscribe to this event to receive updates whenever any buddy has a status change.
		/// </summary>
		public static event EventHandler onGlobalBuddyStatusChange;
		[Obsolete("Use onStatusChange instead", false)]
		public event EventHandler onSignIn;
		[Obsolete("Use onStatusChange instead", false)]
		public event EventHandler onSignOut;
		public event EventHandler onStatusChange;
		public event EventHandler onWindowOpen;
		public event EventHandler<IMMessageEventArgs> onReceiveMessage;
		
		// Variables
		internal UserVisibilityStatus mVisibilityStatus;
		protected string mUsername = "";
		protected string mNickname = "";
		protected BuddyAvatar mAvatar;
		protected IMProtocol mProtocol = null;
		protected ContactListItem mBListItem = null;
		protected bool windowOpen = false; // Is the window currently open
		protected bool isOnline = false;
		protected bool mBlistItemVisible = false;
		protected bool mIsManaged = false;
		protected bool mIsMobileContact = false;
		protected bool mIsInternalUsage = false;
		protected bool mAcceptsMessages = true;
		protected bool mIsOnBuddyList = true;
		protected Guid mGuid;
		protected IMBuddyStatus mStatus = IMBuddyStatus.Offline;
		protected string mStatusMessage = "";
		protected DateTime mStatusChange;
		protected string mGroup = "";
		protected string mSMSnumber = "";
		protected int itemSortIndex = 0;
		protected int mMessageLength = -1;
		protected string buddyImageKey = "";
		protected Dictionary<string, string> data = new Dictionary<string, string>();
	}
}