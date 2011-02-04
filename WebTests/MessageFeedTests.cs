using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusCore.Databases;
using NexusWeb.Services;
using NexusWeb.Services.DataContracts;
using System.Data.SqlClient;

namespace WebTests
{
	/// <summary>
	/// Summary description for AccountService
	/// </summary>
	[TestClass]
	public class MessageFeedTest
	{
		public MessageFeedTest() {}

		private TestContext testContextInstance;
		private UnitTestSession mLocalSession;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get	{
				return testContextInstance;
			}
			set	{
				testContextInstance = value;
			}
		}

		[TestInitialize]
		public void MyTestInitialize()
		{
			Stream stream = new MemoryStream();
			TextWriter writer = new StreamWriter(stream);
			HttpResponse response = new HttpResponse(writer);
			HttpRequest request = new HttpRequest("unittesting", "http://dev.nexus-im.com/unittesting", "");
			HttpContext.Current = new HttpContext(request, response);

			UnitTestSession session = new UnitTestSession();
			session.Add("IsUnitTest", true);
			mLocalSession = session;

			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			mLocalSession = null; // Each Test should be completely isolated from each other. Prevent data bleedover
			SessionStateUtility.RemoveHttpSessionStateFromContext(HttpContext.Current);
		}

		[TestMethod]
		public void UserDetailsTest()
		{
			int userid = 3;
			MessageFeed feed = new MessageFeed();

			mLocalSession.Add("userid", 1);

			UserDetails details = feed.GetUserDetails(userid);
			Assert.AreEqual(userid, details.UserId);
			Assert.AreEqual("Abraham", details.FirstName);
			Assert.AreEqual("Lincoln", details.LastName);
			Assert.AreEqual("nx", details.Prefix);
		}

		[TestMethod]
		public void NoFriendUserDetailsTest()
		{
			HttpContext.Current.Session.Add("userid", 1);
			bool didThrow = false;

			// By Design this should throw an exception
			MessageFeed feed = new MessageFeed();
			try {
				feed.GetUserDetails(4);
			} catch (Exception) {
				didThrow = true;
			}

			if (!didThrow)
				throw new SecurityException("MessageFeed.GetUserDetails() Allowed user to get data on a non-friend");
		}

		[TestMethod]
		public void FriendCacheTest()
		{
			var feed = new MessageFeed_Accessor();

			NexusCoreDataContext db = new NexusCoreDataContext();

			if (feed.GetCachedFriends(1).Count() != db.GetFriends(1).Count())
				throw new Exception("Cached Friends Count does not match DB-stored friend count");
		}

		[TestMethod]
		public void PostSMessageTest()
		{
			int userid = 1;
			mLocalSession.Add("userid", 1);

			var feed = new MessageFeed();

			string msg = "UNITTESTMESSAGE19994749";

			feed.PostStatusMessage(msg);

			NexusCoreDataContext db = new NexusCoreDataContext();
			var result = db.StatusUpdates.Where(su => su.MessageBody == msg && su.Userid == userid);

			db.StatusUpdates.DeleteAllOnSubmit(result);
			db.SubmitChanges();

			Assert.AreNotEqual(0, result.Count());
		}

		/// <summary>
		/// Tests GeoTag Status Message Posting to make sure all data gets properly serialized to database
		/// </summary>
		[TestMethod]
		public void PostSMessageTestGtagNoAlt()
		{
			int userid = 1;
			mLocalSession.Add("userid", 1);

			var feed = new MessageFeed();

			string msg = "UNITTESTMESSAGE19994749";

			var geoloc = new GeoLocation();
			geoloc.mLatitude = 46;
			geoloc.mLongitude = 87;
			
			feed.PostStatusMessage(msg, geoloc);

			NexusCoreDataContext db = new NexusCoreDataContext();
			var result = db.StatusUpdates.Where(su => su.MessageBody == msg && su.Userid == userid).FirstOrDefault();

			db.StatusUpdates.DeleteOnSubmit(result);
			db.SubmitChanges();

			Assert.IsNotNull(result);
			Assert.AreEqual(geoloc.mLatitude, result.GeoTag.Lat);
			Assert.AreEqual(geoloc.mLongitude, result.GeoTag.Long);		
		}
	}
}