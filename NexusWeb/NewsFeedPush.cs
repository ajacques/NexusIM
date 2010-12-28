using System.ServiceModel;
using NexusWeb.Services.DataContracts;

[ServiceContract(CallbackContract = typeof(INewsFeedCallback))]
public interface INewsFeedPush
{
	[OperationContract]
	void PostStatusMessage(string messageBody);
}

[ServiceContract]
public interface INewsFeedCallback
{
	[OperationContract(IsOneWay = true)]
	void OnStatusMessage(string messages);
}