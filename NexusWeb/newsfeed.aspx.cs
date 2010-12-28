using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Diagnostics;
using NexusWeb.Masters;
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
			User user = db.Users.Where(u => u.id == userid).First();

			ver = user.FriendListVersion;
			selfid = user.id;

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/Silverlight.js") { ScriptMode = ScriptMode.Debug });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/Silverlight.supportedUserAgent.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/newsfeed.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jquery.glob.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jquery.dimensions.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jquery.gradient.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jStorage.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/LocationBuilder.js") { ScriptMode = ScriptMode.Auto });

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/MessageFeed.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/GeoServices.svc"));
		}

		protected int ver;
		protected int selfid;
	}
}