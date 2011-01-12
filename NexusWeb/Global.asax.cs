using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using NexusWeb.Properties;
using System.Configuration;

namespace NexusWeb
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			//SqlDependency.Start(ConfigurationManager.ConnectionStrings["SqlDependencyConnectionString"].ConnectionString);
		}

		protected void Application_End(object sender, EventArgs e)
		{
			//SqlDependency.Stop(ConfigurationManager.ConnectionStrings["SqlDependencyConnectionString"].ConnectionString);
			SqlConnection.ClearAllPools();
		}
	}
}