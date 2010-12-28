using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace NexusWeb
{
	public class NetworkTraceListener : TextWriterTraceListener
	{
		public NetworkTraceListener(string hostname, int port) : base(CreateStream(hostname, port))
		{
		}

		private static NetworkStream CreateStream(string hostname, int port)
		{
			Socket s = new Socket(AddressFamily.Unspecified, SocketType.Dgram, ProtocolType.IP);
			s.Connect(hostname, port);

			return new NetworkStream(s);
		}
	}
}