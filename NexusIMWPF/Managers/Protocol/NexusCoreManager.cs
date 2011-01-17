using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Authentication;
using System.ServiceModel;
using InstantMessage;
using NexusIM.NexusCore;

namespace NexusIM.Managers
{
	enum ProtocolConnectMethod
	{
		Direct,
		CloudRouted
	}

	enum NexusCoreState
	{
		Offline,
		LoggingIn,
		Synchronizing,
		Online
	}

	class NexusCoreStateEventArgs : EventArgs
	{
		internal NexusCoreStateEventArgs(NexusCoreState newState)
		{
			PreviousState = mLastState;
			CurrentState = newState;

			mLastState = newState;
		}
		public NexusCoreState PreviousState
		{
			get;
			private set;
		}
		public NexusCoreState CurrentState
		{
			get;
			private set;
		}

		private static NexusCoreState mLastState;
	}

	static class NexusCoreManager
	{
		public static void Setup()
		{
			mClient = new CoreServiceClient();
		}
		public static void Login(string username, string password)
		{
			if (mClient == null)
				Setup();

			if (OnStateChange != null)
				OnStateChange(null, new NexusCoreStateEventArgs(NexusCoreState.LoggingIn));

			mClient.BeginLogin(username, password, new AsyncCallback(CoreService_OnLogin), null);
		}

		// Callbacks
		private static void CoreService_OnLogin(IAsyncResult result)
		{
			mClient.EndLogin(result);

			if (OnStateChange != null)
				OnStateChange(null, new NexusCoreStateEventArgs(NexusCoreState.Synchronizing));

			mClient.BeginGetAccounts(new AsyncCallback(CoreService_OnGetAccounts), null);
		}
		private static void CoreService_OnGetAccounts(IAsyncResult result)
		{
			IEnumerable<AccountInfo> accounts = mClient.EndGetAccounts(result);

			foreach (AccountInfo account in accounts)
			{
				IMProtocol protocol;
				try	{
					protocol = InterfaceManager.CreateProtocol(account.ProtocolType);
				} catch (Exception) {
					Debug.WriteLine("NexusCoreManager: Protocol of type " + account.ProtocolType + " not supported at this time.");
					break;
				}
				protocol.Username = account.Username;
				protocol.Password = account.Password;
				protocol.Enabled = account.Enabled;

				AccountManager.AddNewAccount(protocol);
			}

			if (OnStateChange != null)
				OnStateChange(null, new NexusCoreStateEventArgs(NexusCoreState.Online));
		}

		public static event EventHandler<NexusCoreStateEventArgs> OnStateChange;

		// Variables
		private static StreamReader mMsgStreamReader;
		private static CoreServiceClient mClient;
	}
}