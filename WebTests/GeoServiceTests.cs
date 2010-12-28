using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Services;
using NexusWeb.Services.DataContracts;

namespace WebTests
{
	/// <summary>
	/// Summary description for GeoServiceTests
	/// </summary>
	[TestClass]
	public class GeoServiceTests
	{
		public GeoServiceTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

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

		[TestMethod]
		public void MarquetteCityLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 46.528255;
			double longitude = -87.404898;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.AreEqual("Marquette", city.mCity);
			Assert.AreEqual("USA", city.mCountry.ISO3);
			Assert.AreEqual("Michigan", city.mAdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.State, city.mAdminLevel1.Type);
			Assert.AreEqual("Marquette", city.mAdminLevel2.Name);
			Assert.AreEqual(GeoLevel2Type.County, city.mAdminLevel2.Type);
		}

		[TestMethod]
		public void MarquetteCountyLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 46.42313;
			double longitude = -87.38894;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.IsNull(city.mCity);
			Assert.AreEqual("USA", city.mCountry.ISO3);
			Assert.AreEqual("Michigan", city.mAdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.State, city.mAdminLevel1.Type);
			Assert.AreEqual("Marquette", city.mAdminLevel2.Name);
			Assert.AreEqual(GeoLevel2Type.County, city.mAdminLevel2.Type);
		}

		[TestMethod]
		public void TorontoCityLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 43.6599;
			double longitude = -79.4030;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.AreEqual("Toronto", city.mCity);
			Assert.AreEqual("CAN", city.mCountry.ISO3);
			Assert.AreEqual("Ontario", city.mAdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.Province, city.mAdminLevel1.Type);
		}

		[TestMethod]
		public void TaiwanLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 25.0656;
			double longitude = 121.5129;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.AreEqual("Taipei", city.mCity);
			Assert.AreEqual("TWN", city.mCountry.ISO3);
			Assert.AreEqual("Taipei", city.mAdminLevel1.Name);
		}
	}
}