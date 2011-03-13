using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using InstantMessage;
using Microsoft.WindowsAPICodePack.Net;
using System.Threading;

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
			Trace.WriteLine("Connectivity Status: " + NetworkListManager.Connectivity.ToString());
			mConnected = true;

			Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}

		[Obsolete("Use Accounts.Add instead", false)]
		public static void AddNewAccount(IMProtocol protocol)
		{
			AddNewAccount(new IMProtocolExtraData() { Protocol = protocol, Enabled = true, IsReady = false});
		}
		[Obsolete("Use Accounts.Add instead", false)]
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
		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback((object obj) => {
				if (e.NewItems != null)
				{
					foreach (IMProtocolExtraData extraData in e.NewItems)
					{
						extraData.IsReady = true;
						extraData.PropertyChanged += new PropertyChangedEventHandler(IndividualProtocol_PropertyChanged);
						ConnectIfNeeded(extraData);
					}
				}
			}));
		}

		private static void ConnectIfNeeded(IMProtocolExtraData extraData)
		{
			if (Connected && IsConnectedToInternet())
			{
				if (extraData.Enabled != (extraData.Protocol.ProtocolStatus != IMProtocolStatus.Offline))
				{
					if (extraData.Enabled)
						extraData.Protocol.BeginLogin();
					else
						extraData.Protocol.Disconnect();
				}
			}
		}

		private static void IndividualProtocol_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			IMProtocolExtraData extraData = (IMProtocolExtraData)sender;
			
			switch (e.PropertyName)
			{
				case "Enabled":
					ConnectIfNeeded(extraData);
					break;
			}
			
		}

		public static event EventHandler<StatusUpdateEventArgs> StatusChanged;
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
		public static ObservableCollection<IMProtocolExtraData> Accounts
		{
			get {
				if (accounts == null)
					accounts = new ObservableCollection<IMProtocolExtraData>();
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
		private static ObservableCollection<IMProtocolExtraData> accounts;
		private static bool mConnected;
	}
}