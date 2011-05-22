using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Redis;
using NexusWeb.Properties;

namespace NexusWeb.BackgroundCode
{
	public sealed class RedisSessionStateProvider : SessionStateStoreProviderBase
	{
		public RedisSessionStateProvider()
		{
			mRedisClient = new RedisClient(Settings.Default.RedisServer);
			try {
				mRedisClient.Connect();
				mRedisClient.ChangeDatabase(2);
			} catch (Exception e) {

			}
		}

		public override void Dispose()
		{
			if (mRedisClient != null)
				mRedisClient.Dispose();
		}

		public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
		{
			return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
		}
		public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			hash.Set("Flags", new byte[] { 1 }); // Not initialized yet
			hash.Set("Timeout", BitConverter.GetBytes(timeout));

			mRedisClient.SetExpiration(id, TimeSpan.FromMinutes(timeout));
		}		
		public override void EndRequest(HttpContext context)
		{
			
		}
		public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actions);
		}
		public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actions);
		}
		public override void InitializeRequest(HttpContext context)
		{
			
		}
		public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			
			hash.Delete("Locked");
			hash.Delete("LockDate");
			mRedisClient.SetExpiration(id, TimeSpan.FromMinutes(20));
		}
		public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			int dbLockId = BitConverter.ToInt32(hash.Get("LockId"), 0);
			if (dbLockId == (int)lockId)
			{
				mRedisClient.Delete(id);
			}			
		}
		public override void ResetItemTimeout(HttpContext context, string id)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			int timeout = BitConverter.ToInt32(hash.Get("Timeout"), 0);

			mRedisClient.SetExpiration(id, TimeSpan.FromMinutes(timeout));
		}
		public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			if (newItem)
			{
				mRedisClient.Delete(id);

				hash.Set("Flags", new byte[] { 0 });
				hash.Set("Timeout", BitConverter.GetBytes(item.Timeout));
			}

			Serialize(hash, (SessionStateItemCollection)item.Items);

			mRedisClient.SetExpiration(id, TimeSpan.FromMinutes(item.Timeout));
		}
		public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
		{
			return false;
		}

		private SessionStateStoreData GetSessionStoreItem(bool lockRecord, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			locked = false;
			lockAge = TimeSpan.Zero;
			lockId = null;
			RedisHash config = mRedisClient.GetHash(id);
			actions = SessionStateActions.None;

			if (!mRedisClient.KeyExists(id)) // First check to see if the session exists in the keyspace. If not, return instantly
				return null;

			if (lockRecord)
			{
				if (config.Exists("Locked"))
				{
					locked = true;
					DateTime lockdate = DateTime.FromBinary(BitConverter.ToInt64(config.Get("LockDate"), 0));
					lockAge = DateTime.UtcNow - lockdate;
					lockId = BitConverter.ToInt32(config.Get("LockId"), 0);
				} else {
					config.Set("Locked", new byte[] { 1 });
					config.Set("LockDate", BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
				}
			}
			
			SessionStateStoreData item;

			byte[] flagBytes = config.Get("Flags");
			if (flagBytes != null)
				actions = (SessionStateActions)flagBytes[0];
			else
				actions = SessionStateActions.None;
			int timeout = BitConverter.ToInt32(config.Get("Timeout"), 0);
			byte[] lockidb = config.Get("LockId");
			if (lockidb != null)
				lockId = BitConverter.ToInt32(lockidb, 0);

			if (actions == SessionStateActions.InitializeItem)
				item = CreateNewStoreData(context, timeout);
			else
				item = Deserialize(context, id, timeout);

			return item;
		}
		private SessionStateStoreData Deserialize(HttpContext context, string id, int timeout)
		{
			RedisHash hash = mRedisClient.GetHash(id);
			byte[] data = hash.Get("Items");

			if (data == null)
				return null;

			MemoryStream ms = new MemoryStream(data);
			SessionStateItemCollection collection = new SessionStateItemCollection();

			if (ms.Length > 0)
			{
				BinaryReader reader = new BinaryReader(ms);
				collection = SessionStateItemCollection.Deserialize(reader);
			}

			return new SessionStateStoreData(collection, SessionStateUtility.GetSessionStaticObjects(context), timeout);
		}
		private void Serialize(RedisHash target, SessionStateItemCollection items)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(ms);

			if (items != null)
				items.Serialize(writer);

			writer.Close();

			target.Set("Items", ms.ToArray());
		}

		private RedisClient mRedisClient;
		private const int mTimeout = 20;
	}
}