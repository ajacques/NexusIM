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

namespace WebTests
{
	/// <summary>
	/// Summary description for SecurityTests
	/// </summary>
	[TestClass]
	public class AccountServiceSecurityTests
	{
		public AccountServiceSecurityTests()
		{
			
		}

		private TestContext testContextInstance;

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext
		{
			get	{
				return testContextInstance;
			}
			set	{
				testContextInstance = value;
			}
		}

		[TestMethod]
		public void AntiCSRF()
		{
			Stream stream = new MemoryStream();
			TextWriter writer = new StreamWriter(stream);
			HttpResponse response = new HttpResponse(writer);
			HttpRequest request = new HttpRequest("badfile.html", "http://www.badsite.com/badfile.html", "");

			HttpContext.Current = new HttpContext(request, response);

			SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, new UnitTestSession());

			int userid = 3;
			HttpContext.Current.Session.Add("userid", userid);

			AccountService service = new AccountService();
			service.SetLocationShareState(false);
		}
	}
}
