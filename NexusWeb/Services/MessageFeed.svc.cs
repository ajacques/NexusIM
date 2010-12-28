﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using LinqToCache;
using Microsoft.ApplicationServer.Caching;
using NexusWeb.Databases;
using NexusWeb.Properties;
using NexusWeb.Services.DataContracts;
using Microsoft.SqlServer.Types;
using System.Data.Common;

namespace NexusWeb.Services
{
	internal delegate void ArticlePollDelegate(ClientStatusUpdate e);

	public enum PhotoSize
	{
		Small = 32,
		Medium = 64,
		Large = 128
	}

	/// <summary>
	/// Exposes the user's message feed and feed related objects to javascript.
	/// </summary>
	/// <remarks>
	/// For live streaming, two methods are required
	/// </remarks>
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[KnownType(typeof(ClientArticleUpdate))]
	public sealed class MessageFeed
	{
		static MessageFeed()
		{
			//db = new userdbDataContext();

			// HOLY! Look at this code! What does it do? Well, I shall show you
			mDbToUsableStatusUpdate = (su) => new ClientStatusUpdate() // It's a massive lambda expression that converts the database code to the custom format
			{
				mArticleId = su.Id,
				mUserId = su.Userid,
				mUserPrefix = "nx",
				mTimeStamp = su.Timestamp,
				mMessageBody = su.MessageBody,
				mSourceType = ClientArticleSourceType.User,
				mComments = GetComments(ac => ac.ArticleId == su.Id && ac.ArticleType == "status"),
				// First we make sure there is a geotag, then we convert it to the proper format
				mGeoTag = su.GeoTag != null ? new GeoLocation()
				{
					mLatitude = su.GeoTag.Lat.Value,
					mLongitude = su.GeoTag.Long.Value,
					mAccuracy = su.GeoTagAccuracy,
					mAltitude = !su.GeoTag.Z.IsNull ? su.GeoTag.Z.Value : 0
				} : null
			};
		}

		public MessageFeed()
		{
			if (Settings.Default.EnableAppFabricCache)
				mAppFabricFactory = new DataCacheFactory();
		}

		/// <summary>
		/// Retrieves all of the status updates starting from the specified DateTime.
		/// Returns instantly after getting the status updates
		/// </summary>
		/// <param name="messagesSince">Only show messages newer than this</param>
		/// <returns>A list of all the requested status updates</returns>
		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleUpdate> GetStatusUpdatesSince(DateTime messagesSince)
		{
			int userid;
			HandleWCFAuth(out userid);

			int[] friends = GetCachedFriends(userid);

			var statuses = GetStatusUpdates(su => (userid == su.Userid || friends.Contains(su.Userid)) && su.Timestamp > messagesSince);

			statuses = statuses.Reverse();

			return statuses;
		}

		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleUpdate> GetStatusUpdatesBetween(DateTime startdate, DateTime enddate)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("NexusWeb: Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw WCFExceptions.CrossSiteViolation;
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			int[] friends = GetCachedFriends(userid);

			var statuses = GetStatusUpdates(su => (userid == su.Userid || friends.Contains(su.Userid)) && su.Timestamp > startdate && su.Timestamp < enddate);

			statuses.Reverse();

			return statuses;
		}

		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleUpdate> GetStatusUpdatesSinceLongPoll(DateTime messagesSince)
		{
			// Same code as above, but this time we will wait for messages if there are none
			int userid;
			HandleWCFAuth(out userid);

			int[] friends = GetCachedFriends(userid);

			List<ClientStatusUpdate> statuses = GetStatusUpdates(su => (userid == su.Userid || friends.Contains(su.Userid)) && su.Timestamp > messagesSince).ToList();

			userdbDataContext db = new userdbDataContext();

			SqlDependency depends = new SqlDependency();

			if (statuses.Count() >= 1)
				return statuses.ToArray();

			ManualResetEventSlim mutex = new ManualResetEventSlim();

			mStatusUpdateCallbacks[userid] = new ArticlePollDelegate(delegate(ClientStatusUpdate su)
			{
				statuses.Add(su);
				mutex.Set();
			});

			mutex.Wait(Settings.Default.LongPollTimeout);

			mutex.Dispose();

			mStatusUpdateCallbacks.Remove(userid);

			return statuses;
		}

		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleUpdate> test(DateTime messagesSince)
		{
			// Same code as above, but this time we will wait for messages if there are none
			int userid;
			HandleWCFAuth(out userid);

			int[] friends = GetCachedFriends(userid);

			string conn = ConfigurationManager.ConnectionStrings["SqlDependencyConnectionString"].ConnectionString;

			// Fix this. Setup the account permissions in the database to allow me to use the integrated security
			userdbDataContext db = new userdbDataContext(conn);
			SqlDependency.Start(conn);

			ManualResetEventSlim statusWaitMutex = new ManualResetEventSlim();
			var results = db.StatusUpdates.Where(su => (userid == su.Userid || friends.Contains(su.Userid)) && su.Timestamp > messagesSince).AsCached("StatusUpdates", new CachedQueryOptions()
			{
				OnInvalidated = (sender, args) => statusWaitMutex.Set()
			}).Select(mDbToUsableStatusUpdate);

			if (results.Count() == 0)
				statusWaitMutex.Wait(Settings.Default.LongPollTimeout);

			return results;
		}

		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleComment> GetArticleComments(string articleType, int articleId)
		{
			int userid;
			HandleWCFAuth(out userid);

			return GetComments(c => c.ArticleId == articleId && c.ArticleType == articleType);
		}

