using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Services;
using System.IO;
using System.Web;
using System.Web.SessionState;
using NexusCore.Databases;
using System.ServiceModel.Web;
using System.Security;
using System.Linq.Expressions;

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
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
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
			session.Add("userid", 1);
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
		public void SimpleStringExpressionTest()
		{
			string[] testdata = { "HTC", "Motorola", "Apple", "Samsung" };
			Func<string, bool> expression = ArticleFeed.StringPredicateToFunc((string s) => s, "contains('sung')").Compile();
			
			string actual = testdata.First(expression);

			Assert.AreEqual("Samsung", actual);

			expression = ArticleFeed.StringPredicateToFunc((string s) => s, "Apple").Compile();

			actual = testdata.First(expression);

			Assert.AreEqual("Apple", actual);
		}

		[TestMethod]
		public void SimpleExpressionTest()
		{
			var accessor = new ArticleFeed_Accessor();
			accessor.Users();
		}
	}
}