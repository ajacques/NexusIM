using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Web.SessionState;
using NexusCore.Databases;
using NexusWeb.Services.DataContracts;

namespace NexusWeb.Services
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceContract]
	[ServiceKnownType(typeof(UserDetails))]
	[ServiceKnownType(typeof(ClientStatusUpdate))]
	public sealed class ArticleFeed
	{
		[WebInvoke(Method = "GET", UriTemplate = "updates/*")]
		[OperationContract]
		public IEnumerable StatusUpdates()
		{
			Uri requestUri = HttpContext.Current.Request.Url;

			HttpSessionState session = HttpContext.Current.Session;

			int userid = (int)session["userid"];

			NameValueCollection urlParams = HttpUtility.ParseQueryString(requestUri.Query);

			NexusCoreDataContext db = new NexusCoreDataContext();
			IQueryable<StatusUpdate> query = db.StatusUpdates;

			if (urlParams["since"] != null)
			{
				string since;
				try	{
					since = urlParams["since"];
				} catch (FormatException) {
					throw new FaultException("Invalid format for url parameter 'since'. Acceptable values are id:{uint} and dt:{seconds}", new FaultCode("CLIENT", new FaultCode("URL")));
				}

				if (since.StartsWith("id:"))
				{
					int sinceId = Convert.ToInt32(since.Substring(3));

					query = query.Where(su => su.Id > sinceId);
				}
			}

			if (urlParams["userid"] != null)
			{
				int value = Convert.ToInt32(urlParams["userid"]);
				query = query.Where(su => /*db.AreFriends(userid, value).Value &&*/ su.Userid == value);
			} else	{
				query = query.Where(su => db.AreFriends(userid, su.Userid).Value || su.Userid == userid);
			}
			query = FilterRequest<StatusUpdate>(query, urlParams);
			
			query = query.OrderByDescending(su => su.Id);

			IEnumerable<object> result = query.Select(su => MessageFeed.DbStatusUpdateDelegate(su));

			if (urlParams["delay"] != null && urlParams["delay"] == "ifnone")
			{
				if (!result.Any())
				{
					SqlDependency depends = new SqlDependency();
					//depends.AddCommandDependency()
				}
			}

			return result;
		}

		[WebInvoke(Method = "GET", UriTemplate = "users/*")]
		[OperationContract]
		public IEnumerable Users()
		{
			Uri requestUri = HttpContext.Current.Request.Url;

			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptions.BadCredentials;

			int userid = (int)session["userid"];

			NameValueCollection urlParams = HttpUtility.ParseQueryString(requestUri.Query);

			NexusCoreDataContext db = new NexusCoreDataContext();
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

		internal static IQueryable<T> FilterRequest<T>(IQueryable<T> source, NameValueCollection urlParams)
		{
			if (urlParams["take"] != null)
			{
				int value = Convert.ToInt32(urlParams["take"]);
				source = source.Take(value);
			}

			return source;
		}
		internal static Expression<Func<T, bool>> StringPredicateToFunc<T>(Expression<Func<T, string>> selector, string query)
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