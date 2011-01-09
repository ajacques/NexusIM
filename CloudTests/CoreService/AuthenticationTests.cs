using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudTests.NexusCore;

namespace CloudTests.CoreServiceTests
{
	/// <summary>
	/// Summary description for Authentication
	/// </summary>
	[TestClass]
	public class AuthenticationTests
	{
		public AuthenticationTests()
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
		public void TokenLogin()
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

		[TestMethod]
		public void BadTokenLogin()
		{
			CoreServiceClient service = new CoreServiceClient();
			using (new OperationContextScope(service.InnerChannel))
			{
				try	{
					service.LoginWithToken("");
				} catch (FaultException e) {
				} finally {
					service.Close();
				}
			}
		}

		[TestMethod]
		public void NormalLogin()
		{
			CoreServiceClient service = new CoreServiceClient();
			using (new OperationContextScope(service.InnerChannel))
			{
				try	{
					service.Login("test", "test");
				} finally {
					service.Close();
				}
			}
		}
	}
}