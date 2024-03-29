﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InstantMessage.Events;
using InstantMessage.Protocols;

namespace InstantMessage
{
	/// <summary>
	/// Stores information about a single contact
	/// </summary>
	public sealed partial class IMBuddy : IContact, INotifyPropertyChanged, IComparable<IMBuddy>, IComparable
	{
		public IMBuddy()
		{
			//mProtocol = new IMProtocol();
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
		}
		/// <summary>
		/// Sends a message to this user
		/// </summary>
		/// <param name="message">Message contents</param>
		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
			mProtocol.SendMessage(this.Username, message);
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
				return Status != IMBuddyStatus.Offline && Status != IMBuddyStatus.Unknown;
			}
		}
		public string DisplayName
		{
			get {
				if (String.IsNullOrEmpty(mNickname))
					return mUsername;
				else
					return mNickname;
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
		/// <summary>
		/// Gets or sets the nickname to use for this buddy
		/// </summary>
		public string Nickname
		{
			get	{
				return mNickname;
			}
			set	{
				if (mNickname != value)
				{
					mNickname = value;

					NotifyPropertyChanged("Nickname");
					NotifyPropertyChanged("DisplayName");
				}
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
				if (mStatus != value)
				{
					mStatus = value;
					NotifyPropertyChanged("Status");

					if (onStatusChange != null)
						onStatusChange(this, null);
				}				
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
		
		public static IMBuddy FromUsername(string username, IMProtocol source)
		{
			return (IMBuddy)(from p in source.ContactList where p.Value.Username == username select p.Value).FirstOrDefault();
		}

		public bool Equals(IContact right)
		{
			if (right == null)
				throw new ArgumentNullException("contact");

			return object.ReferenceEquals(Protocol, right.Protocol) && Username.Equals(right.Username);
		}
		public int CompareTo(IMBuddy other)
		{
			if (Protocol != other.Protocol)
				return Protocol.CompareTo(other.Protocol);

			return Username.CompareTo(other.Username);
		}
		public int CompareTo(object other)
		{
			if (other.GetType() != typeof(IMBuddy))
				return -1;

			return CompareTo((IMBuddy)other);
		}

		private void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		// Events
		public event EventHandler onStatusChange;
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<IMMessageEventArgs> onReceiveMessage;
		
		// Variables
		internal UserVisibilityStatus mVisibilityStatus;
		private string mUsername;
		private string mNickname;
		private BuddyAvatar mAvatar;
		private IMProtocol mProtocol;
		private bool mIsMobileContact;
		private bool mIsOnBuddyList = true;
		private IMBuddyStatus mStatus = IMBuddyStatus.Offline;
		private string mStatusMessage;
		private DateTime mStatusChange;
		private string mGroup;
		private int mMessageLength = -1;
		private Dictionary<string, string> data = new Dictionary<string, string>();
	}
}