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
using NexusIM.Properties;
using System.Windows.Threading;

namespace NexusIMWPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			WindowSystem.RegisterApp(this);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			SetupTraceListeners();
			Trace.AutoFlush = true;
			
			// Log some random information for debugging
			Trace.WriteLine("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
			Trace.WriteLine("Process ID: " + Process.GetCurrentProcess().Id);
			Trace.WriteLine("Operating System: " + Environment.OSVersion.ToString());
			Trace.WriteLine("Working Directory: " + Environment.CurrentDirectory);
			Trace.WriteLine("CLR Version: " + Environment.Version.ToString());
			Trace.WriteLine(string.Format("OS Is64Bit: {0} (Current Process Is64Bit: {1})", Environment.Is64BitOperatingSystem, Environment.Is64BitProcess));

			// Single instance Lock
			Mutex mlock = new Mutex(true, "Local\\NexusIM");
			if (mlock.WaitOne(0, false))
			{
				Trace.WriteLine("Mutex Locked and Loaded");

				InterfaceManager.Setup();
				IPCHandler.Setup();

				string configuri = Path.Combine(Environment.CurrentDirectory, "UserData.sdf");

				Trace.WriteLine("Configuration File: " + configuri);

				IMSettings.Setup(new SQLCESettings("Data Source=\"UserProfile.sdf\";Persist Security Info=False;"));

				AccountManager.Setup();

				foreach (IMProtocol protocol in IMSettings.Accounts)
					AccountManager.AddNewAccount(protocol);
				Trace.WriteLine(String.Format("{0} Accounts Loaded", AccountManager.Accounts.Count()));

				WindowSystem.OpenDummyWindow();
				if (FirstRunSetup.IsFirstRun)
				{
					InitialSetupWindow window = new InitialSetupWindow();
					window.Show();
				} else {
					WindowSystem.OpenContactListWindow();
					WindowSystem.ShowSysTrayIcon();
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
			Trace.Listeners.Add(new SocketTraceListener("5.64.115.83", 6536));
			Trace.Listeners.Add(new SocketTraceListener("192.101.0.197", 6536));
			
			Stream file = new FileStream("nexusim_log.txt", FileMode.OpenOrCreate, FileAccess.Write);
			Trace.Listeners.Add(new TextWriterTraceListener(file, "Local File Logger"));
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Debug.WriteLine(e.Exception.ToString());
		}
	}
}