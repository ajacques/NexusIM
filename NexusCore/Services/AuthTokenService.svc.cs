using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Runtime.Serialization;
using System.Security.Authentication;
using NexusCore.Support;
using NexusCore.Databases;

namespace NexusCore.Services
{
	[DataContract]
	public sealed class DeviceInfo
	{
		[DataMember]
		public int DeviceId;
		[DataMember]
		public string DeviceName;
	}

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, AutomaticSessionShutdown = true)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public sealed class AuthTokenService : IAuthTokenService
	{
		public IEnumerable<DeviceInfo> GetDeviceByType(string username, string password, int devicetype)
		{
			User user = db.TryLogin(username, password);

			if (user == null)
				throw WCFExceptionTypes.BadLogin;

			var devices = from d in db.Devices
						  where d.userid == user.id && d.devicetype == devicetype
						  select new DeviceInfo() { DeviceId = d.id, DeviceName = d.name };

			return devices;
		}

		public string GetDeviceToken(string username, string password, int deviceid)
		{
			User user = db.TryLogin(username, password);

			if (user == null)
				throw WCFExceptionTypes.BadLogin;

			string token = (from d in db.Devices
							where d.userid == user.id && d.id == deviceid
							select d.logintoken).FirstOrDefault();

			if (token == null)
				throw new FaultException();
			else
				return token;
		}

		private NexusCoreDataContext db = new NexusCoreDataContext();
	}
}