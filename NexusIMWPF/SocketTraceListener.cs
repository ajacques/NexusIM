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
			mSocket.BeginConnect(hostname, port, new AsyncCallback(OnConnect), null);

			mWriter = new StringWriter();
		}

		private void OnConnect(IAsyncResult e)
		{
			try	{
				mSocket.EndConnect(e);
			} catch (SocketException) {
				Trace.Listeners.Remove(this);
				return;
			}

			StreamWriter newWriter = new StreamWriter(mSocket.GetStream());
			newWriter.Write(mWriter.ToString());
			mWriter.Dispose();
			mWriter = newWriter;
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
		private TextWriter mWriter;
	}
}
