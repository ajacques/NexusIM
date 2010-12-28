using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NexusCore.DataContracts;
using NexusCore.Services;
using NexusCore.Databases;

namespace NexusCore.Controllers
{
	/// <summary>
	/// Allows this server to easily communicate with other servers in the cluster
	/// </summary>
	[Obsolete("", false)]
	class ClusterDistributor
	{
		/// <summary>
		/// Attempts to get a message to a specific device by searching for the server who owns it's connection or puts it in a queue if the device is not
		/// connected to a stream
		/// </summary>
		/// <param name="deviceid">What device should the message be delievered to</param>
		/// <param name="message">Message that should be delievered to the device</param>
		public static void SendMessageToSpecificDevice(int deviceid, ISwarmMessage message)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			db.AddToDeviceMessageQueue(deviceid, message);
			db.Dispose();		
		}

		public static void RegisterStreamingDevice(int deviceid)
		{
			mStreamingDevices.Add(deviceid);
		}

		private static List<int> mStreamingDevices = new List<int>();
	}
}