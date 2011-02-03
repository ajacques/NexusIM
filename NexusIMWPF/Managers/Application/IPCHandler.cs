using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Text;

namespace NexusIM.Managers
{
	/// <summary>
	/// Handles connections between other instances of this application
	/// </summary>
	internal static class IPCHandler
	{
		/// <summary>
		/// Sets-up all variables used by the named pipe, and creates the named pipe
		/// </summary>
		public static void Setup()
		{
			pipeServer = new NamedPipeServerStream("nexusim", PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
			pipeServer.BeginWaitForConnection(new AsyncCallback(onNamedPipeRead), null);
		}
		/// <summary>
		/// Attempts to connect to a pre-existing instance
		/// </summary>
		public static void OpenConnection()
		{
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				ConnectPipe();
				string input = "";

				// Merge all command line arguments into one string
				input = Environment.GetCommandLineArgs().Skip(1).Aggregate((l, r) => l + " " + r);

				input = input.TrimEnd(new char[] { ' ' });
				StreamWriter writer = new StreamWriter(pipeClient);
				writer.WriteLine(input);
				writer.Flush();
				pipeClient.Flush();
				pipeClient.Close();
			}
		}
		public static void BringToFront()
		{
			try	{
				ConnectPipe();
			} catch (IOException) {
				return;
			}

			byte[] mSendBytes = Encoding.ASCII.GetBytes("GETFOCUS");
			
			pipeClient.Write(mSendBytes, 0, mSendBytes.Length);
			pipeClient.Close();
			pipeClient.Dispose();
		}

		private static void ConnectPipe()
		{
			pipeClient = new NamedPipeClientStream(".", "nexusim", PipeDirection.Out);
			pipeClient.Connect(1000);
		}

		private static void onNamedPipeRead(IAsyncResult e)
		{
			pipeServer.EndWaitForConnection(e);
			StreamReader reader = new StreamReader(pipeServer);

			do {
				string data = reader.ReadLine();
				CMDArgsHandler.HandleArg(data);
			} while (pipeServer.IsConnected);
			pipeServer.Disconnect();

			pipeServer.BeginWaitForConnection(new AsyncCallback(onNamedPipeRead), null);
		}

		private static NamedPipeServerStream pipeServer;
		private static NamedPipeClientStream pipeClient;
	}
}