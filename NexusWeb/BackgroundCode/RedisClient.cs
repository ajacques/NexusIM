using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NexusWeb.Infrastructure.Redis
{
	internal delegate EndPoint RedisServerSelector(string key);

	internal class RedisException : Exception
	{
		public RedisException(string message, EndPoint activeServer) : base("Redis operation failed due to an error code returned by the Redis server.\r\nSee the ErrorDetail property for more information.")
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
			mServerList = new List<RedisServerConnection>();
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
			mServerList.Add(new RedisServerConnection(new DnsEndPoint(hostname, port)));
		}

		public void Connect()
		{
			mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mSocket.NoDelay = true;
			mSocket.Connect(Host, Port);
			mDataStream = new NetworkStream(mSocket);
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
				if (mDataStream != null)
				{
					mDataStream.Close();
					mDataStream.Dispose();
				}
				if (mSocket != null)
					mSocket.Dispose();

				mDataStream = null;
				mSocket = null;
				mDisposed = true;
				mSocketOperationLock = null;
				mShardSelector = null;
			}
		}

		public byte[] Get(string key)
		{
			if (mDisposed)
				throw new ObjectDisposedException("this");

			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);
			
			MemoryStream stream = BuildUnifiedCommand(RedisCommands.Get, keyBytes);

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mDataStream);

				return ReadData();
			}
		}
		public void Set(string key, byte[] data)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (data == null)
				throw new ArgumentNullException("data");

			byte[] keyBytes = mEncoder.GetBytes(key);
			
			MemoryStream stream = BuildUnifiedCommand(RedisCommands.Set, keyBytes, data);
			
			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mDataStream);
				
				ExpectOK();
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
			
			MemoryStream stream = BuildUnifiedCommand(RedisCommands.SetEx, keyBytes, expireBytes, data);

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mDataStream);

				ExpectOK();
			}

			stream.Dispose();
		}
		public int Increment(string key, int step = 1)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			byte[] keyBytes = mEncoder.GetBytes(key);
			MemoryStream stream;

			if (step == 1)
				stream = BuildUnifiedCommand(RedisCommands.Increment, keyBytes);
			else {
				byte[] stepBytes = mEncoder.GetBytes(step.ToString());
				stream = BuildUnifiedCommand(RedisCommands.IncrementBy, keyBytes, stepBytes);
			}

			lock (mSocketOperationLock)
			{
				EnsureSocketConnected();

				stream.WriteTo(mDataStream);

				return ExpectInt();
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
					MemoryStream sourceStream = BuildUnifiedCommand(RedisCommands.Select, dbbytes);
					lock (mSocketOperationLock)
					{
						sourceStream.WriteTo(mDataStream);

						ExpectOK();
					}
				}
			}
		}

		/// <summary>
		/// Reads the response from the Redis server and throws an exception if the server does not return a +OK
		/// </summary>
		/// <exception cref="NexusWeb.Infrastructure.Redis.RedisException">Thrown when the server returns an error.</exception>
		private void ExpectOK()
		{
			byte[] result = new byte[5];
			mDataStream.Read(result, 0, result.Length);

			if (result[0] != 0x2b) // +
			{
				byte[] errormsg = new byte[100];
				int bytes = mDataStream.Read(errormsg, 0, errormsg.Length);

				throw new RedisException(mEncoder.GetString(errormsg, 0, bytes), mSocket.RemoteEndPoint);
			}
		}
		private int ExpectInt()
		{
			byte[] result = new byte[16];
			int bytesRead = mDataStream.Read(result, 0, result.Length);

			string number = mEncoder.GetString(result, 1, bytesRead - 2);

			return Int32.Parse(number);
		}
		private void EnsureSocketConnected()
		{
			if (mSocket == null)
				Connect();

			if (!mSocket.Connected)
				Connect();
		}
		private byte[] ReadData()
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
		private MemoryStream BuildUnifiedCommand(params byte[][] commands)
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
		private void WriteStringNumber(int number, Stream stream)
		{
			byte[] lenbytes = mEncoder.GetBytes(number.ToString());
			stream.Write(lenbytes, 0, lenbytes.Length);
		}
		private void WriteNewlines(Stream stream)
		{
			stream.WriteByte(0x0d);
			stream.WriteByte(0x0a);
		}
		private RedisServerConnection SelectServer(string key)
		{
			EndPoint point = mShardSelector(key);

			foreach (RedisServerConnection conn in mServerList)
			{
				if (conn.EndPoint == point)
				{
					conn.EnsureConnected();
					return conn;
				}
			}

			throw new NotImplementedException();
		}

		// Nested Classes
		private class RedisServerConnection : IDisposable
		{
			public RedisServerConnection(EndPoint epoint)
			{
				EndPoint = epoint;
				Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				Socket.NoDelay = true;
				ThreadSyncObject = new object();
			}

			public void Dispose()
			{
				EndPoint = null;
				if (Stream != null)
				{
					Stream.Close();
					Stream.Dispose();
				}
				if (Socket != null)
					Socket.Dispose();

				Socket = null;
				Stream = null;
				GC.SuppressFinalize(this);
			}
			public void EnsureConnected()
			{
				if (!Socket.Connected)
				{
					Socket.Connect(EndPoint);
					Stream = new NetworkStream(Socket);
				}
			}

			public EndPoint EndPoint
			{
				get;
				private set;
			}
			public Socket Socket
			{
				get;
				private set;
			}
			public NetworkStream Stream
			{
				get;
				private set;
			}
			public object ThreadSyncObject
			{
				get;
				private set;
			}
		}
		private static class RedisCommands
		{
			public static readonly byte[] Get = new byte[] { 0x47, 0x45, 0x54 };
			public static readonly byte[] Set = new byte[] { 0x53, 0x45, 0x54 };
			/// <summary>
			/// Set the value of an object that expires in the specified number of seconds
			/// Format: SETEX {key} {expiration in seconds} {data}
			/// </summary>
			public static readonly byte[] SetEx = new byte[] { 0x53, 0x45, 0x54, 0x45, 0x58 };
			public static readonly byte[] Increment = new byte[] { 0x49, 0x4e, 0x43, 0x52 };
			public static readonly byte[] IncrementBy = new byte[] { 0x49, 0x4e, 0x43, 0x52, 0x42, 0x59 };
			public static readonly byte[] Select = new byte[] { 0x53, 0x45, 0x4c, 0x45, 0x43, 0x54, 0x20 };
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
		private NetworkStream mDataStream;
		private readonly Encoding mEncoder;
		private bool mDisposed;
		private int mDatabaseId;
		private RedisServerSelector mShardSelector;
		private List<RedisServerConnection> mServerList;
	}
}