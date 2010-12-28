using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace NexusCore.Services
{
	/// <summary>
	/// For use by visual studio's proxy auto generation stuff only.
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class PushCallback : ISwarmCallback
	{
		public void OnSwarmMessage(DataContracts.ISwarmMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
