namespace NexusIM
{
	partial class DataCache
	{
		public static DataCache Create(string connectionString)
		{
			if (mPool == null)
				mPool = new CEConnectionPool(connectionString);

			return new DataCache(mPool.Connection);
		}

		private static CEConnectionPool mPool;
	}
}