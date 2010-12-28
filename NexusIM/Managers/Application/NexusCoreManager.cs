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
			if (mCoreClient == null)
				mCoreClient = new CoreServiceClient();

			if (mCoreClient.State == CommunicationState.Closed)
				mCoreClient.Open();

			StartPushCallbackService();
			AccountManager.onInternetStatusChange += new GenericEvent(AccountManager_onInternetStatusChange);
		}

		public static void LoginAsDevice()
		{
			if (String.IsNullOrEmpty(mDeviceToken))
				mDeviceToken = IMSettings.GetCustomSetting("devicetoken", "");

			if (!String.IsNullOrEmpty(mDeviceToken))
			{
				if (mCoreClient == null)
					Setup();

				Trace.WriteLine("NexusCoreManager: Starting Core session");

				if (frmMain.Instance == null)
					frmMain.onFormLoad += new GenericEvent(delegate() { frmMain.Instance.FormState = frmMainState.ConnectingToCore; });
				else
					frmMain.Instance.FormState = frmMainState.ConnectingToCore;

				if (!AccountManager.IsConnectedToInternet())
				{
					Trace.WriteLine("NexusCoreManager: NLA reports device is not connected to the internet. Awaiting state change");
					return;
				}

				mCoreClient.StartSession();
				mCoreClient.BeginLoginWithToken(mDeviceToken, new AsyncCallback(LoginWithToken_Completed), null);
			}
		}
		public static void RegisterAsMaster(int accountid)
		{
			mCoreClient.InsertSessionId(mSessionId);
			mCoreClient.RegisterAsMasterAsync(accountid, accountid);
		}
		public static void SendSwarmMessage(ISwarmMessage message, MessageOptions options)
		{
			mCoreClient.InsertSessionId(mSessionId);
			mCoreClient.SendSwarmMessageAsync(message, options);
		}
		private static void StartPushCallbackService()
		{
			mServiceHostedClass = new NexusCoreManager();
			ServiceHost host = HostFactory.CreateUDPHost(mServiceHostedClass, 2362);
			host.Open();
		}

		private static void LoginWithToken_Completed(IAsyncResult e)
		{
			mCoreClient.StartSession();
			try	{
				mCoreClient.EndLoginWithToken(e);
			} catch (FaultException<AuthenticationException> ex) {
				Trace.TraceError("NexusCoreManager: Failed to login with the given token", ex.GetBaseException().Message);
			} catch (Exception ex) {
				Trace.TraceError("NexusCoreManager: Failed to login due to unknown error: " + ex.GetBaseException().GetType().Name + " " + ex.GetBaseException().Message);
				return;
			}

			Trace.WriteLine("NexusCoreManager: Connected to Core. Beginning subscriptions and setup");
			mSessionId = mCoreClient.GetSessionId();
			Trace.WriteLine("NexusCoreManager: Got session id: " + mSessionId);

			if (onLogin != null)
				onLogin(null, null);

			mCoreClient.InsertSessionId(mSessionId);

			try	{
				mCoreClient.SwarmSubscribe();
			} catch (Exception ex) {
				Trace.TraceError("NexusCoreManager: SwarmSubscribe Exception of type (" + ex.GetBaseException().GetType().Name + "): " + ex.GetBaseException().Message);
			}

			mCoreClient.InsertSessionId(mSessionId);
			mCoreClient.StartPushMessageStream(PushChannelType.GenericUdp, 2363);

			mCoreClient.InsertSessionId(mSessionId);

			Trace.WriteLine("NexusCoreManager: Getting Accounts");
			mCoreClient.BeginGetAccounts(new AsyncCallback(CoreClient_GetAccounts), null);
		}
		private static void CoreClient_GetAccounts(IAsyncResult e)
		{
			List<AccountInfo> accounts;
			try	{
				accounts = mCoreClient.EndGetAccounts(e);
			} catch (FaultException<AuthenticationException> ex) {
				Trace.TraceError("NexusCoreManager: Failed to login with the given token", ex.GetBaseException().Message);
				return;
			} catch (Exception ex) {
				Trace.TraceError("NexusCoreManager: Failed to login due to unknown error: " + ex.GetBaseException().GetType().Name + " " + ex.GetBaseException().Message);
				return;
			}

			if (mConnectMethod == ProtocolConnectMethod.Direct)
				DoDirectConnect(accounts);
			else if (mConnectMethod == ProtocolConnectMethod.CloudRouted)
				DoCloudRouted(accounts);

			AccountManager.TriggerNewLoginEvent();
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
			mCoreClient.InsertSessionId(mSessionId);
			mCoreClient.AllAccountCloudLogin();

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
		public static CoreServiceClient NexusCore
		{
			get {
				if (mCoreClient == null)
					Setup();

				return mCoreClient;
			}
		}
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
		private static CoreServiceClient mCoreClient;
		private static string mDeviceToken;
		private static string mSessionId;
	}
}