using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexusWeb.Properties;
using Microsoft.ApplicationServer.Caching;
using System.Diagnostics;
using NexusCore.Databases;
using NexusCore.Databases;

namespace NexusWeb.Pages
{
	public partial class LocationConfig : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// Check to see if we are logged in.. probably a better way to do this
			if (Session["userid"] == null)
				Response.Redirect("../login.aspx?redirect=config/locationconfig.aspx", true);

			int userid = (int)Session["userid"];

			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/location.js"));

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/Accounts.svc"));

			NexusCoreDataContext db = new NexusCoreDataContext();

			bool locationShareState = db.Users.Where(ul => ul.id == userid).Select(ul => ul.locationsharestate).First();

			if (!locationShareState)
			{
				errormessage.Visible = true;
				Label text = new Label() { Text = Resources.LocationDisabledWarning + " " };
				HyperLink link = new HyperLink() { Text = Resources.EnableText, NavigateUrl = "javascript:LocationConfig.enableLocation();" };
				errormessage.Controls.Add(text);
				errormessage.Controls.Add(link);
			}

			// See if the user has configured a location service
			HandleMyServices(db, userid);

			// Get all locations that the user has access to
			HandleFriendServices(db, userid);

			HandleRequests(db, userid);
			HandleSuggestions(db, userid);	
		}

		private void HandleRequests(NexusCoreDataContext db, int user)
		{
			var requests = from r in db.Requests
						   where r.RecipientUserId == user && r.RequestType == "location"
						   select new { RequestId = r.Id, UserId = r.SenderUser.id, Username = r.SenderUser.username, Message = r.MessageBody };

			requests = requests.Take(3);
			
			foreach (var request in requests)
			{
				Label label = new Label();
				label.Text = request.Username + "<br />" + request.Message;

				requestpnl.Controls.Add(label);
			}
		}
		private void HandleFriendServices(NexusCoreDataContext db, int userid)
		{
			IEnumerable<UserLocation> users = db.GetPermittedLocationRows(userid);

			int i = 0;

			foreach (UserLocation user in users.Where(ul => ul.userid != userid))
			{
				TableRow row = new TableRow();
				TableCell name = new TableCell();
				name.Text = user.username;
				TableCell type = new TableCell();
				TableCell options = new TableCell();

				LocationServiceType servicetype = (LocationServiceType)Enum.Parse(typeof(LocationServiceType), user.service);

				if (servicetype == LocationServiceType.GoogleLatitude)
					type.Text = "Latitude";

				if (i % 2 != 0)
					row.Style.Add("background-color", "#EDEBEB");
				else
					row.Style.Add("background-color", "#ffffff");

				i++;

				row.ID = "locationrow" + user.id;
				row.ClientIDMode = ClientIDMode.Static;

				// Options buttons
				HyperLink delete = new HyperLink();
				delete.ImageUrl = "~/images/delete.png";
				delete.NavigateUrl = String.Format("javascript:LocationConfig.deleteFriend({0});", user.id);
				
				options.Controls.Add(delete);

				delete.Style.Add("margin-left", "27px");
				

				row.Cells.Add(name);
				row.Cells.Add(type);
				row.Cells.Add(options);

				friendrows.Rows.Add(row);
			}
		}
		private void HandleMyServices(NexusCoreDataContext db, int userid)
		{
			var services = from ul in db.UserLocations where ul.userid == userid select new { ServiceType = (LocationServiceType)Enum.Parse(typeof(LocationServiceType), ul.service) };
			
			foreach (var service in services)
				myservices.Items.Add(service.ServiceType.ToString());
		}
		private void HandleSuggestions(NexusCoreDataContext db, int userid)
		{
			Dictionary<int, string> suggest = null;

			BooleanSwitch s = new BooleanSwitch("EnableAppFabric", "Enables caching using AppFabric");
			DataCache cache = null;

			if (s.Enabled)
			{
				DataCacheFactory factory = new DataCacheFactory();
				cache = factory.GetCache("LocationSuggestions");

				suggest = (Dictionary<int, string>)cache.Get(userid.ToString());
			}

			if (suggest == null)
			{
				// Gets a list of users that are friends with the current user, but aren't currently sharing their location with you
				var suggestions = from ul in db.UserLocations
								  join u in db.Users on ul.userid equals u.id
								  join f in db.Friends on ul.userid equals f.friendid
								  where ul.userid != null
									&& ul.userid != userid
									&& db.LocationPrivacies.Where(lp => lp.userid == userid
									&& lp.locationid == ul.id).Count() == 0
									&& (f.userid == userid || f.friendid == userid)
								  select new { UserId = u.id, Username = u.username };

				suggest = suggestions.Take(2).ToDictionary(a => a.UserId, b => b.Username);

				if (s.Enabled)
					cache.Put(userid.ToString(), suggest);
			}

			foreach (var suggestion in suggest)
			{
				ListItem item = new ListItem();
				item.Text = suggestion.Value;

				SuggestionList.Items.Add(item);
			}
		}
	}
}