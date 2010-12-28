using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using NexusWeb.Databases;

namespace NexusWeb.Services
{
	/// <summary>
	/// Used by client code to test user inputs to see if they are validate.
	/// </summary>
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ValidationFunctions
	{
		/// <summary>
		/// Checks to see if the specified username has been registered by another user.
		/// </summary>
		/// <param name="username">Username to test.</param>
		/// <returns>True if this username is currently in use.</returns>
		[WebGet]
		[OperationContract]
		public bool UsernameInUse(string username)
		{
			userdbDataContext db = new userdbDataContext();
			
			bool result = db.Users.Any(u => u.username == username);

			db.Dispose(); // Mom always said to clean up after myself

			return result;
		}
	}
}
