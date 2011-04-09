using System;
namespace NexusIM
{
	partial class UserProfile
	{
		public static UserProfile Create(string connectionString)
		{
			if (mPool == null)
				mPool = new CEConnectionPool(connectionString);

			return new UserProfile(mPool.Connection) { Log = Console.Out };
		}

		private static CEConnectionPool mPool;
	}
}