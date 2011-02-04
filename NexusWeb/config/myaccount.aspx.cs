using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.Properties;
using System.Collections;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class MyAccount : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("../login.aspx?redirect=config/myaccount.aspx", true);

			int userid = (int)Session["userid"];

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/myaccount.js"));
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/Silverlight.js") { ScriptMode = ScriptMode.Auto });
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/Silverlight.supportedUserAgent.js") { ScriptMode = ScriptMode.Auto });

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));
			
			NexusCoreDataContext db = new NexusCoreDataContext();

			var accs = from a in db.Accounts
					   where a.userid == userid
					   select a;

			if (accs.Count() >= 1)
			{
				accountstatusnone.Visible = false;
			} else {
				accountstatusgood.Visible = false;
			}

			// Online Devices
			IEnumerable<Device> onlinedevices = db.GetOnlineDevices(userid);

			deviceonlinecount.Text = onlinedevices.Count().ToString();

			if (onlinedevices.Count() >= 1)
				accountState.Visible = true;
			else
				accountState.Visible = false;

			foreach (var device in onlinedevices.Take(3))
				onlinedevicesample.Items.Add(new ListItem(device.name));

			if (onlinedevices.Count() > 3)
				onlinedevicesample.Items.Add(new ListItem("..."));

			// Total Devices
			int devicecount = db.GetDevices(userid).Count();
			devicetotalcount.Text = devicecount.ToString();

			if (devicecount >= 1)
			{
				devicesstatusnodevices.Visible = false;
			} else {
				devicesstatusready.Visible = false;
			}

			bool locationShareState = db.Users.Where(u => u.id == userid).Select(u => u.locationsharestate).First();

			if (locationShareState)
			{
				locationstate.Value = "true";
				locationstatusoff.Visible = false;
				locationtogglehref.InnerText = Resources.DisableText;
			} else {
				locationstate.Value = "false";
				locationstatuson.Visible = false;
				locationtogglehref.InnerText = Resources.EnableText;
			}
		}
	}
}
