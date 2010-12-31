using System;
using System.Linq;
using System.Web.UI;
using NexusWeb.Databases;

namespace NexusWeb.Pages
{
	public partial class NewsFeed : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("login.aspx?redirect=newsfeed.aspx");

			int userid = (int)Session["userid"];
			userdbDataContext db = null;

			db = new userdbDataContext();
			user = db.Users.Where(u => u.id == userid).First();

			ver = user.FriendListVersion;
			selfid = user.id;

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/MessageFeed.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/GeoServices.svc"));
		}

		protected User user;
		protected int ver;
		protected int selfid;
	}
}