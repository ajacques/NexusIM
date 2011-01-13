using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using InstantMessage;
using Microsoft.WindowsAPICodePack.Dialogs;
using NexusIM.Managers;
using NexusIM.Properties;

namespace NexusIM
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Setup generic error handlers for all threads
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

			SetupTraceListeners();
			Trace.AutoFlush = true;
			
			// Log some random information for debugging
			Trace.WriteLine("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
			Trace.WriteLine("Operating System: " + Environment.OSVersion.ToString());
			Trace.WriteLine("Working Directory: " + Environment.CurrentDirectory);
			Trace.WriteLine("CLR Version: " + Environment.Version.ToString());

			// Single instance Lock
			Mutex mlock = new Mutex(true, "Local\\NexusIM");
			if (mlock.WaitOne(0, false))
			{
				Trace.WriteLine("Mutex Locked");

				ProtocolManager.Setup();
				IPCHandler.Setup();

				string configdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NexusIM");
				string configuri = Path.Combine(configdir, "config.xml");

				Trace.WriteLine("Configuration File: " + configuri);

				IMSettings.SettingInterface = new BasicXmlSettingsBinding(configuri);

				AccountManager.Setup();
				ErrorManager.Setup();

				try	{
					IMSettings.Load();
				} catch (System.Xml.XmlException e) { // Thrown by parser errors
					Trace.TraceWarning("Configuration Parse Error: " + e.Message);
					if (false || Win32.IsWinVistaAndUp())
					{
						TaskDialog tdialog = new TaskDialog();
						tdialog.Icon = TaskDialogStandardIcon.Warning;
						tdialog.Caption = Resources.ConfigFileParseErrorTitle;
						tdialog.InstructionText = Resources.ConfigFileParseError.Replace("{messagedata}", e.Message);
						tdialog.Show();
					} else {
						DialogResult result = MessageBox.Show(Resources.ConfigFileParseError.Replace("{messagedata}", e.Message), Resources.ConfigFileParseErrorTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

						if (result == DialogResult.OK)
						{
							File.Copy(configuri, configuri + ".bak", true);
						} else if (result == DialogResult.Cancel) {
							mlock.ReleaseMutex();
							return;
						}
					}
				}

				if (FirstRunSetup.IsFirstRun)
					FirstRunSetup.HandleFirstRun();
				else if (FirstRunSetup.IsNetworkDeployed || !String.IsNullOrEmpty(IMSettings.GetCustomSetting("devicetoken", "")))
					NexusCoreManager.LoginAsDevice();

				Trace.WriteLine("Configuration file loaded and parsed.");
				
				Notifications.Setup();
				RestartManager.Setup();

				Trace.WriteLine("All Managers are loaded and ready");

				try	{
					Application.Run(new frmMain());
				} catch (Exception e) {
					Trace.TraceError(e.Message + "\r\n" + e.Data + "\r\n" + e.Source + "\r\n" + e.StackTrace);
					MessageBox.Show(e.Message + "\r\n" + e.Data + "\r\n" + e.Source + "\r\n" + e.StackTrace);
				}
				AccountManager.DisconnectAll();
				AccountManager.Status = IMStatus.OFFLINE;
				ProtocolManager.Shutdown();
				mlock.ReleaseMutex();
			} else {
				IPCHandler.OpenConnection();
				Application.Exit();
			}
		}

		[Conditional("DEBUG")]
		private static void SetupTraceListeners()
		{
			BooleanSwitch sb = new BooleanSwitch("remotelog", "Tells whether or not remote logging is enabled");

			if (!sb.Enabled)
				return;

			try	{ // Try to create a connection to my hamachi computer logging program
				TcpClient client = new TcpClient();
				client.Connect("5.64.115.83", 6536);
				Trace.Listeners.Add(new TextWriterTraceListener(client.GetStream(), "Network Logger"));
			} catch (SocketException) {}
		
			Stream file = new FileStream("nexusim_log.txt", FileMode.OpenOrCreate, FileAccess.Write);
			Trace.Listeners.Add(new TextWriterTraceListener(file, "Local File Logger"));
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Trace.TraceError("Exception Occurred: " + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
			MessageBox.Show("An exception has occurred during execution. A copy of this error has been submitted to the network log server if it is running and has been saved to nexusim_log.txt.");
		}
	}
}