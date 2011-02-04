namespace NexusIM
{
	partial class ChatHistory
	{
		public static ChatHistory Create(string connectionString)
		{
			if (mPool == null)
				mPool = new CEConnectionPool(connectionString);

			return new ChatHistory(mPool.Connection);
		}

		private static CEConnectionPool mPool;
	}
}
