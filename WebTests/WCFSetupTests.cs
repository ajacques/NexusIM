using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebTests.Properties;

namespace WebTests
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
		public void MessageFeedTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/MessageFeed.svc"));
		}

		[TestMethod]
		public void DevicesTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/Devices.svc"));
		}

		[TestMethod]
		public void AccountsTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/Accounts.svc"));
		}

		[TestMethod]
		public void ValidationFuncsTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/ValidationFunctions.svc"));
		}

		[TestMethod]
		public void SocialNetworksTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/SocialNetworks.svc"));
		}

		[TestMethod]
		public void PhotoServiceTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/Photos.svc"));
		}

		[TestMethod]
		public void GeoServiceTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/GeoServices.svc"));
		}

		[TestMethod]
		public void ArticleFeedTest()
		{
			TestUri(new Uri(Settings.Default.NexusWebRootUri, "/Services/ArticleFeed.svc"));
		}
	}
}