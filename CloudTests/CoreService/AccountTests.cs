using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusCore.Services;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.ServiceModel;

namespace CloudTests.CoreServiceTests
{
	/// <summary>
	/// Summary description for AccountTests
	/// </summary>
	[TestClass]
	public class AccountTests
	{
		private TestContext testContextInstance;

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
			UnitTestSession session = new UnitTestSession();

			Stream stream = new MemoryStream();
			TextWriter writer = new StreamWriter(stream);
			HttpResponse response = new HttpResponse(writer);
			HttpRequest request = new HttpRequest("unittesting", "http://im.adrensoftware.com/unittesting", "");
			HttpContext.Current = new HttpContext(request, response);
			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);
		}

		[TestCleanup]
		public void TestCleanup()
		{
			SessionStateUtility.RemoveHttpSessionStateFromContext(HttpContext.Current);
			HttpContext.Current = null;
		}

		[TestMethod]
		public void EncryptedPasswordTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			var testtoken = from a in db.AuthTokens
							where a.userid == 1
							select a;

			Assert.AreNotEqual(0, testtoken.Count(), "No Authentication Tokens to use");

			string token = testtoken.First().token;
			
			CoreService service = new CoreService();
			service.Login(token);

			var accounts = service.GetAccounts();

			Assert.AreNotEqual<int>(0, accounts.Count());

			foreach (var account in accounts)
			{
				Assert.IsNotNull(account.mEncPassword, "Encrypted Password is null");
				Assert.IsNull(account.Password);
			}

			service.Logout();
		}

		[TestMethod]
		public void DecryptedPasswordTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			//UnitTestChannel channel = new UnitTestChannel();
			//channel.State = CommunicationState.Opened;

			//OperationContext.Current = new OperationContext(channel);

			CoreService service = new CoreService();
			service.Login("test", "test");

			var accounts = service.GetAccounts();

			Assert.AreNotEqual<int>(0, accounts.Count());

			foreach (var account in accounts)
			{
				Assert.IsNull(account.mEncPassword, "Encrypted Password is null");
				Assert.IsNotNull(account.Password);
			}

			service.Logout();
		}

		[TestMethod]
		public void ConnectTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			var testtoken = from a in db.AuthTokens
							where a.userid == 1
							select a;

			Assert.AreNotEqual(0, testtoken.Count(), "No Authentication Tokens to use");

			string token = testtoken.First().token;

			CoreService service = new CoreService();

			service.Login(token);
		}
	}
}
