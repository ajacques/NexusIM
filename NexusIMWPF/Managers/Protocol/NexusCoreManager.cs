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

			mClient.BeginLogin(username, password, new AsyncCallback(CoreService_OnLogin), null);
		}

		// Callbacks
		private static void CoreService_OnLogin(IAsyncResult result)
		{
			mClient.EndLogin(result);
			mClient.BeginGetAccounts(new AsyncCallback(CoreService_OnGetAccounts), null);
		}
		private static void CoreService_OnGetAccounts(IAsyncResult result)
		{
			IEnumerable<AccountInfo> accounts = mClient.EndGetAccounts(result);

			foreach (AccountInfo account in accounts)
			{

			}
		}

		public static event EventHandler onLogin;

		// Variables
		private static StreamReader mMsgStreamReader;
		private static CoreServiceClient mClient;
	}
}