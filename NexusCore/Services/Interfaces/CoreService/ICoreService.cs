using System.Collections.Generic;
using System.ServiceModel;
using NexusCore.DataContracts;
using System;

namespace NexusCore.Services
{
	[ServiceKnownType(typeof(MyAccountInformation))]
	[ServiceContract(Name = "CoreService", Namespace = "com.nexusim.core", SessionMode = SessionMode.Allowed)]
	public interface ICoreService : ICoreServiceLocationSlim
	{
		[OperationContract]
		void Login(string username, string password);

		[OperationContract]
		string CookieLogin(string username, string password);

		/// <summary>
		/// Logs into a user's account using an authentication token.
		/// </summary>
		/// <param name="token">Valid authentication token. Could be a temporary setup key.</param>
		[OperationContract(Name = "LoginWithToken")]
		void Login(string token);

		/*[OperationContract]
		[Obsolete]
		string GenerateToken(AuthenticationTokenTypes types);

		[OperationContract]
		void AllAccountCloudLogin();

		[OperationContract]
		[Obsolete]
		void RegisterAsMaster(int accountid);

		[OperationContract]
		[Obsolete]
		void UnregisterAsMaster(int accountid);

		/// <remarks>You must be already authenticated using </remarks>
		/// <param name="deviceid">A valid device authentication token.</param>
		[OperationContract(Name = "SwarmTokenSubscribe")]
		[Obsolete]
		void SwarmSubscribe(string deviceid);

		[OperationContract]
		[Obsolete]
		void SwarmSubscribe();

		[OperationContract]
		[Obsolete]
		void SwarmUnSubscribe();

		[OperationContract]
		[Obsolete]
		IEnumerable<ISwarmMessage> GetSwarmMessages();

		[OperationContract]
		[Obsolete]
		void SendSwarmMessage(ISwarmMessage message, MessageOptions options);

		[OperationContract]
		[Obsolete]
		void StartPushMessageStream(PushChannelType stype, int port);

		[OperationContract(Name = "StartPushMessageStreamUri")]
		[Obsolete]
		void StartPushMessageStream(Uri urichannel);

		[OperationContract]
		[Obsolete]
		void DeviceKeepAlive();*/

		[OperationContract]
		List<ContactLocationInfo> GetLocationData();

		[OperationContract(Name = "GetMultipleLocations")]
		IEnumerable<UserLocationData> GetLocations(List<int> rowIds);

		[OperationContract]
		IEnumerable<AccountInfo> GetAccounts();

		[OperationContract]
		MyAccountInformation GetMyAccountInfo();
	}
}
