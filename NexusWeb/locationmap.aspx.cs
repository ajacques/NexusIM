using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.Databases;

namespace NexusWeb.Pages
{
	public partial class LocationMap : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] != null && Request.Cookies["AUTHTOKEN"] == null)
			{
				userdbDataContext db = new userdbDataContext();
				int userid = (int)Session["userid"];

				string token = db.NewAuthToken(userid);

				Response.Cookies.Add(new HttpCookie("AUTHTOKEN", token) { HttpOnly = false });
			}
		}
	}
}