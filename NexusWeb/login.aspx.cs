﻿using System;
using System.Security.Authentication;
using System.Web.UI;
using NexusCore.Databases;
using NexusWeb.Masters;
using System.Linq;

namespace NexusWeb.Pages
{
	public partial class Login : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// First check to see if the user is already logged in
			if (Session["userid"] != null)
				RedirectToPage();

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/login.js"));
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/crypto-sha256.js"));

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/ValidationFunctions.svc"));
		}
		private void AttemptLogin(string username, string password)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			var user = db.TryLogin(username, password);

			if (user != null)
			{
				Session["userid"] = user.id;
				Session["username"] = user.username;
			} else
				throw new AuthenticationException("Failed to authenticate user");

			db.Dispose();
		}
		private void RedirectToPage()
		{
			if (Request.QueryString["redirect"] != null)
				Response.Redirect(Request.QueryString["redirect"], true);
			else
				Response.Redirect("newsfeed.aspx", false);
		}
	}
}