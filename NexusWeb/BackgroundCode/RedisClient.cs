using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace NexusWeb.Infrastructure.Redis
{
	internal delegate EndPoint RedisServerSelector(string key);

	internal class RedisException : Exception
	{
		public RedisException(string message, EndPoint activeServer) : base("Redis operation failed due to an error code returned by the Redis server.\r\n" + message)
		{
			ErrorDetail = message;
			ActiveServer = activeServer;
		}

		public string ErrorDetail
		{
			get;
			private set;
		}
		public EndPoint ActiveServer
		{
			get;
			private set;
		}
	}

	internal enum RedisTypes
	{
		String,
		List,
		Set,
		ZSet,
		Hash
	}

	internal class RedisList : ICollection<byte[]>
	{
		internal RedisList(string listkey, RedisStream serverStream, object syncObject)
		{
			mListKey = RedisStream.Encoder.GetBytes(listkey);
			mDataStream = serverStream;
			mSyncObject = syncObject;
			mBatchSyncObject = new object();
		}
		
		public void BeginBatchOperation()
		{
			mBatchStream = new MemoryStream();
		}
		public void CommitBatchOperation()
		{
			byte[] responseBuffer = new byte[1024];
			lock (mBatchSyncObject)
			{
				lock (mSyncObject)
				{
					mBatchStream.WriteTo(mDataStream);

					mDataStream.Read(responseBuffer, 0, responseBuffer.Length);
				}
				mBatchStream = null;
			}
		}

		public void Add(byte[] item)
		{
			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.RPush, mListKey, item))
			{
				if (mBatchStream != null)
				{
					lock (mBatchSyncObject)
						stream.WriteTo(mBatchStream);
				} else {
					lock (mSyncObject)
					{
						stream.WriteTo(mDataStream);

						mDataStream.ExpectInt();
					}
				}				
			}
		}
		public void Clear()
		{
			throw new NotImplementedException();
		}
		public bool Contains(byte[] item)
		{
			throw new NotImplementedException();
		}
		public void CopyTo(byte[][] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		public bool IsReadOnly
		{
			get {
				return false;
			}
		}
		public int Count
		{
			get	{
				using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.LLen, mListKey))
				{
					lock (mSyncObject)
					{
						stream.WriteTo(mDataStream);

						return mDataStream.ExpectInt();
					}
				}
			}
		}
		public byte[] this[int id]
		{
			get	{
				byte[] keyBytes = RedisStream.Encoder.GetBytes(id.ToString());

				using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.LIndex, mListKey, keyBytes))
				{
					lock (mSyncObject)
					{
						stream.WriteTo(mDataStream);

						return mDataStream.ExpectBulkReply();
					}
				}
			}
		}
		public bool Remove(byte[] item)
		{
			throw new NotImplementedException();
		}

		private class BulkReplyEnumerator : IEnumerator<byte[]>
		{
			public BulkReplyEnumerator(byte[][] source)
			{
				mSource = source;
			}

			public byte[] Current
			{
				get {
					return mSource[mCursor];
				}
			}
			public void Dispose() {}
			object IEnumerator.Current
			{
				get {
					return Current;
				}
			}
			public bool MoveNext()
			{
				mCursor++;

				return mCursor < mSource.Length;
			}
			public void Reset() {}
			private byte[][] mSource;
			private uint mCursor;
		}

		public IEnumerator<byte[]> GetEnumerator(int start, int end)
		{
			byte[] startBytes = RedisStream.Encoder.GetBytes(start.ToString());
			byte[] endBytes = RedisStream.Encoder.GetBytes(end.ToString());

			byte[][] blocks;

			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.LRange, mListKey, startBytes, endBytes))
			{
				lock (mSyncObject)
				{
					stream.WriteTo(mDataStream);

					blocks = mDataStream.ExpectMultiBulkReply();
				}
			}

			return blocks != null ? new BulkReplyEnumerator(blocks) : null;
		}
		public IEnumerator<byte[]> GetEnumerator()
		{
			return GetEnumerator(0, -1);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static class RedisListCommands
		{
			public static readonly byte[] Delete = new byte[] { 0x44, 0x45, 0x4C };
			public static readonly byte[] LLen = new byte[] { 0x4C, 0x4C, 0x45, 0x4E };
			public static readonly byte[] RPush = new byte[] { 0x52, 0x50, 0x55, 0x53, 0x48 };
			public static readonly byte[] LIndex = new byte[] { 76, 73, 78, 68, 69, 88 };
			public static readonly byte[] LRange = new byte[] { 0x4C, 0x52, 0x41, 0x4E, 0x47, 0x45 };
		}

		// Variables
		private RedisStream mDataStream;
		private object mSyncObject;
		private byte[] mListKey;
		private object mBatchSyncObject;
		private MemoryStream mBatchStream;
	}

	internal class RedisSet : ICollection<byte[]>
	{
		internal RedisSet(string listkey, RedisStream serverStream, object syncObject)
		{
			mListKey = RedisStream.Encoder.GetBytes(listkey);
			mDataStream = serverStream;
			mSyncObject = syncObject;
		}
		
		public void Add(byte[] item)
		{
			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.SAdd, mListKey, item))
			{
				lock (mSyncObject)
				{
					stream.WriteTo(mDataStream);

					mDataStream.ExpectInt();
				}			
			}
		}
		public void Clear()
		{
			throw new NotImplementedException();
		}
		public bool Contains(byte[] item)
		{
			throw new NotImplementedException();
		}
		public void CopyTo(byte[][] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		public bool IsReadOnly
		{
			get {
				return false;
			}
		}
		public int Count
		{
			get	{
				using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.SCard, mListKey))
				{
					lock (mSyncObject)
					{
						stream.WriteTo(mDataStream);

						return mDataStream.ExpectInt();
					}
				}
			}
		}
		
		public bool Remove(byte[] item)
		{
			throw new NotImplementedException();
		}

		private class BulkReplyEnumerator : IEnumerator<byte[]>
		{
			public BulkReplyEnumerator(byte[][] source)
			{
				mSource = source;
			}

			public byte[] Current
			{
				get {
					return mSource[mCursor];
				}
			}
			public void Dispose() {}
			object IEnumerator.Current
			{
				get {
					return Current;
				}
			}
			public bool MoveNext()
			{
				mCursor++;

				return mCursor < mSource.Length;
			}
			public void Reset() {}
			private byte[][] mSource;
			private uint mCursor;
		}

		public IEnumerator<byte[]> GetEnumerator()
		{
			byte[][] blocks;

			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisListCommands.SMembers, mListKey))
			{
				lock (mSyncObject)
				{
					stream.WriteTo(mDataStream);

					blocks = mDataStream.ExpectMultiBulkReply();
				}
			}

			return blocks != null ? new BulkReplyEnumerator(blocks) : null;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static class RedisListCommands
		{
			public static readonly byte[] SAdd = new byte[] { 0x53, 0x41, 0x44, 0x44 };
			public static readonly byte[] SMembers = new byte[] { 0x53, 0x4d, 0x45, 0x4d, 0x42, 0x45, 0x52, 0x53 };
			public static readonly byte[] SCard = new byte[] { 0x53, 0x43, 0x41, 0x52, 0x44 };
		}

		// Variables
		private RedisStream mDataStream;
		private object mSyncObject;
		private byte[] mListKey;
	}

	internal class RedisStream : Stream
	{
		static RedisStream()
		{
			mEncoder = Encoding.UTF8;
		}
		public RedisStream(Socket socket)
		{
			mDataStream = new NetworkStream(socket);
			mSocket = socket;
		}

		private void ThrowRedisError(string prefix = "")
		{
			byte[] errormsg = new byte[100];
			int bytes = mDataStream.Read(errormsg, 0, errormsg.Length);

			throw new RedisException(prefix + mEncoder.GetString(errormsg, 0, bytes), mSocket.RemoteEndPoint);
		}

		/// <summary>
		/// Reads the response from the Redis server and throws an exception if the server does not return a +OK
		/// </summary>
		/// <exception cref="NexusWeb.Infrastructure.Redis.RedisException">Thrown when the server returns an error.</exception>
		public void ExpectOK()
		{
			byte[] result = new byte[5];
			mDataStream.Read(result, 0, result.Length);

			if (result[0] != 0x2b) // +
				ThrowRedisError();
		}
		public int ExpectInt()
		{
			byte[] result = new byte[8];
			int bytesRead = mDataStream.Read(result, 0, result.Length);

			if (result[0] == 45)
				ThrowRedisError(mEncoder.GetString(result, 5, result.Length - 5));

			string number = mEncoder.GetString(result, 1, bytesRead - 3);

			return Int32.Parse(number);
		}
		public byte[] ExpectBulkReply()
		{
			mDataStream.ReadByte(); // Discard the $
			int pktlen = 0;
			while (true)
			{
				int curbyte = mDataStream.ReadByte();
				if (curbyte == 45)
				{
					for (int i = 0; i < 3; i++)
						mDataStream.ReadByte();
					return null;
				}
				if (curbyte == 0x0d)
					break;

				pktlen = (pktlen * 10) + (curbyte - 48);
			}
			mDataStream.ReadByte(); // Discard the \n

			byte[] returndata = new byte[pktlen];

			mDataStream.Read(returndata, 0, pktlen);
			mDataStream.ReadByte();
			mDataStream.ReadByte();

			return returndata;
		}
		public byte[][] ExpectMultiBulkReply()
		{
			int code = mDataStream.ReadByte();
			if (code == 24)
				return null;
			else if (code == 45)
				ThrowRedisError();

			byte[] lengthbuffer = new byte[8];

			int i = 0;
			while ((lengthbuffer[i] = (byte)mDataStream.ReadByte()) != 10)
				i++;

			int bulkpoints = Int32.Parse(mEncoder.GetString(lengthbuffer, 0, i - 1));

			byte[][] bulks = new byte[bulkpoints][];

			for (int ct = 0; ct < bulkpoints; ct++)
				bulks[ct] = ExpectBulkReply();

			return bulks;
		}
		/// <summary>
		/// Builds a packet following the new Redis Unified Protocol
		/// </summary>
		/// <param name="commands">A collection of byte arrays for each parameter</param>
		/// <returns>A memory stream representing the constructed message</returns>
		public static MemoryStream BuildUnifiedCommand(params byte[][] commands)
		{
			MemoryStream ms = new MemoryStream(100);
			ms.WriteByte(0x2a);
			WriteStringNumber(commands.Length, ms);

			WriteNewlines(ms);

			for (int i = 0; i < commands.Length; i++)
			{
				ms.WriteByte(0x24); // $
				WriteStringNumber(commands[i].Length, ms);
				WriteNewlines(ms);

				ms.Write(commands[i], 0, commands[i].Length);
				WriteNewlines(ms);
			}
			WriteNewlines(ms);

			return ms;
		}
		private static void WriteStringNumber(int number, Stream stream)
		{
			byte[] lenbytes = mEncoder.GetBytes(number.ToString());
			stream.Write(lenbytes, 0, lenbytes.Length);
		}
		private static void WriteNewlines(Stream stream)
		{
			stream.WriteByte(0x0d);
			stream.WriteByte(0x0a);
		}

		public override bool CanRead
		{
			get {
				return mDataStream.CanRead;
			}
		}
		public override bool CanSeek
		{
			get {
				return mDataStream.CanSeek;
			}
		}
		public override bool CanWrite
		{
			get {
				return mDataStream.CanWrite;
			}
		}
		public override void Flush()
		{
			mDataStream.Flush();
		}
		public override long Length
		{
			get {
				return mDataStream.Length;
			}
		}
		public override long Position
		{
			get	{
				return mDataStream.Position;
			}
			set	{
				mDataStream.Position = value;
			}
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			return mDataStream.Read(buffer, offset, count);
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			return mDataStream.Seek(offset, origin);
		}
		public override void SetLength(long value)
		{
			mDataStream.SetLength(value);
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			mDataStream.Write(buffer, offset, count);
		}

		public static Encoding Encoder
		{
			get	{
				return mEncoder;
			}
		}

		// Variables
		private Socket mSocket;
		private NetworkStream mDataStream;
		private static readonly Encoding mEncoder;
	}

	/// <summary>
	/// Implements the Redis client protocol to connect and interface with Redis servers
	/// </summary>
	/// <remarks>
	/// All methods are thread-safe.
	/// </remarks>
	internal class RedisClient : IDisposable
	{
		private RedisClient()
		{
			mSocketOperationLock = new object(); // Empty object that will serve as a synchronization object
			mEncoder = Encoding.UTF8;
		}
		public RedisClient(string hostname) : this(hostname, 6379) {}
		/// <exception cref="System.ArgumentNullException">Thrown when the hostname is null or is empty</exception>
		/// <param name="hostname">Default Redis server hostname in dns format or IP format</param>
		public RedisClient(string hostname, int port) : this()
		{
			if (String.IsNullOrEmpty(hostname))
				throw new ArgumentNullException("hostname");

			if (port < 0 || port > ushort.MaxValue)
				throw new ArgumentOutOfRangeException("port");

			Host = hostname;
			Port = port;
		}

		public void Connect()
		{
			mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mSocket.NoDelay = true;
			mSocket.Connect(Host, Port);
			mRedisStream = new RedisStream(mSocket);
		}
		public void Dispose()
		{
			if (!mDisposed)
				this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (mRedisStream != null)
				{
					mRedisStream.Close();
					mRedisStream.Dispose();
				}
				if (mSocket != null)
					mSocket.Dispose();

				mRedisStream = null;
				mSocket = null;
				mDisposed = true;
				mSocketOperationLock = null;
			}
		}

		public void FlushDatabase()
		{
			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.FlushDb))
			{
				lock (mSocketOperationLock)
				{
					stream.WriteTo(mRedisStream);

					mRedisStream.ExpectOK();
				}
			}
		}
		public byte[] Get(string key)
		{
			if (mDisposed)
				throw new ObjectDisposedException("this");

			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);

			MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.Get, keyBytes);

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mRedisStream);

				return mRedisStream.ExpectBulkReply();
			}
		}
		public void Set(string key, byte[] data)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (data == null)
				throw new ArgumentNullException("data");

			byte[] keyBytes = mEncoder.GetBytes(key);
			
			MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.Set, keyBytes, data);
			
			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mRedisStream);

				mRedisStream.ExpectOK();
			}
			stream.Dispose();
		}
		public void Set(string key, byte[] data, TimeSpan expireIn)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (data == null)
				throw new ArgumentNullException("data");			
			
			byte[] keyBytes = mEncoder.GetBytes(key);
			byte[] expireBytes = mEncoder.GetBytes(expireIn.TotalSeconds.ToString("F0"));
			
			MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.SetEx, keyBytes, expireBytes, data);

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mRedisStream);

				mRedisStream.ExpectOK();
			}

			stream.Dispose();
		}
		public RedisList GetList(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			EnsureSocketConnected();

			return new RedisList(key, mRedisStream, mSocketOperationLock);
		}
		public RedisSet GetSet(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			EnsureSocketConnected();

			return new RedisSet(key, mRedisStream, mSocketOperationLock);
		}
		/// <summary>
		/// Removes a single key from the database. No action if the key does not exist
		/// </summary>
		/// <remarks>
		/// Time complexity: O(m)
		/// m : Number of values (1 for a single string value)
		/// </remarks>
		/// <param name="key">The key to delete</param>
		public void Delete(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);

			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.Delete, keyBytes))
			{
				lock (mSocketOperationLock)
				{
					EnsureSocketConnected();

					stream.WriteTo(mRedisStream);

					mRedisStream.ExpectOK();
				}
			}
		}
		public bool KeyExists(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);

			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.Exists, keyBytes))
			{
				lock (mSocketOperationLock)
				{
					EnsureSocketConnected();

					stream.WriteTo(mRedisStream);

					return mRedisStream.ExpectInt() == 1;
				}
			}
		}
		/// <summary>
		/// Removes the specified keys. A key is ignored if it does not exist.
		/// </summary>
		/// <remarks>
		/// Time complexity: O(n * m)
		/// n : Number of keys to delete
		/// m : Number of values (1 for a single string value)
		/// </remarks>
		public int Delete(params string[] keys)
		{
			return Delete((IEnumerable<string>)keys);
		}
		public IDictionary<string, byte[]> Get(params string[] keys)
		{
			return Get(keys);
		}
		public IDictionary<string, byte[]> Get(IEnumerable<string> keys)
		{
			if (keys == null)
				throw new ArgumentNullException("keys");

			List<byte[]> keysBytes = new List<byte[]>(2);
			keysBytes.Add(RedisCommands.MGet);

			foreach (string key in keys)
				keysBytes.Add(mEncoder.GetBytes(key));

			byte[][] result = null;
			MemoryStream stream = null;
			try {
				stream = RedisStream.BuildUnifiedCommand(keysBytes.ToArray());
				Monitor.Enter(mSocketOperationLock);

				EnsureSocketConnected();
				stream.WriteTo(mRedisStream);
				result = mRedisStream.ExpectMultiBulkReply();
			} finally {
				Monitor.Exit(mSocketOperationLock);
				if (stream != null)
					stream.Dispose();
			}

			Dictionary<string, byte[]> final = new Dictionary<string, byte[]>(result.Length);
			IEnumerator<string> keyEnumr = keys.GetEnumerator();

			for (int i = 0; i < result.Length; i++)
			{
				keyEnumr.MoveNext();
				final.Add(keyEnumr.Current, result[i]);
			}

			return final;
		}
		/// <summary>
		/// Removes the specified keys. A key is ignored if it does not exist.
		/// </summary>
		/// <remarks>
		/// Time complexity: O(n * m)
		/// n : Number of keys to delete
		/// m : Number of values (1 for a single string value)
		/// </remarks>
		public int Delete(IEnumerable<string> keys)
		{
			if (keys == null)
				throw new ArgumentNullException("keys");

			List<byte[]> keysBytes = new List<byte[]>(2);
			keysBytes.Add(RedisCommands.Delete);

			foreach (string key in keys)
				keysBytes.Add(mEncoder.GetBytes(key));

			MemoryStream stream = null;
			try {
				stream = RedisStream.BuildUnifiedCommand(keysBytes.ToArray());
				Monitor.Enter(mSocketOperationLock);

				EnsureSocketConnected();
				stream.WriteTo(mRedisStream);
				return mRedisStream.ExpectInt();
			} finally {
				Monitor.Exit(mSocketOperationLock);
				if (stream != null)
					stream.Dispose();
			}			
		}
		public int Increment(string key, int step = 1)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);
			MemoryStream stream;

			if (step == 1)
				stream = RedisStream.BuildUnifiedCommand(RedisCommands.Increment, keyBytes);
			else {
				byte[] stepBytes = mEncoder.GetBytes(step.ToString());
				stream = RedisStream.BuildUnifiedCommand(RedisCommands.IncrementBy, keyBytes, stepBytes);
			}

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mRedisStream);

				return mRedisStream.ExpectInt();
			}
		}
		public void ChangeDatabase(int dbid)
		{
			if (mDatabaseId != dbid)
			{
				mDatabaseId = dbid;

				if (mSocket != null && mSocket.Connected)
				{
					byte[] dbbytes = mEncoder.GetBytes(dbid.ToString());
					MemoryStream sourceStream = RedisStream.BuildUnifiedCommand(RedisCommands.Select, dbbytes);
					lock (mSocketOperationLock)
					{
						sourceStream.WriteTo(mRedisStream);

						mRedisStream.ExpectOK();
					}
				}
			}
		}
		public TimeSpan GetTimeToLive(string key)
		{
			if (key == null)
				throw new ArgumentNullException(key);
			
			byte[] keyBytes = mEncoder.GetBytes(key);

			int expDate;

			using (MemoryStream stream = RedisStream.BuildUnifiedCommand(RedisCommands.TTL, keyBytes))
			{
				lock (mSocketOperationLock)
				{
					EnsureSocketConnected();

					stream.WriteTo(mRedisStream);

					expDate = mRedisStream.ExpectInt();
				}
			}

			return TimeSpan.FromSeconds(expDate);
		}

		private void EnsureSocketConnected()
		{
			if (mSocket == null)
				Connect();

			if (!mSocket.Connected)
				Connect();
		}

		private static class RedisCommands
		{
			/// <summary>
			/// Retrieve the value of a pair stored on the server
			/// Format: GET {key}
			/// </summary>
			public static readonly byte[] Get = new byte[] { 0x47, 0x45, 0x54 };
			/// <summary>
			/// Retrieve the value of several pairs stored on the server
			/// Format: GET {key1} {key2} .. {keyN}
			/// </summary>
			public static readonly byte[] MGet = new byte[] { 0x4d, 0x47, 0x45, 0x54 };
			public static readonly byte[] Set = new byte[] { 0x53, 0x45, 0x54 };
			/// <summary>
			/// Delete one or more keys
			/// Format: GET {key1} {key2} ... {keyN}
			/// </summary>
			public static readonly byte[] Delete = new byte[] { 0x44, 0x45, 0x4C };
			/// <summary>
			/// Set the value of an object that expires in the specified number of seconds
			/// Format: SETEX {key} {expiration in seconds} {data}
			/// </summary>
			public static readonly byte[] SetEx = new byte[] { 0x53, 0x45, 0x54, 0x45, 0x58 };
			public static readonly byte[] Increment = new byte[] { 0x49, 0x4e, 0x43, 0x52 };
			public static readonly byte[] IncrementBy = new byte[] { 0x49, 0x4e, 0x43, 0x52, 0x42, 0x59 };
			public static readonly byte[] Select = new byte[] { 0x53, 0x45, 0x4c, 0x45, 0x43, 0x54 };
			/// <summary>
			/// Returns the time in seconds until expiration for the specified keys
			/// Format: TTL {key}
			/// </summary>
			public static readonly byte[] TTL = new byte[] { 0x54, 0x54, 0x4C };
			/// <summary>
			/// Determines if a key exists
			/// Format: Exists {key}
			/// </summary>
			public static readonly byte[] Exists = new byte[] { 0x45, 0x58, 0x49, 0x53, 0x54, 0x53 };
			/// <summary>
			/// Delete all keys in the currently selected database
			/// Format: FLUSHDB
			/// </summary>
			public static readonly byte[] FlushDb = new byte[] { 0x46, 0x4C, 0x55, 0x53, 0x48, 0x44, 0x42 };
		}
		private static class RedisResponses
		{
			public static readonly byte[] Success = new byte[] { 0x2b, 0x4f, 0x4b, 0x0d, 0x0a };
		}

		// Properties
		public string Host
		{
			get;
			private set;
		}
		public int Port
		{
			get;
			private set;
		}

		// Variables
		private object mSocketOperationLock;
		private Socket mSocket;
		private RedisStream mRedisStream;
		private readonly Encoding mEncoder;
		private bool mDisposed;
		private int mDatabaseId;
	}
}