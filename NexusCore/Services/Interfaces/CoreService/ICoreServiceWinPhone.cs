using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NexusCore.DataContracts;

namespace NexusCore.Services
{
	[ServiceContract(Name = "CoreService", Namespace = "com.nexusim.mobile")]
	public interface ICoreServiceWinPhone
	{
		[OperationContract]
		void Login(string username, string password);

		[OperationContract]
		ContactInfo GetContactInfo(int userid);

		[OperationContract]
		bool AccountsSignedIn();

		[OperationContract]
		IEnumerable<AccountInfo> GetAccounts();
	}
}