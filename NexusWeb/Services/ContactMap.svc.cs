using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.SessionState;
using NexusCore.Databases;
using Redis;
using NexusWeb.Properties;

namespace NexusWeb.Services
{
	[DataContract]
	public sealed class UserLocationData
	{
		[DataMember(Name = "Latitude")]
		public double mLatitude;
		[DataMember(Name = "Longitude")]
		public double mLongitude;
		[DataMember(Name = "Accuracy")]
		public int mAccuracy;
		[DataMember(Name = "LastUpdated")]
		public DateTime mChange;
		[DataMember(Name = "ServiceType")]
		public LocationServiceType mServiceType;
		[DataMember(Name = "RowId")]
		public int mRowId;
	}

	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public sealed class ContactMap
	{
		public ContactMap()
		{
			mRedisClient = new RedisClient(Settings.Default.RedisServer);
		}

		~ContactMap()
		{
			mRedisClient.Dispose();
		}

		[OperationContract]
		[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate="All")]
		public IEnumerable<UserLocationData> QueryAllPoints()
		{
			HttpSessionState session = HttpContext.Current.Session;
			int userid = (int)session["userid"];

			RedisSet list = mRedisClient.GetSet("CLMAP_" + userid);

			IEnumerable<ContactIdData> locations = null;

			int lcount = list.Count;

			if (lcount == 0)
			{
				NexusCoreDataContext db = new NexusCoreDataContext();

				// Gets all the UserLocations rows that this user has permission to access, and grabs the appropriate account information that is owned by the current user to communicate with the location user
				// Also filters out anybody who has disabled location sharing if they have an account
				locations = from a in db.Accounts
								from ul in db.UserLocations
								join p in db.LocationPrivacies on ul.id equals p.locationid
								join u in db.Users on ul.userid equals u.id into sr
								from x in sr.DefaultIfEmpty()
								where p.userid == userid && a.id == p.accountid && (x == null || x.locationsharestate)
								select new ContactIdData() { ServiceType = (LocationServiceType)Enum.Parse(typeof(LocationServiceType), ul.service), Identifier = ul.identifier, RowId = ul.id };

				foreach (var location in locations)
				{
					byte[] buffer = new byte[location.Identifier.Length + 5];

					// {Type [1B]}{RowId [4B]}{Identifier [nB]}

					buffer[0] = (byte)location.ServiceType;

					Buffer.BlockCopy(BitConverter.GetBytes(location.RowId), 0, buffer, 1, 4);
					byte[] ident = Encoding.ASCII.GetBytes(location.Identifier);
					Buffer.BlockCopy(ident, 0, buffer, 5, ident.Length);

					list.Add(buffer);
				}
			} else {
				List<ContactIdData> dataBuffer = new List<ContactIdData>(lcount);
				foreach (byte[] buffer in list)
				{
					ContactIdData id = new ContactIdData();

					id.ServiceType = (LocationServiceType)buffer[0];
					id.RowId = BitConverter.ToInt32(buffer, 1);
					id.Identifier = Encoding.ASCII.GetString(buffer, 5, buffer.Length - 5);

					dataBuffer.Add(id);
				}
				locations = dataBuffer;
			}
			
			IDictionary<string, byte[]> cache = mRedisClient.Get(locations.Select(p => "CLPointCache_" + p.RowId));
			List<UserLocationData> answer = new List<UserLocationData>(lcount);

			IEnumerable<ContactIdData> input = locations.Where(p => cache["CLPointCache_" + p.RowId] == null);
			IAsyncResult lookupWait = null;
			if (input.Any())
				lookupWait = LocationLookup.BeginLookupMultiple(LocationServiceType.GoogleLatitude, input.Select(cid => cid.Identifier), new AsyncCallback(CacheResultsToRedis), input);

			foreach (var cacheItem in cache)
			{
				if (cacheItem.Value != null)
				{
					byte[] block = cacheItem.Value;
					
					UserLocationData locdata = new UserLocationData();
					locdata.mLatitude = BitConverter.ToDouble(block, 0);
					locdata.mLongitude = BitConverter.ToDouble(block, 8);
					locdata.mAccuracy = BitConverter.ToInt32(block, 16);
					locdata.mChange = DateTime.FromBinary(BitConverter.ToInt64(block, 20));
					locdata.mRowId = BitConverter.ToInt32(block, 28);

					answer.Add(locdata);
				}
			}

			if (lookupWait != null)
			{
				IDictionary<string, UserLocationData> newResults = LocationLookup.EndLookupMultiple(lookupWait);

				foreach (var result in newResults)
					result.Value.mRowId = input.Where(p => p.Identifier == result.Key).Select(p => p.RowId).First();

				answer.AddRange(newResults.Values);
			}

			return answer;
		}

		private void CacheResultsToRedis(IAsyncResult e)
		{
			IEnumerable<ContactIdData> data = (IEnumerable<ContactIdData>)e.AsyncState;
			IDictionary<string, UserLocationData> results = LocationLookup.EndLookupMultiple(e);

			foreach (var result in results)
			{
				result.Value.mRowId = data.Where(p => p.Identifier == result.Key).Select(p => p.RowId).First();

				byte[] block = new byte[32];

				Buffer.BlockCopy(BitConverter.GetBytes(result.Value.mLatitude), 0, block, 0, 8);
				Buffer.BlockCopy(BitConverter.GetBytes(result.Value.mLongitude), 0, block, 8, 8);
				Buffer.BlockCopy(BitConverter.GetBytes(result.Value.mAccuracy), 0, block, 16, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(result.Value.mChange.ToBinary()), 0, block, 20, 8);
				Buffer.BlockCopy(BitConverter.GetBytes(result.Value.mRowId), 0, block, 28, 4);

				mRedisClient.Set("CLPointCache_" + result.Value.mRowId, block, TimeSpan.FromMinutes(5));
			}
		}

		private sealed class ContactIdData
		{
			public LocationServiceType ServiceType;
			public string Identifier;
			public int RowId;
		}

		private RedisClient mRedisClient;
	}
}