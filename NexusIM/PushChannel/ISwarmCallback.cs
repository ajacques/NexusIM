using System.ServiceModel;
using NexusIM.NexusCore;

namespace NexusIM.PushChannel
{
	[ServiceContract]
	interface ISwarmCallback
	{
		[OperationContract(IsOneWay = true)]
		void OnSwarmMessage(ISwarmMessage message);
	}
}