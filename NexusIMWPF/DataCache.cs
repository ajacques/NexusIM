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

	partial class VideoMetadata
	{
		public long Likes
		{
			get;
			set;
		}
		public long Dislikes
		{
			get;
			set;
		}
		public long Views
		{
			get;
			set;
		}
	}
}