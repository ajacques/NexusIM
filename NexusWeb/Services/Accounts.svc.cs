﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.SessionState;
using Microsoft.ApplicationServer.Caching;
using NexusWeb.AdminChannel;
using NexusWeb.Databases;
using System.Runtime.Serialization;
using System.IO;

namespace NexusWeb.Services
{
	[DataContract(Namespace = "")]
	public enum UserGender
	{
		Unspecified,
		[EnumMember]
		Male,
		[EnumMember]
		Female
	}

	[DataContract(Namespace = "")]
	public class DateOfBirth
	{
		[DataMember]
		public int Month
		{
			get;
			set;
		}
		[DataMember]
		public int Day
		{
			get;
			set;
		}
		[DataMember]
		public int Year
		{
			get;
			set;
		}
		public DateTime ToDateTime()
		{
			return new DateTime(Year, Month, Day);
		}
		public TimeSpan Age
		{
			get	{
				return DateTime.Now.Subtract(ToDateTime());
			}
		}
	}

	/// <summary>
	/// Provides access for web clients to control their account in a standardized and secure way.
	/// </summary>
	[ServiceContract(Namespace = "")]
	[ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public sealed class AccountService
	{
		public AccountService()
		{
			//db = new userdbDataContext();
			//mAppFabricFactory = new DataCacheFactory();
		}

		[OperationContract]
		public void Login(string username, string password)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("NexusWeb: Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw WCFExceptions.CrossSiteViolation;
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] != null)
				return;

			userdbDataContext db = new userdbDataContext();
			var userrow = db.TryLogin(username, password);
			NexusAuditLogDataContext audit = new NexusAuditLogDataContext();

			if (userrow == null)
			{
				Trace.WriteLine("NexusWeb: Invalid login credentials");

				db.Dispose();

				audit.LogLoginAttempt(username, null);

				throw WCFExceptions.BadCredentials;
			}
			audit.LogLoginAttempt(userrow.id);

			session.Add("userid", userrow.id);
			session.Add("username", userrow.username);

			db.Dispose();
		}

		[OperationContract]
		public void Logout()
		{
			HttpSessionState session = HttpContext.Current.Session;
			session.Clear();
			session.Abandon();
		}

