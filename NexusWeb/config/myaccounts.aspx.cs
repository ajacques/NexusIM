using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class MyNetworkAccounts : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
				Response.Redirect("../login.aspx?redirect=config/myaccounts.aspx");

			int userid = (int)Session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/myaccounts.js"));
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/jquery.tablednd_0_5.js"));

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));

			foreach (Account acc in db.GetAccounts(userid))
			{
				TableRow row = new TableRow();
				TableCell state = new TableCell();
				TableCell username = new TableCell();
				TableCell network = new TableCell();
				TableCell options = new TableCell();

				row.ID = "account" + acc.id;
				row.CssClass = "sortable";
				username.CssClass = "accusername";
				//row.Attributes.Add("origpriority", 

				// Sub Controls
				CheckBox statechk = new CheckBox();
				HyperLink deleteimg = new HyperLink();
				deleteimg.NavigateUrl = "javascript:deleteAccount(" + acc.id + ");";
				deleteimg.ImageUrl = "../images/cross.png";

				HyperLink editimg = new HyperLink();
				editimg.NavigateUrl = "javascript:editAccount(" + acc.id + ");";
				editimg.ImageUrl = "../images/bullet_key.png";

				// Set the data
				statechk.Checked = acc.enabled;
				statechk.EnableViewState = false;
				statechk.ID = "enableState" + acc.id;
				statechk.Attributes.Add("startstate", acc.enabled.ToString().ToLower());
				statechk.Attributes.Add("accountid", acc.id.ToString());
				username.Text = acc.username;
				network.Text = acc.acctype;

				options.Controls.Add(editimg);
				options.Controls.Add(deleteimg);
				state.Controls.Add(statechk);
				row.Cells.Add(state);
				row.Cells.Add(username);
				row.Cells.Add(network);
				row.Cells.Add(options);

				accounttable.Rows.Add(row);
			}
		}
	}
}