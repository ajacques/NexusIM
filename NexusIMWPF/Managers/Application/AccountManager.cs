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
			UserIdle.onUserIdle += new EventHandler(UserIdle_onChange);

			NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

			Trace.WriteLine("AccountManager loaded and ready");
		}

		public static void AddNewAccount(IMProtocol protocol)
		{
			AddNewAccount(new IMProtocolExtraData() { Protocol = protocol, Enabled = true, IsReady = true});
		}
		public static void AddNewAccount(IMProtocolExtraData extraData)
		{
			if (accounts.Contains(extraData))
				throw new ArgumentException("This protocol has already been added");

			extraData.IsReady = true;
			accounts.Add(extraData);

			if (extraData.Enabled && extraData.Protocol.ProtocolStatus == IMProtocolStatus.Offline)
			{
				extraData.Protocol.BeginLogin();
			}

			if (OnNewAccount != null)
				OnNewAccount(null, new NewAccountEventArgs(extraData.Protocol));
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
			IEnumerable<IMProtocolExtraData> protocols = accounts.Where(p => p.Enabled && p.Protocol.ProtocolStatus == IMProtocolStatus.Offline);

			if (!protocols.Any())
				return;

			string distinctp = protocols.Select(p => p.Protocol.ShortProtocol).Distinct().OrderBy(s => s).Aggregate((s, p) => s + ", " + p); // Creates a debug string that controls all the unique protocol types (ex. yahoo, aim, jabber)

			Trace.WriteLine("Connecting to " + protocols.Count() + " enabled account(s) (out of " + accounts.Count + ") (" + distinctp + ")");

			foreach (var protocol in protocols)
				protocol.Protocol.BeginLogin(); // For all the accounts that are enabled, login
		}

		/// <summary>
		/// Triggers a disconnect on all connected accounts
		/// </summary>
		public static void DisconnectAll()
		{
			accounts.Where(p => p.Protocol.ProtocolStatus == IMProtocolStatus.Online).Where(p => p.Enabled).ToList().ForEach(p => p.Protocol.Disconnect());

			if (onStatusChange != null)
				onStatusChange(null, null);
			mGeneralStatus = IMStatus.OFFLINE;
		}
		/// <summary>
		/// Searches through all accounts for a protocol 
		/// </summary>
		/// <param name="username">What username to search for</param>
		public static IMProtocol GetAccountByUsername(string username)
		{
			foreach (IMProtocolExtraData obj in accounts)
			{
				if (obj.Protocol.Username == username)
				{
					return obj.Protocol;
				}
			}
			throw new Exception("Not found");
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

				accounts.Where(p => p.Protocol.ProtocolStatus == IMProtocolStatus.Online).Where(p => p.Protocol.Status != IMStatus.OFFLINE).ToList().ForEach(p => p.Protocol.Status = mGeneralStatus);
			}
		}

		public static event EventHandler onInternetStatusChange;
		public static event EventHandler<StatusUpdateEventArgs> onStatusChange;
		public static event EventHandler<NewAccountEventArgs> OnNewAccount;

		public static bool IsConnectedToInternet()
		{
			return NetworkListManager.IsConnectedToInternet;
		}

		/// <summary>
		/// Returns a list of protocols currently setup by the user
		/// </summary>
		public static IEnumerable<IMProtocolExtraData> Accounts
		{
			get {
				return accounts;
			}
		}
		
		private static IMStatus mGeneralStatus = IMStatus.AVAILABLE;
		private static IMProtocolStatus mProtocolStatus = IMProtocolStatus.Offline;
		private static List<IMProtocolExtraData> accounts = new List<IMProtocolExtraData>();
		private static string statusmessage;
		private static int lastSid = 0;
		private static bool mStarted;
	}
}