		/// <summary>
		/// Returns the last 
		/// </summary>
		/// <param name="targetid"></param>
		/// <returns></returns>
		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public IEnumerable<ClientArticleUpdate> GetStatusUpdatesForUserPage(int targetid)
		{
			int userid;
			HandleWCFAuth(out userid);

			return GetStatusUpdates(su => su.Userid == targetid).OrderByDescending(su => su.mTimeStamp).Take(10);
		}

		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public UserDetails[] GetFriends()
		{
			int userid;
			HandleWCFAuth(out userid);

			userdbDataContext db = new userdbDataContext();

			// Add their friends
			var friends = db.GetFriends(userid).Select(u => new UserDetails()
			{
				FirstName = u.firstname,
				LastName = u.lastname,
				Prefix = "nx",
				UserId = u.id,
				LocationAllowed = db.HasLocationViewPermission(u.id, userid)//,
				//LocationId = db.Get
			}).ToList();

			db.Dispose();

			return friends.ToArray();
		}

		/// <summary>
		/// Retrieves information on the requested user. You must be friends with this user to execute this command.
		/// </summary>
		/// <param name="profileid">Target user id</param>
		[OperationContract]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public UserDetails GetUserDetails(int profileid)
		{
			int userid;
			HandleWCFAuth(out userid);

			userdbDataContext db = new userdbDataContext();

			if (!db.AreFriends(userid, profileid) && profileid != userid) // Verify that the both users are friends
			{
				// WCF can't properly serialize WebFaultExceptions so we must do it ourselves
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Forbidden;
				WebOperationContext.Current.OutgoingResponse.StatusDescription = "NotFriends";
				WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
				StreamWriter writer = new StreamWriter(HttpContext.Current.Response.OutputStream);
				writer.WriteLine("{\"isError\":True,\"error\":{\"ReasonType\":\"Security\", \"ReasonSubType\":\"Permissions\",\"ReasonCode\":\"NotFriends\"}}");
				writer.Close();
				throw WCFExceptions.NotFriends;
			}

			User usr = db.GetUser(profileid);
			int laststatus = db.GetUsersLastStatusUpdate(userid).Id;

			int locaterow = db.UserLocations.Where(ul => ul.userid == profileid).Select(ul => ul.id).FirstOrDefault();

			bool locperm = db.HasLocationViewPermission(userid, profileid);

			return new UserDetails()
			{
				FirstName = usr.firstname,
				LastName = usr.lastname,
				UserId = usr.id,
				Prefix = "nx",
				DateOfBirth = usr.DateOfBirth,
				LocationAllowed = locperm,
				LocationId = locperm ? locaterow : 0,
				LastStatusUpdate = GetStatusUpdate(laststatus)
			};
		}

		//[OperationContract]
		public int PostStatusMessage(string messageBody)
		{
			int userid;
			HandleWCFAuth(out userid);

			StatusUpdate post = new StatusUpdate();
			post.Userid = userid;
			post.MessageBody = messageBody;
			post.Timestamp = DateTime.UtcNow;

			userdbDataContext db = new userdbDataContext();
			db.StatusUpdates.InsertOnSubmit(post);
			db.SubmitChanges();

			return post.Id;
		}

		[OperationContract(Name = "PostStatusMessage")]
		public int PostStatusMessage(string messageBody, GeoLocation position)
		{
			int userid;
			HandleWCFAuth(out userid);

			StatusUpdate post = new StatusUpdate();
			post.Userid = userid;
			post.MessageBody = messageBody;
			post.Timestamp = DateTime.UtcNow;

			if (position != null && position.mAccuracy != null)
				post.GeoTagAccuracy = position.mAccuracy;

			userdbDataContext db = new userdbDataContext();
			db.StatusUpdates.InsertOnSubmit(post);
			//db.SubmitChanges();

			if (position != null)
			{
				// Linq to SQL won't properly serialize the GeoTag so we must do it ourselves
				if (position.mAltitude == 0 || !position.mAltitude.HasValue)
					db.GeoTagStatusMessage(post.Id, position.mLatitude, position.mLongitude);
				else
					db.GeoTagAltStatusMessage(post.Id, position.mLatitude, position.mLongitude, position.mAltitude);
			}

			return post.Id;
		}

