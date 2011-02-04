using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.Properties;
using System.Collections;
using NexusWeb;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class MyDevices : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("../login.aspx?redirect=config/mydevices.aspx", true);

			int userid = (int)Session["userid"];

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/mydevices.js"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Devices.svc"));

			NexusCoreDataContext db = new NexusCoreDataContext();

			foreach (Device device in db.GetDevices(userid))
			{
				TableRow row = new TableRow();
				// Columns
				TableCell name = new TableCell();
				TableCell type = new TableCell();

				// Column Controls
				HyperLink explodelink = new HyperLink();
				Image statusimage = new Image();
				Label nameLabel = new Label();

				explodelink.NavigateUrl = String.Format("javascript:MyDevices.explodeDevice({0});", device.id);

				// Data setup
				row.ID = "device" + device.id;
				row.ClientIDMode = ClientIDMode.Static;
				nameLabel.Text = device.name;

				DeviceType dtype = db.GetDeviceType(device.devicetype);

				type.Text = dtype.LongName; ;

				if (device.lastseen == null && device.lastsignin != null)
				{
					statusimage.ImageUrl = "../images/accept.png";
					name.ToolTip = "Online for " + ((DateTime)device.lastsignin).ToHumanReadableString();
					row.Attributes.Add("connected", "true");
				} else if (device.lastseen != null) {
					statusimage.ImageUrl = "../images/delete.png";
					nameLabel.ForeColor = System.Drawing.Color.LightGray;
					name.ToolTip = "Last seen " + ((DateTime)device.lastseen).ToHumanReadableString() + " ago";
					row.Attributes.Add("connected", "false");
				} else {
					statusimage.ImageUrl = "../images/delete.png";
					nameLabel.ForeColor = System.Drawing.Color.LightGray;
				}

				name.Controls.Add(statusimage);
				explodelink.Controls.Add(nameLabel);
				name.Controls.Add(explodelink);

				// Now we add them to the row
				row.Cells.Add(name);
				row.Cells.Add(type);

				devicetable.Rows.Add(row); // And to the table
			}
		}
	}
}