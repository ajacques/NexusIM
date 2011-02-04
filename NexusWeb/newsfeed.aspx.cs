using System;
using System.Linq;
using System.Web.UI;
using NexusCore.Databases;
using System.Web.UI.WebControls;

namespace NexusWeb.Pages
{
	public partial class NewsFeed : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("login.aspx?redirect=newsfeed.aspx");

			int userid = (int)Session["userid"];
			NexusCoreDataContext db = new NexusCoreDataContext();
			dbUser = db.Users.Where(u => u.id == userid).First();

			ver = dbUser.FriendListVersion;
			selfid = dbUser.id;

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/MessageFeed.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/GeoServices.svc"));
		}

		protected User dbUser;
		protected int ver;
		protected int selfid;
	}
}