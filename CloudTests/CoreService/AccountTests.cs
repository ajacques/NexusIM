using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudTests.NexusCore;

namespace CloudTests.CoreServiceTests
{
	/// <summary>
	/// Summary description for AccountTests
	/// </summary>
	[TestClass]
	public class AccountTests
	{
		public AccountTests()
		{
			db = new NexusCoreDataContext();
		}

		private TestContext testContextInstance;
		private NexusCoreDataContext db;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get {
				return testContextInstance;
			}
			set	{
				testContextInstance = value;
			}
		}

		[TestInitialize]
		public void TestInitialize()
		{
			db = new NexusCoreDataContext();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			db.Dispose();
		}

		[TestMethod]
		public void CountTest()
		{
			var testtoken = from a in db.AuthTokens
							where a.userid == 1
							select a;

			Assert.AreNotEqual(0, testtoken.Count(), "No Authentication Tokens to use");

			string token = testtoken.First().token;

			CoreServiceClient service = new CoreServiceClient();

			service.LoginWithToken(token);

			Assert.AreNotEqual<int>(0, service.GetAccounts().Count());

			service.Logout();
			service.Close();
		}

		[TestMethod]
		public void ConnectTest()
		{
			var testtoken = from a in db.AuthTokens
							where a.userid == 1
							select a;

			Assert.AreNotEqual(0, testtoken.Count(), "No Authentication Tokens to use");

			string token = testtoken.First().token;

			CoreServiceClient service = new CoreServiceClient();

			service.LoginWithToken(token);

			service.Logout();
			service.Close();
		}
	}
}
