using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;

namespace NexusIM
{
	class SocketTraceListener : TraceListener, IDisposable
	{
		public SocketTraceListener(string hostname, int port)
		{
			mSocket = new TcpClient();
			mSocket.BeginConnect(hostname, port, new AsyncCallback(OnConnect), null);

			mWriter = new StringWriter();
		}

		public void Dispose()
		{
			Trace.Listeners.Remove(this);
			if (mWriter != null)
				mWriter.Close();
			else if (mSocket != null)
				mSocket.Close();

			mSocket = null;
			mWriter = null;
		}

		private void OnConnect(IAsyncResult e)
		{
			try	{
				mSocket.EndConnect(e);
			} catch (SocketException) {
				Dispose();
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
				Dispose();
				return;
			}
		}

		public override void WriteLine(string message)
		{
			try	{
				mWriter.WriteLine(message);
				mWriter.Flush();
			} catch (SocketException) {
				Dispose();
				return;
			} catch (IOException) {
				Dispose();
				return;
			}
		}

		private TcpClient mSocket;
		private TextWriter mWriter;
	}
}
