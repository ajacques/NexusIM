using System;
using System.Linq;
using Microsoft.SqlServer.Types;
using System.Data.Common;
using System.Text;

namespace NexusCore.Databases
{
	partial class GeoDataDataContext
	{
		public string LatLngToPrettyString(double latitude, double longitude)
		{
			Country country = GetCountry(latitude, longitude).FirstOrDefault();

			string admin2 = null;

			if (country.ISO3 == "USA") // Special Handling for USA
			{
				USA_State state = GetUSAState(latitude, longitude).FirstOrDefault();

				if (state != null)
				{
					admin2 = state.Name;
				}
			}

			City city = GetNearestCity(latitude, longitude).FirstOrDefault();

			StringBuilder builder = new StringBuilder();
			if (city != null)
				builder.Append(city.name + ", ");

			if (!String.IsNullOrEmpty(admin2))
				builder.Append(admin2);

			return builder.ToString();
		}
	}
}