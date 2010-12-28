using System;
using System.ServiceModel;
using InstantMessage;
using NexusCore.DataContracts;

namespace NexusCore.Services
{
	[ServiceContract(Namespace = "com.adrensoftware.webim", SessionMode = SessionMode.Allowed)]
	interface IWebIMWinPhone : ICoreLogin
	{
		[OperationContract]
		void PreconfiguredLogin(int protocolId, IMStatus status);

		[OperationContract]
		void StartSession();
		
		[OperationContract]
		void SendMessage(Guid protocolId, string username, string messagebody);

		[OperationContract(Name = "StartPushStream")]
		void StartPushStream(PushChannelType type, Uri urichannel);

		[OperationContract]
		void StopPushStream();
	}
}