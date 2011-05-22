using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using NexusCore.Databases;

namespace NexusWeb.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class DeviceService
	{
		/// <summary>
		/// Generates a temporary download url that allows the user to instantly download and attach a device to their account without any setup.
		/// </summary>
		/// <param name="devicename">Client visible name that refers to the device's name</param>
		/// <returns>Url to the download file</returns>
		[OperationContract]
		public string CreateDeviceDownloadLink(string devicetype, string devicename)
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				return "javascript:alert('Not Logged in? But how?');";

			int userid = (int)session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();

			DeviceType type = db.GetDeviceType(devicetype);

			Device device = new Device();
			device.userid = userid;
			device.name = devicename;
			device.devicetype = type.Id;
			device.logintoken = PasswordGenerator.RandomString(50);

			string dlurl = String.Format(type.DownloadUrl, device.logintoken);

			db.Devices.InsertOnSubmit(device);
			db.SubmitChanges();
			db.Dispose();

			return dlurl;
		}

		[OperationContract]
		public void DeleteDevice(int deviceid)
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();

			var device = (from d in db.Devices
						  where d.userid == userid && d.id == deviceid
						  select d).FirstOrDefault();

			if (device == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			} else {
				db.Devices.DeleteOnSubmit(device);

				//db.SubmitChanges();
			}

			db.Dispose();
		}

		[OperationContract]
		public void DisconnectDevice(int deviceid)
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.StatusDescription = "Not Logged-In";
				response.Close();
			}

			int userid = (int)session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();

			var devices = from d in db.Devices
						  where d.userid == userid && d.id == deviceid
						  select d;

			if (devices.Count() >= 1)
			{
				var device = devices.First();
				device.lastseen = DateTime.UtcNow;

				db.SubmitChanges();
			} else {
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.StatusDescription = "Invalid Device Id";
				response.Close();
			}
		}
	}
}