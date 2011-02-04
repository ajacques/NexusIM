using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class DemoLogin : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			AttemptLogin("test", "test");
			RedirectToPage();
		}
		private void AttemptLogin(string username, string password)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			var user = db.TryLogin(username, password);

			if (user != null)
			{
				Session["userid"] = user.id;
				Session["username"] = user.password;
			}

			db.Dispose();
		}
		private void RedirectToPage()
		{
			if (Request.QueryString["redirect"] != null)
				Response.Redirect(Request.QueryString["redirect"], true);
			else
				Response.Redirect("config/myaccount.aspx", false);
		}
	}
}