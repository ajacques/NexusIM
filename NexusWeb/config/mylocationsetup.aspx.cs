using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using NexusWeb.Properties;
using NexusWeb;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class MyLocationSetup : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("../login.aspx?redirect=/config/mylocationsetup.aspx", true);

			if (glatitudeident.Text != "")
			{
				string data = glatitudeident.Text;
				Match rgx = Regex.Match(data, Resources.GoogleLatitudeIdentiferParseRegex);

				NexusCoreDataContext db = new NexusCoreDataContext();
				UserLocation location = new UserLocation();
				location.userid = (int)Session["userid"];
				location.service = "GoogleLatitude";
				location.username = (string)Session["username"];
				location.identifier = rgx.Groups[1].Value;
				db.UserLocations.InsertOnSubmit(location);
				db.SubmitChanges();

				Response.Redirect("locationconfig.aspx", true);
			}

			GoogleLatitudeSetup.Visible = false;
		}

		protected void googlelatitude_CheckedChanged(object sender, EventArgs e)
		{
			lblsetup.Style["color"] = "#000000";
			GoogleLatitudeSetup.Visible = true;
		}
	}
}
