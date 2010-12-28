using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using InstantMessage;
using NexusCore.DataContracts;

namespace NexusCore.Services
{
	[ServiceContract(Namespace = "com.nexusim", SessionMode = SessionMode.Allowed)]
	public interface IWebIMService
	{
		
	}
}