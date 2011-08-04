using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Threading;

namespace NexusIM
{
	/// <summary>
	/// Manages open connections on a per-thread basis
	/// </summary>
	class CEConnectionPool : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPool"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public CEConnectionPool(string connectionString)
		{
			threadConnectionMap = new SortedDictionary<int, IDbConnection>();
			ConnectionString = connectionString;
			mapWriteLock = new ReaderWriterLockSlim();
		}

		public void Dispose()
		{
			if (disposed)
				return;

			disposed = true;
			if (threadConnectionMap != null)
			{
				foreach (var pair in threadConnectionMap)
					pair.Value.Dispose();
			}
			threadConnectionMap = null;

			if (mapWriteLock != null)
				mapWriteLock.Dispose();
			mapWriteLock = null;
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
				if (disposed)
					throw new ObjectDisposedException("this");

				int threadId = Thread.CurrentThread.ManagedThreadId;
				IDbConnection connection = null;
				bool foundValue;

				// Check to see if this thread already has a connection
				mapWriteLock.EnterReadLock();
				try {
					foundValue = threadConnectionMap.TryGetValue(threadId, out connection);
				} finally {
					mapWriteLock.ExitReadLock();
				}

				if (!foundValue)
				{
					// Open a new connection for the thread
					connection = new SqlCeConnection(ConnectionString);
					connection.Open();
					Trace.WriteLine(String.Format("CEConnectionPool: Opening new {0} connection (Total: {1})", connection.Database.Substring(connection.Database.LastIndexOf('\\') + 1), ++mConnections));

					mapWriteLock.EnterWriteLock();
					try {
						threadConnectionMap.Add(threadId, connection);
					} finally {
						mapWriteLock.ExitWriteLock();
					}
				}

				return connection;
			}
		}

		// Variables
		private int mConnections = 0;
		private SortedDictionary<int, IDbConnection> threadConnectionMap;
		private ReaderWriterLockSlim mapWriteLock;
		private bool disposed;
	}
}
