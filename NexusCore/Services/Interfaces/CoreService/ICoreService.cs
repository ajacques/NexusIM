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

		/// <summary>
		/// Logs into a user's account using an authentication token.
		/// </summary>
		/// <param name="token">Valid authentication token. Could be a temporary setup key.</param>
		[OperationContract(Name = "LoginWithToken")]
		void Login(string token);

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
