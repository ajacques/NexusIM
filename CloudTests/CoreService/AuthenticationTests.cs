using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusCore.Services;
using System.Web.SessionState;
using System.Web;
using System.Web.Hosting;
using System.IO;

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

		[TestMethod]
		public void TokenLogin()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			var testtoken = from a in db.AuthTokens
							where a.userid == 1
							select a;

			Assert.AreNotEqual(0, testtoken.Count(), "No Authentication Tokens to use");

			string token = testtoken.First().token;

			UnitTestSession session = new UnitTestSession();

			CoreService service = new CoreService();
			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);

			service.Login(token);

			service.Logout();
		}

		[TestMethod]
		public void BadTokenLogin()
		{
			UnitTestSession session = new UnitTestSession();

			CoreService service = new CoreService();
			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);
			service.Login("");
		}

		[TestMethod]
		public void NormalLogin()
		{
			UnitTestSession session = new UnitTestSession();
			CoreService service = new CoreService();
			StringWriter writer = new StringWriter();
			HttpRequest request = new HttpRequest("CoreService.svc", "http://core.nexus-im.com/Services/CoreService.svc", "");
			HttpResponse response = new HttpResponse(writer);
			UnitTestChannel channel = new UnitTestChannel();
			channel.State = CommunicationState.Opened;

			OperationContext.Current = new OperationContext(channel);

			HttpContext.Current = new HttpContext(request, response);
			
			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);
			service.Login("test", "test");
		}

		[TestMethod]
		public void CertificateTest()
		{
			UnitTestSession session = new UnitTestSession();
			CoreService service = new CoreService();
			StringWriter writer = new StringWriter();
			HttpRequest request = new HttpRequest("CoreService.svc", "http://core.nexus-im.com/Services/CoreService.svc", "");
			HttpResponse response = new HttpResponse(writer);
			HttpContext.Current = new HttpContext(request, response);
			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, session);

			
		}
	}
}