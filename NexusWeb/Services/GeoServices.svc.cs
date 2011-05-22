using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using NexusCore.Databases;
using Redis;
using NexusWeb.Services.DataContracts;

namespace NexusWeb.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class GeoService
	{
		[OperationContract]
		[WebGet]
		public GeoCity LatLngToCity(double latitude, double longitude)
		{
			GeoCity gcity = null;

			latitude = Math.Round(latitude, 3);
			longitude = Math.Round(longitude, 3);

			string cachekey = ComputeCacheKey(latitude, longitude);

			if (EnableGISCaching && RedisClient != null && GetCachedResult(cachekey, out gcity))
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				try {
					if (RedisClient != null && GetCachedResult(cachekey, out gcity))
						return gcity;
				} finally {
					sw.Stop();
					Trace.Write(string.Format("GeoService: Redis search took {0} seconds and found ", sw.Elapsed.TotalSeconds));
					Trace.WriteLineIf(gcity != null, "a result");
					Trace.WriteLineIf(gcity == null, "no results");
				}
			}

			GeoDataDataContext db = new GeoDataDataContext();
			
			if (gcity == null)
				gcity = new GeoCity();

			Country country = db.GetCountry(latitude, longitude).FirstOrDefault();

			if (country != null)
				gcity.Country = new GeoCountry() { ISO3 = country.ISO3, FullName = country.Name };

			if (gcity.City == null)
			{
				City city = db.GetNearestCity(latitude, longitude).FirstOrDefault();

				if (city != null)
					gcity.City = city.name;
			}

			if (gcity.AdminLevel1 == null)
			{
				AdminLevel1 level1 = db.GetAdminLevel1(latitude, longitude).FirstOrDefault();

				if (level1 != null)
					gcity.AdminLevel1 = new GeoLevel1() { Name = level1.Name, Type = GetType1FromDb(level1.Type) };
			}

			if (gcity.AdminLevel2 == null)
			{
				AdminLevel2 level2 = db.GetAdminLevel2(latitude, longitude).FirstOrDefault();

				if (level2 != null)
					gcity.AdminLevel2 = new GeoLevel2() { Name = level2.Name, Type = GetType2FromDb(level2.EnglishType) };
			}

			if (EnableGISCaching)
				SetCachedResult(cachekey, gcity);

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

		private static string ComputeCacheKey(double latitude, double longitude)
		{
			byte[] latBytes = BitConverter.GetBytes(latitude);
			byte[] lonBytes = BitConverter.GetBytes(longitude);
			byte[] key = new byte[16];

			Buffer.BlockCopy(latBytes, 0, key, 0, 8);
			Buffer.BlockCopy(lonBytes, 0, key, 8, 8);

			string strkey = Convert.ToBase64String(latBytes, Base64FormattingOptions.None);

			return strkey;
		}
		private static bool GetCachedResult(string key, out GeoCity gcityResult)
		{
			gcityResult = null;
			byte[] body = RedisClient.Get("GeoCity" + key);
			
			if (body == null)
				return false;

			string city = body[0] >= 1 ? mCacheEncoding.GetString(body, 4, body[0]) : null;
			string admlvl1 = body[1] >= 1 ? mCacheEncoding.GetString(body, 4 + body[0], body[1]) : null;
			string admlvl2 = body[2] >= 1 ? mCacheEncoding.GetString(body, 4 + body[0] + body[1], body[2]) : null;
			string country = body[3] >= 1 ? mCacheEncoding.GetString(body, 4 + body[0] + body[1] + body[2], body[3]) : null;

			gcityResult = new GeoCity();
			gcityResult.City = city;

			return true;
		}
		private static void SetCachedResult(string key, GeoCity gcity)
		{
			byte[] city = null;
			byte[] country = null;
			byte[] admlvl1 = null;
			byte[] admlvl2 = null;
			city = country = admlvl1 = admlvl2 = new byte[0];

			if (gcity.City != null)
				city = mCacheEncoding.GetBytes(gcity.City);
			if (gcity.AdminLevel1 != null)
				admlvl1 = mCacheEncoding.GetBytes(gcity.AdminLevel1.Name);
			if (gcity.AdminLevel2 != null)
				admlvl2 = mCacheEncoding.GetBytes(gcity.AdminLevel2.Name);
			if (gcity.Country != null)
				country = mCacheEncoding.GetBytes(gcity.Country.FullName);

			byte[] body = new byte[4 + city.Length + admlvl1.Length + admlvl2.Length + country.Length];
			body[0] = (byte)city.Length;
			body[1] = (byte)admlvl1.Length;
			body[2] = (byte)admlvl2.Length;
			body[3] = (byte)country.Length;

			Buffer.BlockCopy(city, 0, body, 4, city.Length);
			if (admlvl1 != null)
				Buffer.BlockCopy(admlvl1, 0, body, 4 + body[0], admlvl1.Length);
			if (admlvl2 != null)
				Buffer.BlockCopy(admlvl2, 0, body, 4 + body[0] + body[1], admlvl2.Length);
			if (country != null)
				Buffer.BlockCopy(country, 0, body, 4 + body[0] + body[1] + body[2], country.Length);

			RedisClient.Set("GeoCity" + key, body);
		}

		private static RedisClient RedisClient
		{
			get	{
				if (mRedisClient == null)
				{
					try	{
						mRedisClient = new RedisClient("192.168.56.102", 6379);
						mRedisClient.Connect();
					} catch (IOException e) {
						Trace.WriteLine("GeoService: Failed to connect to Redis cache server due to reason: " + e.Message);
						EnableGISCaching = false;
						mRedisClient = null;
					}
				}

				return mRedisClient;
			}
		}
		
		private static readonly Encoding mCacheEncoding = Encoding.UTF8; 
		private static RedisClient mRedisClient;
		private static bool EnableGISCaching = true;
	}
}