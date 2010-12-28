using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NexusCore.Services
{
	[ServiceContract(Namespace = "com.adrensoftware.webim")]
	public interface ICoreLogin
	{
		[OperationContract]
		void LoginToCore(string username, string password);

		[OperationContract(Name = "LoginToCoreWithToken")]
		void LoginToCore(string token);
	}
}