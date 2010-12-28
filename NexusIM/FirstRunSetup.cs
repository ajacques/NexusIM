using System.Collections.Specialized;
using System.Deployment.Application;
using System.Web;
using NexusIM.Managers;

namespace NexusIM
{
	static class FirstRunSetup
	{
		public static void HandleFirstRun()
		{
			NameValueCollection collection = HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);

			if (collection["setupkey"] != null)
			{
				string setupkey = collection["setupkey"];

				NexusCoreManager.DeviceToken = setupkey;
				NexusCoreManager.Setup();
			}
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
				return ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun;
			}
		}
	}
}