using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Web.SessionState;

namespace NexusWeb
{
	public class NXServiceAuthorizationManager : ServiceAuthorizationManager
	{
		public override bool CheckAccess(OperationContext operationContext)
		{
			HttpSessionState state = HttpContext.Current.Session;

			return state["userid"] != null;
		}
	}
}