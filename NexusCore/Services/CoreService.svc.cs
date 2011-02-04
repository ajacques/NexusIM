using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web;
using System.Web.SessionState;
using NexusCore.Controllers;
using NexusCore.Databases;
using NexusCore.DataContracts;
using NexusCore.PushChannel;
using NexusCore.Support;

namespace NexusCore.Services
{	
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[KnownType(typeof(WCFWebPrettyFault))]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, AutomaticSessionShutdown = true)]
	public sealed class CoreService : ICoreService, ICoreServiceWinPhone, ICoreServiceLocationSlim, IDisposable
	{
		public CoreService()
		{
			db = new NexusCoreDataContext();
		}

		// HttpSessionState
		// userid : (int) Currently logged in user's id

		// ICoreService
		public void Login(string username, string password)
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] != null)
				throw WCFExceptionTypes.AlreadyAuthenticated;

			NexusCoreDataContext db = new NexusCoreDataContext();

			User mUserData = db.TryLogin(username, password);
			NexusAuditLogDataContext audit = new NexusAuditLogDataContext();

			if (mUserData != null)
			{
				session["userid"] = mUserData.id;

				OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Session", "com.nexusim.core", session.SessionID));
				
				audit.LogLoginAttempt(mUserData.id);
			} else {
				audit.LogLoginAttempt(username, password);
				throw WCFExceptionTypes.BadLogin;
			}
		}
		public void Login(string token)
		{
			// Attempts to get a user that matches the input token
			AuthToken mToken;
			User mUserData = db.TryTokenLogin(token, out mToken);
			HttpSessionState session = HttpContext.Current.Session;

			if (mUserData != null)
			{
				if (DateTime.UtcNow > mToken.expires)
				{
					db.AuthTokens.DeleteOnSubmit(mToken);
					db.SubmitChanges();
				}

				session["userid"] = mUserData.id;
				session["token"] = mToken.token;
				Trace.WriteLine("CoreService: UserToken login with generic user authentication token");
			} else {
				Device mDevice = db.TryDeviceTokenLogin(token, out mUserData);
				if (mUserData != null)
				{
					session["userid"] = mUserData.id;
					session["deviceid"] = mDevice.id;
					Trace.WriteLine("CoreService: UserToken login with device authentication token");
				} else {
					Trace.TraceError("CoreService: UserToken login failed due to invalid authentication token");
					throw new FaultException<AuthenticationException>(new AuthenticationException("Given user credentials are invalid"), new FaultReason("Given user credentials are invalid"), new FaultCode("Sender"));
				}
			}
		}
		void Login()
		{
			HttpClientCertificate certificate = HttpContext.Current.Request.ClientCertificate;
			
		}

		public void Logout()
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["deviceid"] != null)
			{
				Device row = db.GetDeviceById((int)session["device"]);
				row.lastseen = DateTime.UtcNow;
			}

			db.SubmitChanges(); // Submit any pending changes

			session.Clear();
			session.Abandon();
		}

		public bool AccountsSignedIn()
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] == null)
			{
				Trace.TraceError("CoreService: Session is not authenticated");
				throw WCFExceptionTypes.NotAuthorized;
			}

			int userid = (int)session["userid"];

			bool isImSignedIn = (from a in db.Accounts
									 where a.userid == userid 
									 && WebIMProtocolManager.StorageBin.Any(si => si.ProtocolId == a.id && si.Protocol.ProtocolStatus == InstantMessage.IMProtocolStatus.Online)
									 select a).Any();
				
				//(from u in db.Users where u.id == userid select u.IsIMSignedIn).First();

			return isImSignedIn;
		}
		public ContactInfo GetContactInfo(int contactId)
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] == null)
			{
				Trace.TraceError("CoreService: Session is not authenticated");
				throw WCFExceptionTypes.NotAuthorized;
			}

			int userid = (int)session["userid"];

			var contactRow = (from u in db.Users
							 where u.id == contactId
							 select u).FirstOrDefault();

			if (contactRow == null)
				throw new ArgumentException("The request user id does not exist");

			ContactInfo info = new ContactInfo();
			info.FirstName = contactRow.firstname;
			info.LastName = contactRow.lastname;

			if (db.AreFriends(userid, contactId).Value)
			{
				info.PhoneNumbers = contactRow.PhoneNumbers.Select(pn => new PhoneNumberStruct() { CountryCode = pn.CountryCode, Extension = pn.Extension, SubscriberNumber = pn.SubscriberNumber, Type = (PhoneType)Enum.Parse(typeof(PhoneType), pn.PhoneType) });
			}

			return info;
		}
		
		public string GenerateToken(AuthenticationTokenTypes types)
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw new FaultException();

			AuthToken token = new AuthToken();
			token.userid = (int)session["userid"];
			token.token = PasswordGenerator.RandomString(20);
			token.expires = DateTime.UtcNow.AddDays(1);

			db.AuthTokens.InsertOnSubmit(token);
			db.SubmitChanges();

			return token.token;
		}
		public IEnumerable<AccountInfo> GetAccounts()
		{
			HttpSessionState session = HttpContext.Current.Session;
			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;

			if (session["userid"] == null)
			{
				Trace.TraceError("CoreService: Session is not authenticated");
				throw WCFExceptionTypes.NotAuthorized;
			}

			int userid = (int)session["userid"];

			Trace.WriteLine("CoreService: GetAccounts Called");

			int dbVer = (from u in db.Users
						   where u.id == userid
						   select u.AccountListVersion).First();

			response.AddHeader("Version", dbVer.ToString());

			if (request.Headers.Get("Version") != null)
			{
				ushort version = Convert.ToUInt16(request.Headers["Version"]);

				if (version == dbVer)
					return new AccountInfo[] {};
			}

			var accounts = from a in db.Accounts
						   where a.userid == userid
						   select new AccountInfo(a.acctype, a.username, a.password) { mEnabled = a.enabled, mGuid = new Guid(a.guid), mAccountId = a.id };

			return accounts;
		}
		public MyAccountInformation GetMyAccountInfo()
		{
			HttpSessionState session = HttpContext.Current.Session;
			if (session["userid"] == null)
				throw new FaultException<SecurityException>(new SecurityException("This session has not been authorized to do this operation"), new FaultReason("Session has not logged-in"), new FaultCode("Sender"));

			var user = db.GetUserById((int)session["userid"]);

			return new MyAccountInformation(user.username, user.firstname);
		}
	
		private bool isAuthenticated()
		{
			var session = HttpContext.Current.Session;

			return (session["userid"] != null);
		}

		// Location Data
		public List<ContactLocationInfo> GetLocationData()
		{
			HttpSessionState session = HttpContext.Current.Session;
			if (session["userid"] == null)
			{
				Trace.TraceError("CoreService: GetLocationData failed because this session is not authenticated: (IsNewSession: " + session.IsNewSession + ")");
				throw new FaultException<SecurityException>(new SecurityException("This session has not been authorized to do this operation"), new FaultReason("Session has not logged-in"), new FaultCode("Sender"));
			}

			int userid = (int)session["userid"];

			// Gets all the UserLocations rows that this user has permission to access, and grabs the appropriate account information that is owned by the current user to communicate with the location user
			// Also filters out anybody who has disabled location sharing if they have an account
			var locations = from a in db.Accounts
							from ul in db.UserLocations
							join p in db.LocationPrivacies on ul.id equals p.locationid
							join u in db.Users on ul.userid equals u.id into sr
							from x in sr.DefaultIfEmpty()
							where p.userid == userid && a.id == p.accountid && (x == null || x.locationsharestate)
							select new { LocationData = ul, ContactInfo = p, AccInfo = a };

			// Takes the needed data from the location mash-up above, and converts it into the correct classes, then puts it in a list to return to the user
			List<ContactLocationInfo> mLocations = locations.Select(l => new ContactLocationInfo((LocationServiceType)Enum.Parse(typeof(LocationServiceType), l.LocationData.service), l.LocationData.id, new AccountInfo(l.AccInfo.acctype, l.AccInfo.username), l.LocationData.username)).ToList();

			return mLocations;
		}
		public UserLocationData GetLocation(int rowId)
		{
			HttpSessionState session = HttpContext.Current.Session;

			HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;
			if (session["userid"] == null && cookies["usrloctoken"] != null)
			{
				HttpCookie cookie = cookies["usrloctoken"];

				User userAttempted = db.TryHashLogin(Convert.ToInt32(cookie.Values["userid"]), cookie.Values["pwdhash"]);
				if (userAttempted != null)
				{
					session["userid"] = userAttempted.id;
				} else
					throw WCFExceptionTypes.NotAuthorized;
			}
						
			if (session["userid"] == null)
				throw WCFExceptionTypes.NotAuthorized;

			int userid = (int)session["userid"];

			var privacies = db.LocationPrivacies.Where(p => p.locationid == rowId && p.userid == userid);

			if (privacies.Count() == 0)
				throw new Exception("No locations matching that id");

			var ident = privacies.Join(db.UserLocations, r => r.locationid, r => r.id, (s, t) => t).First();

			LocationServiceType sType = (LocationServiceType)Enum.Parse(typeof(LocationServiceType), ident.service);

			UserLocationData data = null;
			
			try {
				data = LocationLookup.Lookup(sType, ident.identifier);
			} catch (LocationUnavailableException) {
				// WCF won't package up my exception the way I want it to do so I have to destroy all output from WCF and write it myself
				OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.SuppressEntityBody = true;

				MemoryStream stream = new MemoryStream();

				var error = new WCFWebPrettyFault() { ReasonType = "Location", ReasonSubType = "Unavailable", ReasonCode = "LocationDisabled" };

				StreamWriter writer = new StreamWriter(HttpContext.Current.Response.OutputStream);

				var serializer = new DataContractJsonSerializer(typeof(WCFWebPrettyFault));
				serializer.WriteObject(stream, error);

				stream.Seek(0, SeekOrigin.Begin);

				StreamReader reader = new StreamReader(stream);
				string d = reader.ReadToEnd().Insert(1, "\"__type\":\"WCFWebPrettyFault:#NexusCore.Support\",");

				string callback = HttpContext.Current.Request.QueryString["callback"];

				if (callback != null)
					d = callback + "(" + d + ");";
				else
					d = "{\"d\":" + d + "}";

				//d = "Sys._jsonp0({\"__type\":\"UserLocationData:#NexusCore.DataContracts\",\"Accuracy\":11056,\"LastUpdated\":\"\\/Date(1281771556000-0400)\\/\",\"Latitude\":46.536431,\"Longitude\":-87.404525,\"ReverseGeocode\":\"Marquette, MI, USA\",\"RowId\":1,\"ServiceType\":0});";

				writer.Write(d);
				//writer.WriteLine("{\"isError\":true,\"error\":{\"ReasonType\":\"Location\", \"ReasonSubType\":\"Unavailable\",\"ReasonCode\":\"RemoteServerError\"}}");

				HttpContext.Current.Response.Headers.Add("Content-Length", d.Length.ToString());
				HttpContext.Current.Response.ContentType = "application/x-javascript";

				writer.Flush();
				writer.Close();
				HttpContext.Current.Response.End();
				
				throw new Exception();
			}

			data.RowId = rowId;

			return data;
		}
		public IEnumerable<UserLocationData> GetLocations(List<int> rowIds)
		{
			// Check to see if the user has security clearance
			HttpSessionState session = HttpContext.Current.Session;
			if (session["userid"] == null)
				throw new FaultException<SecurityException>(new SecurityException("This session has not been authorized to do this operation"), new FaultReason("Session has not logged-in"), new FaultCode("Sender"));

			int userid = (int)session["userid"];

			// Get all Location rows that the current user has access to and the ones that they requested ignoring the ones that are in the location cache pending transfer to the client
			var privacies = from l in db.UserLocations
							join p in db.LocationPrivacies on l.id equals p.locationid
							where p.userid == userid &&
								rowIds.Contains(l.id)
							select new { ServiceType = (LocationServiceType)Enum.Parse(typeof(LocationServiceType), l.service), Identifier = l.identifier, RowId = l.id,  };

			if (privacies.Count() == 0)
				throw new Exception("No matching location id");

			var results = LocationLookup.LookupMultiple(LocationServiceType.GoogleLatitude, privacies.Select(s => s.Identifier));

			foreach (var result in results)
				result.Value.RowId = privacies.Where(p => p.Identifier == result.Key).Select(p => p.RowId).First();

			/*session["locationlookupsremain"] = privacies.Count();

			if (mLocationLookupEvent == null)
				mLocationLookupEvent = new ManualResetEvent(false);
			else
				mLocationLookupEvent.Reset();

			foreach (var location in privacies)
			{
				LocationLookup.BeginLookup(location.ServiceType, location.Identifier, new AsyncCallback(LocationLookup_Completed), new LocationStuff() { List = mLocationCache, State = session, Row = location.RowId });
			}

			mLocationLookupEvent.WaitOne(10000);

			mReturnCache.AddRange(mLocationCache);
			session["locationcache"] = null;*/

			return results.Select(kvp => kvp.Value);
		}

		// IDisposable
		public void Dispose()
		{
			if (db != null)
				db.Dispose();

			GC.SuppressFinalize(this);
		}

		// Unit Testing Helpers
		public User UserRow
		{
			get {
				HttpSessionState session = HttpContext.Current.Session;

				int userid = (int)session["userid"];
				return db.GetUserById(userid);
			}
		}
		public bool Authenticated
		{
			get {
				return isAuthenticated();
			}
		}

		private NexusCoreDataContext db;
	}
}