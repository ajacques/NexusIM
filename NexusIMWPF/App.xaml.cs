using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using InstantMessage;
using NexusIM.Managers;
using Microsoft.WindowsAPICodePack.Dialogs;
using NexusIM;
using System.Threading;
using NexusIM.Windows;

namespace NexusIMWPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

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

				InterfaceManager.Setup();
				IPCHandler.Setup();

				//string configdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NexusIM");
				//string configuri = Path.Combine(configdir, "config.xml");
				string configuri = "..\\..\\UserData.sdf";

				Trace.WriteLine("Configuration File: " + configuri);

				IMSettings.SettingInterface = new SQLCESettings("Data Source=\"..\\..\\UserData.sdf\";Persist Security Info=False;");

				AccountManager.Setup();
				ErrorManager.Setup();

				// Attempt to load the configuration file
				try	{
					IMSettings.Load();
				} catch (System.Xml.XmlException x) { // Thrown by parser errors
					Trace.TraceWarning("Configuration Parse Error: " + x.Message);
					if (false || Win32.IsWinVistaAndUp())
					{
						TaskDialog tdialog = new TaskDialog();
						tdialog.Icon = TaskDialogStandardIcon.Warning;
						tdialog.Caption = NexusIM.Properties.Resources.ConfigFileParseErrorTitle;
						tdialog.InstructionText = NexusIM.Properties.Resources.ConfigFileParseError.Replace("{messagedata}", x.Message);
						tdialog.Show();
					} else {
						MessageBoxResult result = MessageBox.Show(NexusIM.Properties.Resources.ConfigFileParseError.Replace("{messagedata}", x.Message), NexusIM.Properties.Resources.ConfigFileParseErrorTitle, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

						if (result == MessageBoxResult.OK)
						{
							File.Copy(configuri, configuri + ".bak", true);
						} else if (result == MessageBoxResult.Cancel) {
							mlock.ReleaseMutex();
							return;
						}
					}
				}

				AccountManager.Start();
				if (FirstRunSetup.IsFirstRun)
				{
					InitialSetupWindow window = new InitialSetupWindow();
					window.Show();
				} else {
					WindowSystem.OpenContactListWindow();
				}

				Trace.WriteLine("Configuration file loaded and parsed.");
				
				Notifications.Setup();
				RestartManager.Setup();

				Trace.WriteLine("All Managers are loaded and ready");
			} else {
				IPCHandler.OpenConnection();
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			SuperTaskbarManager.Shutdown();
		}

		[Conditional("DEBUG")]
		private static void SetupTraceListeners()
		{
			try
			{ // Try to create a connection to my hamachi computer logging program
				TcpClient client = new TcpClient();
				client.Connect("5.64.115.83", 6536);
				Trace.Listeners.Add(new TextWriterTraceListener(client.GetStream(), "Network Logger"));
			} catch (SocketException) { }

			Stream file = new FileStream("nexusim_log.txt", FileMode.OpenOrCreate, FileAccess.Write);
			Trace.Listeners.Add(new TextWriterTraceListener(file, "Local File Logger"));
		}
	}
}