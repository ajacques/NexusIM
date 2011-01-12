using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using CloudTests.NexusCore;

namespace CloudTests.CoreServiceTests
{
	[TestClass]
	public class LocationTests
	{
		public LocationTests() {}

		private TestContext testContextInstance;
		private NexusCoreDataContext db;
		private CoreServiceClient service;
		private string sessioncookie = "";

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

		private CoreServiceClient HandleLogin()
		{
			CoreServiceClient service = new CoreServiceClient();

			string atoken = db.GetTestToken();

			service.LoginWithToken(atoken);

			TestContext.WriteLine("Authentication Token: " + atoken);

			return service;
		}

		[TestInitialize]
		public void TestInitialize()
		{
			db = new NexusCoreDataContext();
			service = HandleLogin();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			db.Dispose();
			service.Close();
		}

		[TestMethod]
		public void SecurityTest()
		{
			CoreServiceClient service = HandleLogin();

			List<ContactLocationInfo> cls = service.GetLocationData();

			Assert.AreNotEqual(0, cls.Count, "No Permitted Locations");

			service.Logout();
			service.Close();
		}

		[TestMethod]
		public void LocationConstraintTest()
		{
			CoreServiceClient service = HandleLogin();

			List<ContactLocationInfo> cls = service.GetLocationData();

			Assert.AreNotEqual(0, cls.Count, "Not permitted to view any user location");

			Random r = new Random();

			int index = r.Next(0, cls.Count() - 1);

			ContactLocationInfo info = cls[0];

			UserLocationData data = service.GetLocation(info.LocationId);

			Assert.IsTrue(-90 < data.Latitude && data.Latitude < 90);
			Assert.IsTrue(-180 < data.Longitude && data.Longitude < 180);

			service.Logout();
			service.Close();
		}
	}
}