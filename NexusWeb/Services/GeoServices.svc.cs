using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using NexusCore.Databases;
using NexusWeb.Services.DataContracts;

namespace NexusWeb.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class GeoService
	{
		[OperationContract]
		public GeoCity LatLngToCity(double latitude, double longitude)
		{
			GeoDataDataContext db = new GeoDataDataContext();
			Country country = db.GetCountry(latitude, longitude).FirstOrDefault();

			City city = db.GetNearestCity(latitude, longitude).FirstOrDefault();

			GeoCity gcity = new GeoCity();

			if (country != null)
			{
				gcity.mCountry = new GeoCountry() { ISO3 = country.ISO3, FullName = country.Name };

				AdminLevel1 level1 = db.GetAdminLevel1(latitude, longitude).FirstOrDefault();

				if (level1 != null)
				{
					gcity.mAdminLevel1 = new GeoLevel1() { Name = level1.Name, Type = GetType1FromDb(level1.Type) };

					AdminLevel2 level2 = db.GetAdminLevel2(latitude, longitude).FirstOrDefault();

					if (level2 != null)
					{
						gcity.mAdminLevel2 = new GeoLevel2() { Name = level2.Name, Type = GetType2FromDb(level2.EnglishType) };
					}
				}
			}
			
			if (city != null)
				gcity.mCity = city.name;

			db.Dispose();

			return gcity;
		}

		private static GeoLevel1Type GetType1FromDb(string input)
		{
			switch (input)
			{
				case "Province":
					return GeoLevel1Type.Province;
				case "State":
					return GeoLevel1Type.State;
				default:
					return GeoLevel1Type.Other;
			}
		}
		private static GeoLevel2Type GetType2FromDb(string input)
		{
			switch (input)
			{
				case "County":
					return GeoLevel2Type.County;
				default:
					return GeoLevel2Type.Other;
			}
		}
	}
}