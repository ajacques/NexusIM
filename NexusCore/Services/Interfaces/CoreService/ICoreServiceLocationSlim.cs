using System.Collections.Generic;
using System.ServiceModel;
using NexusCore.DataContracts;
using System.ServiceModel.Web;

namespace NexusCore.Services
{
	/// <summary>
	/// Basic CoreService for use by clients such as javascript to get a user location
	/// </summary>
	[ServiceKnownType(typeof(MyAccountInformation))]
	[ServiceContract(Name = "JSCoreService", Namespace = "", SessionMode = SessionMode.Allowed)]
	public interface ICoreServiceLocationSlim
	{
		[OperationContract]
		void Logout();

		/// <summary>
		/// Gets the user's location from the appropriate location service.
		/// </summary>
		[OperationContract]
		[WebInvoke(Method="GET")]
		UserLocationData GetLocation(int rowId);
	}
}