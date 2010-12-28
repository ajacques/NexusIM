using System;
using System.Net;
using NexusCore.Services;
using NexusCore.DataContracts;
using System.ServiceModel;

namespace NexusCore.Controllers
{
	class PushCallbackFactory : ISwarmCallback
	{
		public static PushCallbackFactory CreateUDP(Uri endpoint)
		{
			throw new NotImplementedException();
		}

		public static PushCallbackFactory CreateTCP(Uri endpoint)
		{
			throw new NotImplementedException();
		}

		public void OnSwarmMessage(ISwarmMessage message)
		{
			throw new NotImplementedException();
		}
	}
}