		[OperationContract]
		[WebInvoke(Method = "GET")]
		[Obsolete("Will be replaced by the new OData Service", false)]
		public Stream GetUserImage(int userid, PhotoSize size)
		{
			userdbDataContext db = new userdbDataContext();
			var user = (from u in db.Users
						where u.id == userid
						select new { u.DisplayImageId, u.DefaultPhotoAcl }).FirstOrDefault();
			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;
			string etag = "";

			if (user != null)
			{
				// Check to see if the image has been modified
				etag = GenerateDisplayImgEtag(user.DefaultPhotoAcl, size);

				if (etag == request.Headers["If-None-Match"])
				{
					WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
					return null;
				}
			}

			string imgpath;

			if (user == null || user.DisplayImageId == null)
			{
				Trace.TraceWarning("MessageFeed.GetUserImage: Failed -- Image not found or user does not have image");
				imgpath = "C:\\cataduck.jpg";
			} else {
				imgpath = HttpContext.Current.Server.MapPath(String.Format("~/uploads/avatars/{0}/{1}.jpg", size.ToString().ToLower(), user.DisplayImageId));
			}

			// Example Image -- Replace with live images
			MemoryStream imageAsMemoryStream = new MemoryStream();
			Bitmap image = new Bitmap(imgpath);
			image = new Bitmap(image, (int)size, (int)size);
			image.Save(imageAsMemoryStream, ImageFormat.Jpeg);
			imageAsMemoryStream.Position = 0;

			OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
			context.ContentType = "image/jpeg";
			context.Headers.Remove("Cache-Control");

			response.Headers.Add("Etag", etag); // <-- Let's do something with this

			return imageAsMemoryStream;
		}

		private static string GenerateDisplayImgEtag(int photoid, PhotoSize size)
		{
			string etag = "";

			if (size == PhotoSize.Large)
				etag += "l";
			else if (size == PhotoSize.Medium)
				etag += "m";
			else if (size == PhotoSize.Small)
				etag += "s";

			etag += photoid.ToString();
			etag = etag.PadRight(etag.Length + (3 % etag.Length), '0');

			etag = "\"" + Convert.ToBase64String(mEncoding.GetBytes(etag)) + "\"";

			return etag;
		}

		private static void HandleWCFAuth(out int userid)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("NexusWeb: Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw WCFExceptions.CrossSiteViolation;
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			userid = (int)session["userid"];
		}
		private int[] GetCachedFriends(int userid)
		{
			userdbDataContext db = new userdbDataContext();

			if (!Settings.Default.EnableAppFabricCache)
				return db.GetFriends(userid).Select(u => u.id).ToArray();

			DataCache cache = mAppFabricFactory.GetCache("Friends");

			int[] friends = (int[])cache.Get(userid.ToString());

			if (friends == null || friends.Length == 0)
			{
				friends = db.GetFriends(userid).Select(u => u.id).ToArray();

				cache.Put(userid.ToString(), friends);
			}

			return friends;
		}
		private ClientStatusUpdate GetStatusUpdate(int articleid)
		{
			userdbDataContext db = new userdbDataContext();
			var result = (from su in db.StatusUpdates
						  where su.Id == articleid
						  select su).FirstOrDefault();

			if (result == null)
				return null;

			ClientStatusUpdate update = new ClientStatusUpdate()
			{
				mArticleId = result.Id,
				mMessageBody = result.MessageBody
			};

			update.mComments = (from c in db.ArticleComments
							   where c.ArticleId == articleid && c.ArticleType == "status"
							   select new ClientArticleComment() { mUserId = c.UserId, mMessageBody = c.MessageBody, mTimeStamp = c.TimeStamp }).ToArray();

			return update;
		}
		/// <summary>
		/// Gets a list of Status Updates matching the given parameter
		/// </summary>
		/// <param name="whereclause">Predicate to search through status updates for.</param>
		internal static IEnumerable<ClientStatusUpdate> GetStatusUpdates(Func<StatusUpdate, bool> whereclause)
		{
			userdbDataContext db = new userdbDataContext();

			return db.StatusUpdates.Where(whereclause).Select(mDbToUsableStatusUpdate);
		}
		private static IEnumerable<ClientArticleComment> GetComments(Func<ArticleComment, bool> whereclause)
		{
			userdbDataContext db = new userdbDataContext();
			return db.ArticleComments.Where(whereclause).Select(c => new ClientArticleComment() { mUserId = c.UserId, mTimeStamp = c.TimeStamp, mMessageBody = c.MessageBody });
		}

		private static Func<StatusUpdate, ClientStatusUpdate> mDbToUsableStatusUpdate;
		private static Dictionary<int, ArticlePollDelegate> mStatusUpdateCallbacks = new Dictionary<int, ArticlePollDelegate>();
		private static DataCacheFactory mAppFabricFactory;
		private static Encoding mEncoding = Encoding.Unicode;
	}
}