		[OperationContract]
		public void LoginHashedPassword(string username, string passwordhash)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("NexusWeb: Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw WCFExceptions.CrossSiteViolation;
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] != null)
				return;

			userdbDataContext db = new userdbDataContext();
			var userrow = (from u in db.Users
						   where u.username == username && u.password == passwordhash
						   select new {id = u.id, username = u.username}).FirstOrDefault();
			NexusAuditLogDataContext audit = new NexusAuditLogDataContext();

			if (userrow == null)
			{
				Trace.WriteLine("NexusWeb: Invalid login credentials");

				db.Dispose();
								
				audit.LogLoginAttempt(username, null);

				throw WCFExceptions.BadCredentials;
			}
			audit.LogLoginAttempt(userrow.id);

			session.Add("userid", userrow.id);
			session.Add("username", userrow.username);

			db.Dispose();
		}
		
		[OperationContract]
		public void CreateAccount(string firstName, string lastName, string email, string password, UserGender gender, DateTime dob)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("NexusWeb: Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw WCFExceptions.CrossSiteViolation;
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] != null)
				return;

			userdbDataContext db = new userdbDataContext();
			User userRow = new User();
			userRow.username = email;
			userRow.firstname = firstName;
			userRow.lastname = lastName;
			userRow.password = db.HashString(password);
			//userRow.dateregistered = DateTime.UtcNow;
			//userRow.DateOfBirth = dob;
			
			db.Users.InsertOnSubmit(userRow);
			db.SubmitChanges();

			session.Add("userid", userRow.id);
			session.Add("username", userRow.username);
		}

		[OperationContract]
		public void StopCustomLocationSharing(int locationId)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("Attempted WCF call from outside secure website boundaries (Method: AccountService.StopLocationSharing): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw new FaultException<SecurityException>(new SecurityException("Attempted HTTP request outside of secure website boundaries"), new FaultReason("Attempted HTTP request outside of secure website boundaries"), new FaultCode("Sender"));
			}

			Dictionary<string, string> data = new Dictionary<string, string>();

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			var rows = from lp in db.LocationPrivacies
					   where lp.locationid == locationId
					   select lp;

			if (rows.Count() == 0)
				throw new FaultException();
			
			var row = rows.First();

			if (rows.Count() == 1)
				db.UserLocations.DeleteOnSubmit(db.UserLocations.First(ul => ul.id == row.locationid));

			db.LocationPrivacies.DeleteOnSubmit(row);
		}

		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		public void SetLocationShareState(bool enabled)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("Attempted WCF call from outside secure website boundaries (Method: AccountService.SetLocationShareState): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw new FaultException<SecurityException>(new SecurityException("Attempted HTTP request outside of secure website boundaries"), new FaultReason("Attempted HTTP request outside of secure website boundaries"), new FaultCode("Sender"));
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				throw new FaultException<AuthenticationException>(new AuthenticationException("Session is not authenticated"), new FaultReason("Session is not authenticated"), new FaultCode("Security", new FaultCode("NotLoggedIn")));
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			var userStates = from u in db.Users
							 where u.id == userid
							 select u;

			var state = userStates.FirstOrDefault();

			if (state == null)
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			} else {
				state.locationsharestate = enabled;
				db.SubmitChanges();
			}
		}

		[OperationContract]
		[FaultContract(typeof(ArgumentException))]
		public void ChangeAllUsersProtocolStatuses(string newStatus)
		{
			IMStatus status;
			try	{
				status = (IMStatus)Enum.Parse(typeof(IMStatus), newStatus, true);
			} catch (ArgumentException) { // We catch this exception to prevent any unwanted data from returning to the client
				throw new FaultException(new FaultReason("Failed to parse " + newStatus + " into type IMStatus"));
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null) // Make sure the user is logged-in
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			AdminChannelClient client = new AdminChannelClient();
			client.ChangeAllProtocolsStatusForAccount(userid, status);
			client.Close();
		}

		[OperationContract]
		public void CreateIMAccount(string acctype, string username, string password)
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null) // Make sure the user is logged-in
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			Account row = new Account();
			row.acctype = acctype;
			row.username = username;
			row.password = password;

			db.Accounts.InsertOnSubmit(row);
			db.SubmitChanges();

			AdminChannelClient client = new AdminChannelClient();
			client.BroadcastNewAccount(row.id);
			client.Close();
		}

		/// <summary>
		/// Modifies the properties of the account id to the new settings.
		/// </summary>
		/// <param name="accountid">Row id that corresponds to the account that should be updated.</param>
		/// <param name="username">The account's username will be changed to this.</param>
		/// <param name="password">If this is not null or empty, the account's password will be changed to this</param>
		[OperationContract]
		public void EditIMAccount(int accountid, string username, string password)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("Attempted WCF call from outside secure website boundaries (Method: AccountService.EditAccount): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw new FaultException<SecurityException>(new SecurityException("Attempted HTTP request outside of secure website boundaries"), new FaultReason("Attempted HTTP request outside of secure website boundaries"), new FaultCode("Sender"));
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null) // Make sure the user is logged-in
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			var account = from a in db.Accounts
						  where a.userid == userid && a.id == accountid
						  select a;

			var acc = account.FirstOrDefault();

			if (acc != null)
			{
				if (acc.username != username) // Do this to prevent Linq to SQL from thinking we changed and it uselessly sending an update to the database
					acc.username = username;
				if (!String.IsNullOrEmpty(password) && acc.password != password)
					acc.password = password;
				db.SubmitChanges();
			}

			db.Dispose();
		}

		[OperationContract]
		public void DeleteIMAccount(int accountid)
		{
			if (!CounterCSRF.IsValidFromSecureWCF())
			{
				Trace.TraceError(String.Format("Attempted WCF call from outside secure website boundaries (Method: AccountService.DeleteAccount): {0}", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
				throw new FaultException<SecurityException>(new SecurityException("Attempted HTTP request outside of secure website boundaries"), new FaultReason("Attempted HTTP request outside of secure website boundaries"), new FaultCode("Sender"));
			}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null) // Make sure the user is logged-in
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			var account = (from a in db.Accounts
						  where a.userid == userid && a.id == accountid
						  select a).FirstOrDefault();

			db.Accounts.DeleteOnSubmit(account);
			db.SubmitChanges();
			db.Dispose();
		}

		[OperationContract]
		public void ChangeAccountEnabledStatus(IDictionary<int, bool> states)
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if (session["userid"] == null) // Make sure the user is logged-in
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.Close();
			}

			int userid = (int)session["userid"];

			userdbDataContext db = new userdbDataContext();

			// Lots 'o magic here. Grab all the rows for the requested state changes and filter out any accounts that don't belong to this user
			var accounts = from a in db.Accounts
						   where a.userid == userid && states.Select(kv => kv.Key).Contains(a.id)
						   select a;

			if (accounts.Count() != states.Count())
			{
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				response.StatusDescription = "Attempted to modify account state for another user's account";
				response.Close();
			}
			
			foreach (var account in accounts)
				account.enabled = states[account.id];

			db.SubmitChanges();
		}
	}
}