using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.AdminChannel;
using NexusWeb.Masters;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class UserPage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/user.js"));
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jquery.glob.js") { ScriptMode = ScriptMode.Auto });

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/MessageFeed.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("http://core.nexus-im.com/Services/CoreService.svc/locjs")); // Used to resolve user locations

			// Check to see if the user is logged in
			if (Session["userid"] == null)
				Response.Redirect("~/w/login.aspx?redirect=" + HttpUtility.UrlDecode("user.aspx?userid=" + Session["userid"] != null ? Session["userid"].ToString() : "0"));

			int userid = (int)Session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();
			User clientUser = db.GetUser(userid);

			if (Request["userid"] != null)
			{
				int profileid = Convert.ToInt32(Request["userid"]);
				
				var profileinfo = (from u in db.Users
								  where u.id == profileid
								  select u).FirstOrDefault();

				remoteid = profileid;

				// Escalation Cookie - Used to bootstrap authentication on core.nexus-im.com
				HttpCookie escalatecookie = new HttpCookie("usrloctoken")
				{
					Expires = DateTime.UtcNow.AddMinutes(1),
					Domain = ".nexus-im.com"
				};
				escalatecookie.Values.Add("userid", userid.ToString());
				escalatecookie.Values.Add("pwdhash", clientUser.password);

				Response.SetCookie(escalatecookie);

				// Change the UI
				Title = profileinfo.firstname + " " + profileinfo.lastname;
				majorname.Text = profileinfo.firstname + " " + profileinfo.lastname;

				avatar.ImageUrl = String.Format(avatar.ImageUrl, profileid);
			}

			db.Dispose();
		}

		protected int remoteid = -1;
	}
}