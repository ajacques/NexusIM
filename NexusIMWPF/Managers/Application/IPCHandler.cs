using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace NexusIM.Managers
{
	/// <summary>
	/// Handles connections between other instances of this application
	/// </summary>
	public static class IPCHandler
	{
		/// <summary>
		/// Sets-up all variables used by the named pipe, and creates the named pipe
		/// </summary>
		public static void Setup()
		{
			// TODO: Can this be done without starting a thread just to start another thread?
			Thread t = new Thread(new ThreadStart(initWaitThread));
			t.Name = "Named Pipe Protocol Thread";
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}
		/// <summary>
		/// Attempts to connect to a pre-existing instance
		/// </summary>
		public static void OpenConnection()
		{
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				pipeClient = new NamedPipeClientStream(".", "nexusim", PipeDirection.Out);
				try {
					pipeClient.Connect(1000);
				} catch (IOException) {
					return;
				}
				if (pipeClient.IsConnected)
				{
					string input = "";

					// Merge all command line arguments into one string
					Environment.GetCommandLineArgs().ToList().GetRange(1, Environment.GetCommandLineArgs().Length - 1).ForEach(delegate(string s)
					{
						input += s + " ";
					});

					input = input.TrimEnd(new char[] { ' ' });
					StreamWriter writer = new StreamWriter(pipeClient);
					writer.WriteLine(input);
					writer.Flush();
					pipeClient.Flush();
					pipeClient.Close();
				}
			}
		}
		private static void initWaitThread()
		{
			pipeServer = new NamedPipeServerStream("nexusim", PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
			pipeServer.BeginWaitForConnection(new AsyncCallback(onNamedPipeRead), null);
		}
		private static void onNamedPipeRead(IAsyncResult e)
		{
			pipeServer.EndWaitForConnection(e);
			StreamReader reader = new StreamReader(pipeServer);

			do {
				string data = "";
				data = reader.ReadToEnd();
				CMDArgsHandler.HandleArg(data);
			} while (true);
		}

		private static byte[] dataqueue = new byte[256];
		private static NamedPipeServerStream pipeServer = null;
		private static NamedPipeClientStream pipeClient = null;
	}
}