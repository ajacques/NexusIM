using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using NexusWeb.Services.DataContracts;
using System.ServiceModel;
using System.Net;
using System.Security;
using NexusWeb.Databases;

namespace NexusWeb.Services
{
	public class ArticleService
	{
		public ArticleService()
		{

		}

		/// <summary>
		/// Returns an IQueryable`1 of StatusUpdates
		/// </summary>
		public IQueryable<ClientStatusUpdate> StatusUpdates
		{
			get	{
				HttpContext context = HttpContext.Current;
				if (context.Session["userid"] == null)
					throw WCFExceptions.BadCredentials;

				int userid = (int)context.Session["userid"];

				userdbDataContext db = new userdbDataContext();
				var users = db.GetFriends(userid).Select(u => u.id);

				return MessageFeed.GetStatusUpdates(su => users.Contains(su.Userid)).AsQueryable<ClientStatusUpdate>();
			}
		}

		public IQueryable<ClientArticleComment> Comments
		{
			get	{
				throw new NotImplementedException();
			}
		}
	}

	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class ArticleFeed : DataService<ArticleService>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("StatusUpdates", EntitySetRights.AllRead);
			config.SetEntitySetAccessRule("Comments", EntitySetRights.AllRead);
			config.RegisterKnownType(typeof(ClientArticleUpdate));
		}
	}
}