﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.Databases;

namespace NexusWeb.Masters
{
	public partial class CenteredMaster : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (Session["userid"] != null)
			{
				int userid = (int)Session["userid"];
				userdbDataContext db = new userdbDataContext();
				var user = (from u in db.Users
							where u.id == userid
							select new { u.firstname, u.lastname }).First();

				headerright.Visible = true;
				MyDisplayImageByUpdatebox.ImageUrl = String.Format(MyDisplayImageByUpdatebox.ImageUrl, userid);
				ProminantMyUsername.Text = user.firstname + " " + user.lastname;
				ProminantMyUsername.NavigateUrl = String.Format(ProminantMyUsername.NavigateUrl, userid);
			} else {
				rightcol.Visible = false;
				bodyDiv.Style.Add("width", "770px");
			}
		}
	}
}