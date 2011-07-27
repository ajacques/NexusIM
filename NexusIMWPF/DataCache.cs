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
		/// <summary>
		/// Gets or sets the number of likes the video has.
		/// Used by YouTube
		/// </summary>
		public long Likes
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the number of dislikes the video has.
		/// Used by Youtube
		/// </summary>
		public long Dislikes
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the number of times the video has been viewed.
		/// </summary>
		public long Views
		{
			get;
			set;
		}
	}
}