using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using InstantMessage;
using InstantMessage.Events;
using Microsoft.WindowsAPICodePack.Net;

namespace NexusIM.Managers
{
	enum GlobalErrorReasons
	{
		NotConnectedToNetwork,
		Unknown
	}
	class GlobalErrorEventArgs : EventArgs
	{
		public GlobalErrorEventArgs(GlobalErrorReasons reason)
		{
			mReason = reason;
		}
		public GlobalErrorReasons Reason
		{
			get {
				return mReason;
			}
		}
		private GlobalErrorReasons mReason;
	}
	class StatusUpdateEventArgs : EventArgs
	{
		public StatusUpdateEventArgs(int sId)
		{
			statusId = sId;
		}
		public int StatusUpdateId
		{
			get	{
				return statusId;
			}
		}
		private int statusId;
	}
	class NewAccountEventArgs : EventArgs
	{
		public NewAccountEventArgs(IMProtocol protocol)
		{
			Account = protocol;
		}
		public IMProtocol Account
		{
			get;
			private set;
		}
	}
	/// <summary>
	/// Controls the status and all registered protocols
	/// </summary>
	internal static class AccountManager
	{
		public static void Setup()
		{
			IMProtocol.AnyLoginCompleted += new EventHandler(Protocol_OnLogin);
			IMProtocol.onError += new EventHandler<IMErrorEventArgs>(Protocol_OnError);
			UserIdle.onUserIdle += new EventHandler(UserIdle_onChange);

			NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

			Trace.WriteLine("AccountManager loaded and ready");
		}

		public static void AddNewAccount(IMProtocol protocol)
		{
			if (accounts.Contains(protocol))
				throw new ArgumentException("This protocol has already been added");

			accounts.Add(protocol);

			if (protocol.Enabled && protocol.ProtocolStatus == IMProtocolStatus.Offline)
			{
				protocol.BeginLogin();
			}

			if (OnNewAccount != null)
				OnNewAccount(null, new NewAccountEventArgs(protocol));
		}
		/// <summary>
		/// Begins an instant messaging session
		/// </summary>
		public static void Start()
		{
			if (mStarted)
				return;

			Trace.WriteLine("AccountManager is now started and ready for events");

			mStarted = true; // We are now handling all events and triggers

			Trace.WriteLine("Connectivity Status: " + NetworkListManager.Connectivity.ToString());

			if (IsConnectedToInternet()) // Don't bother trying if we aren't connected
			{
				mProtocolStatus = IMProtocolStatus.Connecting;

				TriggerNewLoginEvent();
			} else
				Trace.WriteLine("Not connected to the internet. Waiting for connection alert.");
		}
		public static void TriggerNewLoginEvent()
		{
			IEnumerable<IMProtocol> protocols = accounts.Where(p => p.Enabled && p.ProtocolStatus == IMProtocolStatus.Offline);

			if (!protocols.Any())
				return;

			string distinctp = protocols.Select(p => p.ShortProtocol).Distinct().OrderBy(s => s).Aggregate((s, p) => s + ", " + p); // Creates a debug string that controls all the unique protocol types (ex. yahoo, aim, jabber)

			Trace.WriteLine("Connecting to " + protocols.Count() + " enabled account(s) (out of " + accounts.Count + ") (" + distinctp + ")");

			foreach (var protocol in protocols)
				protocol.BeginLogin(); // For all the accounts that are enabled, login
		}

		/// <summary>
		/// Triggers a disconnect on all connected accounts
		/// </summary>
		public static void DisconnectAll()
		{
			accounts.Where(p => p.ProtocolStatus == IMProtocolStatus.Online).Where(p => p.Enabled).ToList().ForEach(p => p.Disconnect());

			if (onStatusChange != null)
				onStatusChange(null, null);
			mGeneralStatus = IMStatus.OFFLINE;
		}
		/// <summary>
		/// Searches all protocols for a requested buddy
		/// </summary>
		/// <param name="username">What username to search for</param>
		/// <returns>The requested buddy</returns>
		public static IMBuddy GetByName(string username)
		{
			foreach (IMProtocol obj in accounts)
			{
				foreach (IMBuddy buddy in obj.ContactList)
				{
					if (buddy.Username == username)
					{
						return buddy;
					}
				}
			}
			throw new Exception("Failed to find user");
		}
		/// <summary>
		/// Searches a single protocol for a requested buddy
		/// </summary>
		/// <param name="username">What username to search for</param>
		/// <param name="protocol">What protocol to search</param>
		/// <returns>The requested buddy</returns>
		public static IMBuddy GetByName(string username, IMProtocol protocol)
		{
			foreach (IMBuddy buddy in protocol.ContactList)
			{
				if (buddy.Username == username)
				{
					return buddy;
				}
			}
			throw new Exception("Failed to find user");
		}
		/// <summary>
		/// Searches through all accounts for a protocol 
		/// </summary>
		/// <param name="username">What username to search for</param>
		public static IMProtocol GetAccountByUsername(string username)
		{
			foreach (IMProtocol obj in accounts)
			{
				if (obj.Username == username)
				{
					return obj;
				}
			}
			throw new Exception("Not found");
		}
		public static IEnumerable<IMBuddy> MergeAllBuddyLists()
		{
			// First, Take the contact list from each protocol
			// next, we use the aggregate function on each contact list to concat or combine it with the next one
			// this gives us all the contacts in all the protocols
			return accounts.SelectMany(p => p.ContactList);
		}

