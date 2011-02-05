using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using InstantMessage;
using Microsoft.WindowsAPICodePack.Net;
using System.Reflection;

namespace NexusIM.Managers
{
	class StatusUpdateEventArgs : EventArgs
	{
		public IMStatus NewStatus
		{
			get;
			internal set;
		}
		public IMStatus OldStatus
		{
			get;
			internal set;
		}
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
			accounts = new List<IMProtocolExtraData>();
			UserIdle.onUserIdle += new EventHandler(UserIdle_onChange);

			NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

			Trace.WriteLine("AccountManager loaded and ready");
			Trace.WriteLine("Connectivity Status: " + NetworkListManager.Connectivity.ToString());
			mConnected = true;
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

			if (Connected && extraData.Enabled && extraData.Protocol.ProtocolStatus == IMProtocolStatus.Offline)
			{
				extraData.Protocol.BeginLogin();
			}

			if (OnNewAccount != null)
				OnNewAccount(null, new NewAccountEventArgs(extraData.Protocol));
		}
		public static void RemoveAccount(IMProtocolExtraData extraData)
		{
			extraData.Protocol.Disconnect();
			accounts.Remove(extraData);
		}

		// Event Handlers
		private static void Nic_AvailabilityChange(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine("AccountManager: NIC Availability Change (is available: " + e.IsAvailable + ")");
		}
		private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine("AccountManager: Network Availability Change (is available: " + e.IsAvailable + ")");
		}
		private static void UserIdle_onChange(object sender, EventArgs e)
		{

		}
		
		public static event EventHandler<StatusUpdateEventArgs> StatusChanged;
		public static event EventHandler<NewAccountEventArgs> OnNewAccount;
		public static event PropertyChangedEventHandler PropertyChanged;

		private static void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
		}

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
		public static string StatusMessage
		{
			get {
				throw new NotImplementedException();
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
				if (mGeneralStatus != value)
				{
					IMStatus oldStatus = mGeneralStatus;
					mGeneralStatus = value;
#if DEBUG
					StackTrace trace = new StackTrace();
					Debug.WriteLine(String.Format("AccountManager: Status Change ({0} to {1}) requested by {2}.{3}", oldStatus, value, trace.GetFrame(1).GetMethod().DeclaringType.FullName, trace.GetFrame(1).GetMethod().Name));
#endif

					foreach (IMProtocolExtraData account in accounts)
					{
						if (!account.Enabled)
							continue;

						account.Protocol.Status = mGeneralStatus;
					}

					NotifyPropertyChanged("Status");
					if (StatusChanged != null)
						StatusChanged(null, new StatusUpdateEventArgs() { OldStatus = oldStatus, NewStatus = value });
				}
			}
		}
		public static bool Connected
		{
			get	{
				return mConnected;
			}
			set	{
				if (mConnected != value)
				{
					mConnected = value;

					foreach (var account in Accounts)
					{
						if (account.Enabled && account.Protocol.ProtocolStatus == IMProtocolStatus.Online)
						{
							if (value)
								account.Protocol.BeginLogin();
							else
								account.Protocol.Disconnect();
						}
					}
					NotifyPropertyChanged("Connected");
				}
			}
		}

		private static IMStatus mGeneralStatus = IMStatus.Available;
		private static List<IMProtocolExtraData> accounts;
		private static bool mConnected;
	}
}