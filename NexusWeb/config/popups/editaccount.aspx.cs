using System;
using System.Web.UI;
using System.Net;
using System.Linq;
using NexusWeb.Databases;

namespace NexusWeb.SubPages
{
	public partial class EditAccount : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["userid"] == null)
			{
				Response.StatusCode = (int)HttpStatusCode.Forbidden;
				Response.Close();
			}

			int userid = (int)Session["userid"];
			int accid = Convert.ToInt32(Request["id"]);

			userdbDataContext db = new userdbDataContext();

			var account = from a in db.Accounts
						  where a.userid == userid && a.id == accid
						  select a;

			var acc = account.FirstOrDefault();

			if (acc != null)
			{
				protocol.Text = acc.acctype;
				username.Text = acc.username;
				accountid.Value = accid.ToString();
			}
		}
	}
}