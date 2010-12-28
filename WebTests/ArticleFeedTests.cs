﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Services;
using System.IO;
using System.Web;
using System.Web.SessionState;
using NexusWeb.Databases;
using System.ServiceModel.Web;
using System.Security;

namespace WebTests
{
	/// <summary>
	/// Summary description for ArticleFeedTests
	/// </summary>
	[TestClass]
	public class ArticleFeedTests
	{
		public ArticleFeedTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

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
			HttpRequest request = new HttpRequest("unittesting", "http://im.adrensoftware.com/unittesting", "");
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

		/// <summary>
		/// Verifies that we are friends with all status updates that are returned.
		/// </summary>
		[TestMethod]
		public void NotFriendsTest()
		{
			int userid = 1;
			mLocalSession.Add("userid", userid);

			ArticleService service = new ArticleService();

			userdbDataContext db = new userdbDataContext();
			IQueryable<int> friends = db.GetFriends(userid).Select(u => u.id);

			var results = service.StatusUpdates.Any(cs => !friends.Contains(cs.UserId));

			Assert.IsFalse(results);
		}

		[TestMethod]
		[ExpectedException(typeof(WebFaultException<string>))]
		public void NotLoggedInTest()
		{
			ArticleService service = new ArticleService();

			bool result = service.StatusUpdates.Any();
		}
	}
}