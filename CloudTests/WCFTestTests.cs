using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudTests.Properties;

namespace CloudTestsTests
{
	/// <summary>
	/// Validates that the specified WCF services are coded correctly so that the WSDL can be generated.
	/// Very Generic method of checking to see if they are working.
	/// </summary>
	[TestClass]
	public class WCFSetupTests
	{
		private void TestUri(Uri uri)
		{
			HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;

			if (response.StatusCode != HttpStatusCode.OK)
				throw new WebException("WCF Service Contract Generation failed..", new Exception(), WebExceptionStatus.UnknownError, response);
		}

		[TestMethod]
		public void CoreServerWSTest()
		{
			TestUri(new Uri(Settings.Default.NexusCoreBaseUri, "/Services/CoreService.svc"));
		}

		[TestMethod]
		public void CoreServiceJSTest()
		{
			TestUri(new Uri(Settings.Default.NexusCoreBaseUri, "/Services/CoreService.svc/locjs/js"));
		}
	}
}