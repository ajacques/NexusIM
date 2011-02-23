using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Data.SqlServerCe;

namespace NexusIM
{
	/// <summary>
	/// Manages open connections on a per-thread basis
	/// </summary>
	class CEConnectionPool
	{
		private Dictionary<int, IDbConnection> threadConnectionMap;

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a connection.
		/// </summary>
		/// <returns>An open connection</returns>
		public IDbConnection Connection
		{
			get	{
				int threadId = Thread.CurrentThread.ManagedThreadId;

				lock (threadConnectionMap)
				{
					IDbConnection connection = null;
					if (!threadConnectionMap.TryGetValue(threadId, out connection))
					{
						connection = new SqlCeConnection(ConnectionString);
						connection.Open();
						threadConnectionMap.Add(threadId, connection);
					}

					return connection;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPool"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public CEConnectionPool(string connectionString)
		{
			threadConnectionMap = new Dictionary<int, IDbConnection>();
			ConnectionString = connectionString;
		}
	}
}
