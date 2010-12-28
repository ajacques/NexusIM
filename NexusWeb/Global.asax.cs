using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;

namespace NexusWeb
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{
			SqlConnection.ClearAllPools();
		}
	}
}