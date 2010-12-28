using System;
using System.Linq;
using System.Web.UI;
using NexusWeb.Databases;

namespace NexusWeb.SubPages
{
	public partial class LocationAddCustom : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["displayname"] != null)
			{
				customlocationform.Visible = false;
				string username = Request["displayname"];
				string identifier = Request["identifier"];
				string service = Request["servicetype"];
				int userid = (int)Session["userid"];

				Enum.Parse(typeof(LocationServiceType), service);

				userdbDataContext db = new userdbDataContext();

				int rowid = db.UserLocations.Where(ul => ul.identifier == identifier && ul.service == service).Select(ul => ul.id).FirstOrDefault();

				if (rowid == 0)
				{
					UserLocation location = new UserLocation();
					location.username = username;
					location.service = service;
					location.identifier = identifier;
				}

				if (db.LocationPrivacies.Where(lp => lp.locationid == rowid && lp.userid == userid).Count() >= 1)
				{
					Response.Write("duplicate");
					Response.End();
				}

				Response.Write("yes");
				Response.End();
			}
		}
	}
}
