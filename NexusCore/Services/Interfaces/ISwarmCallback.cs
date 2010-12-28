using NexusCore.DataContracts;
using System;
using System.ServiceModel;

namespace NexusCore.Services
{
	[ServiceContract]
	public interface ISwarmCallback
	{
		[OperationContract(IsOneWay = true)]
		void OnSwarmMessage(ISwarmMessage message);
	}
}