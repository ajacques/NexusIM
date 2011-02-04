using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class MainPage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] != null && Request.Cookies["AUTHTOKEN"] == null)
			{
				NexusCoreDataContext db = new NexusCoreDataContext();
				int userid = (int)Session["userid"];

				string token = db.NewAuthToken(userid);

				Response.Cookies.Add(new HttpCookie("AUTHTOKEN", token) { HttpOnly = false });
			}
		}
	}
}