using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Session;
using NexusWeb.Properties;
using NexusWeb.Services.DataContracts;

namespace NexusWeb.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class SocialNetworks
	{
		[OperationContract]
		public string GetFacebookConnectUrl()
		{
			ConnectSession session = new ConnectSession(Settings.Default.FacebookAPIKey, Settings.Default.FacebookSecret);
			Api api = new Api(session);
			return api.ExtendedPermissionUrl(Enums.ExtendedPermissions.read_stream | Enums.ExtendedPermissions.publish_stream | Enums.ExtendedPermissions.status_update | Enums.ExtendedPermissions.offline_access);
		}

		[OperationContract]
		public ClientStatusUpdate[] GetFacebookArticles()
		{
			ConnectSession session = new ConnectSession(Settings.Default.FacebookAPIKey, Settings.Default.FacebookSecret);
			Api api = new Api(session);

			throw new NotImplementedException(api.LoginUrl);
		}
	}
}