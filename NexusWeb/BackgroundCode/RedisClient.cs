using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NexusWeb.Infrastructure.Redis
{
	internal class RedisClient : IDisposable
	{
		private RedisClient()
		{
			mSocketOperationLock = new object();
		}
		public RedisClient(string hostname) : this(hostname, 6379) {}
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
			}
		}

		public byte[] Get(string key)
		{
			VerifySocketState();

			byte[] keyBytes = mEncoder.GetBytes(key);
			byte[] buffer = new byte[6 + keyBytes.Length];
			
			buffer[0] = 0x47; // G
			buffer[1] = 0x45; // E
			buffer[2] = 0x54; // T
			buffer[3] = 0x20; // (space)
			buffer[buffer.Length - 2] = 0x0d; // \r
			buffer[buffer.Length - 1] = 0x0a; // \n

			Buffer.BlockCopy(keyBytes, 0, buffer, 4, keyBytes.Length);
			
			lock(mSocketOperationLock)
			{
				mDataStream.Write(buffer, 0, buffer.Length);

				return ReadData();
			}
		}

		public void Set(string key, byte[] data)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (data == null)
				throw new ArgumentNullException("data");

			VerifySocketState();

			byte[] keyBytes = mEncoder.GetBytes(key);
			MemoryStream stream = BuildUnifiedCommand(new byte[] { 0x53, 0x45, 0x54 }, keyBytes, data);

			lock (mSocketOperationLock)
			{
				stream.WriteTo(mDataStream);
				
				byte[] result = new byte[5];
				mDataStream.Read(result, 0, 5);
			}
			stream.Dispose();
		}

		private void VerifySocketState()
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
		private Stream mDataStream;
		private static readonly Encoding mEncoder = Encoding.UTF8;
		private bool mDisposed;
	}
}