		// Event Callbacks
		private static void Protocol_OnError(object sender, IMErrorEventArgs e)
		{
			accountsInError.Add((IMProtocol)sender);

			if (onGlobalError == null)
				return;
			
			if (accountsInError.Count == accounts.Count && accounts.Count >= 1)
			{
				onGlobalError(null, new GlobalErrorEventArgs(GlobalErrorReasons.NotConnectedToNetwork)); // We have confirmation that all networks are currently down
			} else {
				onProtocolError(null, e);
			}
		}
		private static void Protocol_OnLogin(object sender, EventArgs e)
		{
			connectingAccs.Remove((IMProtocol)sender);
			if (connectingAccs.Count == 0 && onAllFinishConnecting != null)
				onAllFinishConnecting(null, null);
		}
		private static void SignedOff_OtherClient_Relogin(object sender, EventArgs e)
		{
			IEnumerator myEnum = accountsInError.GetEnumerator();
			while (myEnum.MoveNext())
			{
				IMProtocol obj = (IMProtocol)(myEnum.Current);
				obj.BeginLogin();
			}
			accountsInError.Clear();
		}
		private static void Nic_AvailabilityChange(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine("AccountManager: NIC Availability Change (is available: " + e.IsAvailable + ")");
		}
		private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine("AccountManager: Network Availability Change (is available: " + e.IsAvailable + ")");

			if (onInternetStatusChange != null)
				onInternetStatusChange(null, null);
		}
		private static void UserIdle_onChange(object sender, EventArgs e)
		{

		}

		public static string StatusMessage
		{
			get {
				return statusmessage;
			}
			set {
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// Changes the status for all accounts to the specified status.
		/// </summary>
		public static IMStatus Status
		{
			get {
				return mGeneralStatus;
			}
			set	{
				mGeneralStatus = value;

				accounts.Where(p => p.ProtocolStatus == IMProtocolStatus.Online).Where(p => p.Status != IMStatus.OFFLINE).ToList().ForEach(p => p.Status = mGeneralStatus);
			}
		}

		public static event EventHandler onInternetStatusChange;
		public static event EventHandler<StatusUpdateEventArgs> onStatusChange;
		public static event EventHandler<GlobalErrorEventArgs> onGlobalError;
		public static event EventHandler<IMErrorEventArgs> onProtocolError;
		public static event EventHandler onAllFinishConnecting;
		public static event EventHandler<NewAccountEventArgs> OnNewAccount;

		public static bool IsConnectedToInternet()
		{
			return NetworkListManager.IsConnectedToInternet;
		}

		/// <summary>
		/// Returns a list of protocols currently setup by the user
		/// </summary>
		public static IEnumerable<IMProtocol> Accounts
		{
			get {
				return accounts;
			}
		}
		
		private static IMStatus mGeneralStatus = IMStatus.AVAILABLE;
		private static IMProtocolStatus mProtocolStatus = IMProtocolStatus.Offline;
		private static List<IMProtocol> accounts = new List<IMProtocol>();
		private static List<IMProtocol> accountsInError = new List<IMProtocol>();
		private static Dictionary<IMProtocol, int> accErrorCount = new Dictionary<IMProtocol, int>();
		private static Dictionary<IMErrorEventArgs.ErrorReason, IMProtocol> accErrorReason = new Dictionary<IMErrorEventArgs.ErrorReason, IMProtocol>();
		private static List<IMProtocol> connectingAccs = new List<IMProtocol>();
		private static string statusmessage;
		private static int lastSid = 0;
		private static bool mStarted = false;
	}
}