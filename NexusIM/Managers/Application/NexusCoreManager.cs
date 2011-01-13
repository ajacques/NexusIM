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
using NexusIM.Protocol;
using NexusIM.PushChannel;

namespace NexusIM.Managers
{
	enum ProtocolConnectMethod
	{
		Direct,
		CloudRouted
	}

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	class NexusCoreManager : ISwarmCallback
	{
		public static void Setup()
		{
			StartPushCallbackService();
			AccountManager.onInternetStatusChange += new GenericEvent(AccountManager_onInternetStatusChange);
		}

		public static void LoginAsDevice()
		{
		}
		public static void RegisterAsMaster(int accountid)
		{
			
		}
		public static void SendSwarmMessage(ISwarmMessage message)
		{
			
		}
		private static void StartPushCallbackService()
		{
			mServiceHostedClass = new NexusCoreManager();
			ServiceHost host = HostFactory.CreateUDPHost(mServiceHostedClass, 2362);
			host.Open();
		}

		private static void LoginWithToken_Completed(IAsyncResult e)
		{
			

			Trace.WriteLine("NexusCoreManager: Getting Accounts");
		}
		private static void CoreClient_GetAccounts(IAsyncResult e)
		{
		}
		private static void AccountManager_onInternetStatusChange()
		{
			throw new NotImplementedException();
		}

		private static void DoDirectConnect(IEnumerable<AccountInfo> accounts)
		{
			foreach (var account in accounts)
			{
				IMProtocol protocol = new SwarmHostedProtocol(account);

				AccountManager.Accounts.Add(protocol);
			}
		}
		private static void DoCloudRouted(IEnumerable<AccountInfo> accounts)
		{
			foreach (var account in accounts)
			{
				IMProtocol protocol = new CoreWrapperProtocol(account);

				AccountManager.Accounts.Add(protocol);
			}
		}

		// ISwarmCallback
		public void OnSwarmMessage(ISwarmMessage message)
		{
			if (message is ProtocolReadyMessage)
			{
				var msg = message as ProtocolReadyMessage;

				IMProtocol protocol = AccountManager.Accounts.Where(p => p.Protocol == msg.AccountInfo.ProtocolType && p.Username == msg.AccountInfo.Username).FirstOrDefault();

				if (protocol == null)
					return;

				protocol.Guid = msg.ProtocolId;
			}
		}

		// Properties
		public static bool LoginState
		{
			get {
				return mLoginState;
			}
			set {
				mLoginState = value;
			}
		}
		public static string DeviceToken
		{
			get {
				return mDeviceToken;
			}
			set {
				mDeviceToken = value;
			}
		}
		public static event EventHandler onLogin;

		// Variables
		private static ProtocolConnectMethod mConnectMethod;
		private static NexusCoreManager mServiceHostedClass;
		private static StreamReader mMsgStreamReader;
		private static bool mLoginState;
		private static string mDeviceToken;
		private static string mSessionId;
	}
}