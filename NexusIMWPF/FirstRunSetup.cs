using System.Collections.Specialized;
using System.Deployment.Application;
using System.Web;
using NexusIM.Managers;
using InstantMessage;
using System.Linq;

namespace NexusIM
{
	static class FirstRunSetup
	{
		public static void HandleFirstRun()
		{
			
		}

		public static bool IsNetworkDeployed
		{
			get {
				return ApplicationDeployment.IsNetworkDeployed;
			}
		}

		public static bool IsFirstRun
		{
			get {
				return false;
			}
		}
	}
}