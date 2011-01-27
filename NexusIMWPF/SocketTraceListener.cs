using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;

namespace NexusIM
{
	class SocketTraceListener : TraceListener
	{
		public SocketTraceListener(string hostname, int port)
		{
			mSocket = new TcpClient();
			mSocket.Connect(hostname, port);

			mWriter = new StreamWriter(mSocket.GetStream());
		}
		public override void Write(string message)
		{
			try	{
				mWriter.Write(message);
				mWriter.Flush();
			} catch (SocketException) {
				Trace.Listeners.Remove(this);
			}
		}

		public override void WriteLine(string message)
		{
			try {
				mWriter.WriteLine(message);
				mWriter.Flush();
			} catch (SocketException){
				Trace.Listeners.Remove(this);
			}
		}

		private TcpClient mSocket;
		private StreamWriter mWriter;
	}
}
