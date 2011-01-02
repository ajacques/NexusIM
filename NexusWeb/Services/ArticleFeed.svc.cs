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
	public class DbArticleService
	{
		/// <summary>
		/// Returns an IQueryable of StatusUpdates
		/// </summary>
		public IQueryable<StatusUpdate> StatusUpdates
		{
			get	{
				HttpContext context = HttpContext.Current;
				//if (context.Session["userid"] == null)
				//	throw WCFExceptions.BadCredentials;

				int userid = 1;//(int)context.Session["userid"];

				userdbDataContext db = new userdbDataContext();
				var users = db.GetFriends(userid).Select(u => u.id);

				var result = from su in db.StatusUpdates
					where users.Contains(su.Userid) || su.Userid == userid
					select su;

				return result;
			}
		}
		public IQueryable<User> Users
		{
			get
			{
				HttpContext context = HttpContext.Current;
				//if (context.Session["userid"] == null)
				//	throw WCFExceptions.BadCredentials;

				int userid = 1;

				userdbDataContext db = new userdbDataContext();

				return db.Users;
			}
		}
		public IQueryable<ArticleComment> Comments
		{
			get	{
				userdbDataContext db = new userdbDataContext();

				var results = from ac in db.ArticleComments
							  select ac;

				return results;
			}
		}

		[WebGet]
		public IQueryable<User> SearchUsersByDistance(int centerPoint)
		{
			userdbDataContext db = new userdbDataContext();

			return db.Users;
		}
		[WebGet]
		public IQueryable<ArticleComment> GetCommentsForStatusUpdate(int sid)
		{
			HttpContext context = HttpContext.Current;
			//if (context.Session["userid"] == null)
			//	throw WCFExceptions.BadCredentials;

			int userid = 1;//(int)context.Session["userid"];

			userdbDataContext db = new userdbDataContext();

			var result = from ac in db.ArticleComments
						 where ac.ArticleId == sid && ac.ArticleType == "status"
						 select ac;
			
			return result;
		}
	}

	public class ArticleService
	{
		/// <summary>
		/// Returns an IQueryable of StatusUpdates
		/// </summary>
		public IQueryable<ClientStatusUpdate> StatusUpdates
		{
			get	{
				HttpContext context = HttpContext.Current;
				//if (context.Session["userid"] == null)
				//	throw WCFExceptions.BadCredentials;

				int userid = 1;//(int)context.Session["userid"];

				userdbDataContext db = new userdbDataContext();
				var users = db.GetFriends(userid).Select(u => u.id);

				var result = from su in db.StatusUpdates
							 where users.Contains(su.Userid) || su.Userid == userid
							 select MessageFeed.mDbToUsableStatusUpdate(su);

				return result.ToArray().AsQueryable(); //! TODO: This prevents WCF Data Services from carrying the query out on the server and causes the entire request set that matches above and forces .net to do the searching
			}
		}

		public IQueryable<StatusUpdate> Update
		{
			get	{
				throw new NotImplementedException();
			}
		}
		public IQueryable<User> Users
		{
			get	{
				HttpContext context = HttpContext.Current;
				//if (context.Session["userid"] == null)
				//	throw WCFExceptions.BadCredentials;

				int userid = 1;

				userdbDataContext db = new userdbDataContext();

				return db.Users;
			}
		}
		public IQueryable<ClientArticleComment> Comments
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}

	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class ArticleFeed : DataService<DbArticleService>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.SetEntitySetAccessRule("StatusUpdates", EntitySetRights.AllRead);
			//config.SetEntitySetAccessRule("Update", EntitySetRights.AllRead);
			config.SetEntitySetAccessRule("Comments", EntitySetRights.AllRead);
			config.SetEntitySetAccessRule("Users", EntitySetRights.AllRead);
			//config.RegisterKnownType(typeof(ClientArticleUpdate));
		}
	}
}