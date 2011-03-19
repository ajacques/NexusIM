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

			Assert.AreEqual("Marquette", city.City);
			Assert.AreEqual("USA", city.Country.ISO3);
			Assert.AreEqual("Michigan", city.AdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.State, city.AdminLevel1.Type);
			Assert.AreEqual("Marquette", city.AdminLevel2.Name);
			Assert.AreEqual(GeoLevel2Type.County, city.AdminLevel2.Type);
		}

		[TestMethod]
		public void MarquetteCountyLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 46.42313;
			double longitude = -87.38894;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.IsNull(city.City);
			Assert.AreEqual("USA", city.Country.ISO3);
			Assert.AreEqual("Michigan", city.AdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.State, city.AdminLevel1.Type);
			Assert.AreEqual("Marquette", city.AdminLevel2.Name);
			Assert.AreEqual(GeoLevel2Type.County, city.AdminLevel2.Type);
		}

		[TestMethod]
		public void TorontoCityLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 43.6599;
			double longitude = -79.4030;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.AreEqual("Toronto", city.City);
			Assert.AreEqual("CAN", city.Country.ISO3);
			Assert.AreEqual("Ontario", city.AdminLevel1.Name);
			Assert.AreEqual(GeoLevel1Type.Province, city.AdminLevel1.Type);
		}

		[TestMethod]
		public void TaiwanLocationTest()
		{
			GeoService service = new GeoService();

			double latitude = 25.0656;
			double longitude = 121.5129;

			GeoCity city = service.LatLngToCity(latitude, longitude);

			Assert.AreEqual("Taipei", city.City);
			Assert.AreEqual("TWN", city.Country.ISO3);
			Assert.AreEqual("Taipei", city.AdminLevel1.Name);
		}
	}
}