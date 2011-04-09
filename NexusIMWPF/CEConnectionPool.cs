using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Data.SqlServerCe;
using System.Diagnostics;

namespace NexusIM
{
	/// <summary>
	/// Manages open connections on a per-thread basis
	/// </summary>
	class CEConnectionPool : IDisposable
	{
		private Dictionary<int, IDbConnection> threadConnectionMap;

		public void Dispose()
		{
			if (threadConnectionMap != null)
			{
				foreach (var pair in threadConnectionMap)
					pair.Value.Dispose();
			}
		}

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
						Trace.WriteLine(string.Format("CEConnectionPool: Opening new {0} connection (Total: {1})", connection.Database.Substring(connection.Database.LastIndexOf('\\') + 1), ++mConnections));
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

		private int mConnections = 0;
	}
}
