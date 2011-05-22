using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using InstantMessage;
using NexusIM.Windows;

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
		public static void StartServer()
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
				string input;

				// Merge all command line arguments into one string
				input = Environment.GetCommandLineArgs().Skip(1).Aggregate((l, r) => l + " " + r);

				input = input.TrimEnd(new char[] { ' ' });
				StreamWriter writer = new StreamWriter(pipeClient);
				writer.WriteLine("CMDARG  " + input);
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
			StreamWriter writer = new StreamWriter(pipeServer);
			writer.AutoFlush = true;

			while (pipeServer.IsConnected)
			{
				string line = reader.ReadLine();
				if (line == null)
					break;

				int firstParam = line.IndexOf(' ');
				if (firstParam == -1)
				{
					writer.WriteLine("ERROR Malformed request.");
					continue;
				}
				string command = line.Substring(0, firstParam).ToLowerInvariant();

				switch (command)
				{
					case "sendim":
						ProcessSendIMMessage(line.Substring(7));
						break;
					case "cmdarg":
						ProcessCommandArgMessage(line.Substring(7));
						break;
					default:
						if (pipeServer.IsConnected)
							writer.WriteLine("ERROR Unrecognized command");
						continue;
				}
			}

			pipeServer.Disconnect();
			pipeServer.BeginWaitForConnection(new AsyncCallback(onNamedPipeRead), null);
		}

		private static void ProcessCommandArgMessage(string input)
		{
			CMDArgsHandler.HandleArg(input);
		}
		private static void ProcessSendIMMessage(string input)
		{
			string[] chunks = input.Split(' ');
			string fileName = null;
			string recipient = null;
			string account = null;

			foreach (string chunk in chunks)
			{
				int keyvalsep = chunk.IndexOf('=');

				if (keyvalsep != -1)
				{
					string key = chunk.Substring(0, keyvalsep).ToLowerInvariant();
					string value = chunk.Substring(keyvalsep + 1);

					switch (key)
					{
						case "file":
							fileName = DecodeIfNeeded(value);							
							break;
						case "recipient":
							recipient = DecodeIfNeeded(value);
							break;
						case "account":
							account = DecodeIfNeeded(value);
							break;
					}
				}
			}

			IEnumerable<IMProtocolWrapper> actualAccounts = null;
			if (account != null)
			{
				int keyvalsep = account.IndexOf(':');
				actualAccounts = AccountManager.Accounts;

				if (keyvalsep != -1)
				{
					string type = account.Substring(0, keyvalsep).ToLowerInvariant();
					string username = account.Substring(keyvalsep + 1);					

					if (!String.IsNullOrWhiteSpace(type))
						actualAccounts = actualAccounts.Where(ed => ed.Protocol.Protocol == type);

					if (!String.IsNullOrWhiteSpace(username))
						actualAccounts = actualAccounts.Where(ed => ed.Protocol.Username == username);
				} else
					actualAccounts = actualAccounts.Where(ed => ed.Protocol.Username == account);
			}

			// Open the window
			WindowSystem.Application.Dispatcher.BeginInvoke(new GenericEvent(() => {
				SelectRecipientWindow window = new SelectRecipientWindow();
				window.Show();
				window.Activate();
				WindowSystem.OtherWindows.Add(window);
			}));
		}

		private static string DecodeIfNeeded(string parameter)
		{
			if (parameter.StartsWith("b64:"))
				return Encoding.UTF8.GetString(Convert.FromBase64String(parameter.Substring(4)));
			else
				return parameter;
		}

		private static NamedPipeServerStream pipeServer;
		private static NamedPipeClientStream pipeClient;
	}
}