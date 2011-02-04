using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.SessionState;
using System.IO;
using NexusWeb.Services;
using NexusWeb;
using WebTests.Properties;
using System.Data.Linq;
using NexusCore.Databases;

namespace WebTests
{
	/// <summary>
	/// Summary description for AccountService
	/// </summary>
	[TestClass]
	public class AccountServiceTest
	{
		public AccountServiceTest() {}

		private TestContext testContextInstance;
		private UnitTestSession session;

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

			session = new UnitTestSession();
			session.Add("IsUnitTest", true);

			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			SessionStateUtility.RemoveHttpSessionStateFromContext(HttpContext.Current);

			session.Clear();
			session.Abandon();
			session = null;
		}

		[TestMethod]
		public void LocationStateChange()
		{
			int userid = 3;
			HttpContext.Current.Session.Add("userid", userid);

			AccountService_Accessor service = new AccountService_Accessor();
			service.SetLocationShareState(true);

			NexusCoreDataContext db = new NexusCoreDataContext(Settings.Default.NexusCoreConnectionString);

			var result = from u in db.Users
						  where u.id == userid
						  select u.locationsharestate;

			Assert.AreEqual<bool>(true, result.First());

			service.SetLocationShareState(false);

			Assert.AreEqual<bool>(false, result.First());
		}

		[TestMethod]
		public void LoginTest()
		{
			int userid = 1;
			string logintoken = PasswordGenerator.RandomString(20);

			HttpSessionState session = HttpContext.Current.Session;
			session.Add("userid", userid);
			session.Add("logintoken", logintoken);

			AccountService_Accessor service = new AccountService_Accessor();
			service.Login("test", "test");
		}

		[TestMethod]
		public void EditAccountTest()
		{
			int userid = 3;
			int accid = 5;
			HttpContext.Current.Session.Add("userid", userid);

			AccountService_Accessor service = new AccountService_Accessor();

			service.EditIMAccount(accid, "generated", "generated");

			NexusCoreDataContext db = new NexusCoreDataContext(Settings.Default.NexusCoreConnectionString);

			var result = (from a in db.Accounts
						 where a.id == accid
						 select a).First();

			Assert.AreEqual("generated", result.username);
			Assert.AreEqual("generated", result.password);

			service.EditIMAccount(accid, "default", "default");

			db.Refresh(RefreshMode.OverwriteCurrentValues, result);

			Assert.AreEqual("default", result.username);
			Assert.AreEqual("default", result.password);

			db.Dispose();
		}
	}
}