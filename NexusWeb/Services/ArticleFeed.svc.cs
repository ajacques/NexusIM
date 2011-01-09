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
using System.Collections.Specialized;
using System.Reflection;

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

			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptions.BadCredentials;

			int userid = (int)session["userid"];

			NameValueCollection urlParams = HttpUtility.ParseQueryString(requestUri.Query);

			userdbDataContext db = new userdbDataContext();
			IQueryable<StatusUpdate> query = db.StatusUpdates;

			if (urlParams["userid"] != null)
			{
				int value = Convert.ToInt32(urlParams["userid"]);
				query = query.Where(su => su.Userid == value);
			} else	{
				query = query.Where(su => db.AreFriends(userid, su.Userid).Value);
			}
			query = FilterRequest<StatusUpdate>(query, urlParams);
			
			return query.Select(su => MessageFeed.DbStatusUpdateDelegate(su));
		}

		[WebInvoke(Method = "GET", UriTemplate = "users/*")]
		[OperationContract]
		public IEnumerable<UserDetails> Users()
		{
			Uri requestUri = HttpContext.Current.Request.Url;

			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptions.BadCredentials;

			int userid = (int)session["userid"];

			NameValueCollection urlParams = HttpUtility.ParseQueryString(requestUri.Query);

			userdbDataContext db = new userdbDataContext();
			IQueryable<User> query = db.Users;

			string fullNameQuery = urlParams["fullname"];
			if (fullNameQuery != null)
			{
				Expression<Func<User, bool>> whereQuery = StringPredicateToFunc<User>(u => u.firstname + " " + u.lastname, fullNameQuery);
				query = query.Where(whereQuery);
			}

			query = FilterRequest<User>(query, urlParams); // Generic Filters for all types

			IEnumerable<UserDetails> result = query.Select(u => new UserDetails(u));
			return result;
		}

		private static IQueryable<T> FilterRequest<T>(IQueryable<T> source, NameValueCollection urlParams)
		{
			if (urlParams["take"] != null)
			{
				int value = Convert.ToInt32(urlParams["take"]);
				source = source.Take(value);
			}

			return source;
		}
		private static Expression<Func<T, bool>> StringPredicateToFunc<T>(Expression<Func<T, string>> selector, string query)
		{
			ParameterExpression dbParam = Expression.Parameter(typeof(T)); // Row from the database to search from
			if (query.StartsWith("contains("))
			{
				string part = query.Substring(10, query.Length - 12);
				return Expression.Lambda<Func<T, bool>>(
					Expression.Call(
						Expression.Invoke(selector, dbParam), typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(part)
					), dbParam
				);
			} else {
				return Expression.Lambda<Func<T, bool>>( // Returns true if the input data specified by selector equals the query parameter
					Expression.Equal(
						Expression.Invoke(selector, dbParam), Expression.Constant(query)
					), dbParam
				);
			}
		}
	}
}