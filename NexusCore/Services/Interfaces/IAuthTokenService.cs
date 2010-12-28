using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NexusCore.Services
{
	/// <summary>
	/// Tool library for client applications.
	/// </summary>
	[ServiceContract]
	public interface IAuthTokenService
	{
		[OperationContract]
		IEnumerable<DeviceInfo> GetDeviceByType(string username, string password, int devicetype);

		/// <summary>
		/// Returns the authentication token of the specified device if the device belongs to the specified user's account.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="deviceid"></param>
		/// <returns></returns>
		[OperationContract]
		string GetDeviceToken(string username, string password, int deviceid);
	}
}