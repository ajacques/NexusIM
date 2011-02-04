using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class Logout : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				return;

			int userid = (int)Session["userid"];
			string token = (string)Request["token"];

			NexusCoreDataContext db = new NexusCoreDataContext();

			try	{
				Session.Clear();
				Session.Abandon();
			} catch (SecurityException) {

			}
		}
	}
}