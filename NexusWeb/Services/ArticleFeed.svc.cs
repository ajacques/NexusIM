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
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Activation;
using System.Xml;
using System.Collections;
using System.Data.Linq;
using System.Web.SessionState;
using System.Web.Script.Services;
using System.Linq.Expressions;

namespace NexusWeb.Services
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceContract]
	public class ArticleFeed
	{
		[WebInvoke(Method = "GET", UriTemplate = "updates/*")]
		[OperationContract]
		public IEnumerable StatusUpdates()
		{
			Uri requestUri = HttpContext.Current.Request.Url;
			Dictionary<string, string> expression = null;

			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptions.BadCredentials;

			int userid = (int)session["userid"];

			try	{
				expression = requestUri.Query.Substring(1).Split('&').ToDictionary(s => s.Substring(0, s.IndexOf('=')), s => s.Substring(s.IndexOf('=') + 1)); // Break down the url query into each separate part
			} catch (ArgumentOutOfRangeException) {
				expression = new Dictionary<string, string>();
			}

			userdbDataContext db = new userdbDataContext();
			IQueryable<StatusUpdate> query = db.StatusUpdates;

			if (expression.ContainsKey("userid"))
			{
				int value = Convert.ToInt32(expression["userid"]);
				query = query.Where(su => su.Userid == value);
			} else	{
				query = query.Where(su => db.AreFriends(userid, su.Userid).Value);
			}
			query = FilterRequest<StatusUpdate>(query, expression);
			
			return query.Select(su => MessageFeed.DbStatusUpdateDelegate(su));
		}

		[WebInvoke(Method = "GET", UriTemplate = "users/*")]
		[OperationContract]
		public IEnumerable Users()
		{
			Uri requestUri = HttpContext.Current.Request.Url;
			Dictionary<string, string> expression = null;

			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptions.BadCredentials;

			int userid = (int)session["userid"];

			try	{
				expression = requestUri.Query.Substring(1).Split('&').ToDictionary(s => s.Substring(0, s.IndexOf('=' )), s => s.Substring(s.IndexOf('=') + 1)); // Break down the url query into each separate part
			} catch (ArgumentOutOfRangeException) {
				expression = new Dictionary<string, string>();
			}

			userdbDataContext db = new userdbDataContext();
			IQueryable<User> query = db.Users;

			if (expression.ContainsKey("fullname"))
			{
				string predicate = expression["fullname"];
				query = query.Where(u => StringPredicateToFunc(predicate)(u.firstname));
			}

			query = FilterRequest<User>(query, expression);

			return query.Select(u => new UserDetails(u));
		}

		private static IQueryable<T> FilterRequest<T>(IQueryable<T> source, Dictionary<string, string> urlParams)
		{
			if (urlParams.ContainsKey("take"))
			{
				int value = Convert.ToInt32(urlParams["take"]);
				source = source.Take(value);
			}

			return source;
		}

		private static Predicate<string> StringPredicateToFunc(string predicate)
		{
			// (u.firstname + " " + u.lastname)
			if (predicate.StartsWith("contains("))
			{
				string part = predicate.Substring(9, predicate.Length - 11);
				return (str) => str.Contains(part);
			} else {
				return (s) => s == predicate;
			}
		}
	}
}