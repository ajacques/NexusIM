using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using InstantMessage;
using NexusIM;
using NexusIM.Managers;
using NexusIM.Windows;

namespace NexusIMWPF
{
	/// <summary>
	/// The entry point for NexusIM
	/// Initializes all required managers and classes
	/// </summary>
	public partial class App : Application, IDisposable
	{
		[STAThread]
		public static void Main()
		{
			StopwatchManager.Start("AppInit");
			App app = new App();
			app.Run();
		}

		public App()
		{
			// Single instance Lock
			mSingleInstanceMutex = new Mutex(true, "Local\\NexusIM");
			if (!mSingleInstanceMutex.WaitOne(0, false)) // Check to see if there is already an instance running
			{
				string[] commands = Environment.GetCommandLineArgs();

				if (commands.Length == 1) // Bring to front
					IPCHandler.BringToFront();
				else
					IPCHandler.OpenConnection();

				mSingleInstanceMutex.Dispose();
				this.Shutdown();
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(DoInit), null);

			WindowSystem.RegisterApp(this);
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
		}

		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Trace.WriteLine("Unhandled Exception: " + e.Exception.Message);
			Trace.WriteLine(e.Exception.StackTrace);
		}

		public void Dispose()
		{
			if (mSingleInstanceMutex != null)
				mSingleInstanceMutex.Dispose();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			mSingleInstanceMutex.Close();
			SuperTaskbarManager.Shutdown();
			this.Dispose();
		}

		private static void DelayedStart(object state)
		{
			IPCHandler.StartServer();
			AccountManager.Setup();
			NotificationQueue.EventWireup();
		}
		private static void LoadAccounts(object state)
		{
			StopwatchManager.Start("AccDBLoad");

			try {
				Assembly.Load("System.Data.SqlServerCe, Version=4.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");
			} catch (FileNotFoundException) {
				if (MessageBox.Show("Failed to load System.Data.SqlServerCe (v4.0).\r\nWould you like to download it now?", "NexusIM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=033cfb76-5382-44fb-bc7e-b3c8174832e2");
				Trace.WriteLine("Failed to load System.Data.SqlServerCe. User does not appear to have SQLCE 4.0 installed");
				Environment.Exit(-1);
				return;
			}

			IEnumerable<IMProtocolWrapper> accounts;

			try	{
				accounts = IMSettings.Accounts;
			} catch (Exception e) {
				Trace.WriteLine("ERROR: Failed to accounts from the local configuration file (Reason: " + e.Message);
				Trace.WriteLine(e.StackTrace);
				return;
			}

			IEnumerator<IMProtocolWrapper> accEnumer = accounts.GetEnumerator();

			while (accEnumer.MoveNext())
				AccountManager.Accounts.Add(accEnumer.Current);

			StopwatchManager.Stop("AccDBLoad");
			accEnumer.Dispose();
		}
		private static void DoInit(object state)
		{
			SetupTraceListeners();
			StopwatchManager.TraceElapsed("AppInit", "{0} - OnStartup begin: {1}");

			Trace.AutoFlush = true;
			// Log some random information for debugging
			Trace.WriteLine("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
			Trace.WriteLine("Process ID: " + Process.GetCurrentProcess().Id);
			Trace.WriteLine("Operating System: " + Environment.OSVersion.ToString());
			Trace.WriteLine("Working Directory: " + Environment.CurrentDirectory);
			Trace.WriteLine("CLR Version: " + Environment.Version.ToString());
			Trace.WriteLine(string.Format("OS Is64Bit: {0} (Current Process Is64Bit: {1})", Environment.Is64BitOperatingSystem, Environment.Is64BitProcess));
			Trace.WriteLineIf(Debugger.IsAttached, "Debugger is attached");
			Trace.WriteLineIf(!Debugger.IsAttached, "No debugger attached");

			// Load the configuration file
			string configuri = Path.Combine(Environment.CurrentDirectory, "UserProfile.sdf");
			Trace.WriteLine("Configuration File: " + configuri);

			IMSettings.Setup(new SQLCESettings("Data Source=\"" + configuri + "\";Persist Security Info=False;"));

			AggregateContactList.Setup();
			IMMessageProcessor.Setup();
			ThreadPool.QueueUserWorkItem(new WaitCallback(DelayedStart), null);
			if (FirstRunSetup.IsFirstRun)
			{
				InitialSetupWindow window = new InitialSetupWindow();
				window.Show();
				StopwatchManager.TraceElapsed("AppInit", "{0} - InitialSetup Opened in: {1}");
			} else {
				ThreadPool.QueueUserWorkItem(new WaitCallback(LoadAccounts), null); // We don't need the accounts loaded immediately since they slow down the application startup due to SQLCE module loading. Schedule them to be loaded later

				WindowSystem.OpenContactListWindow();
				WindowSystem.ShowSysTrayIcon();
			}

			Trace.WriteLine("Configuration file loaded and parsed.");

			//RestartManager.Setup();

			Trace.WriteLine("All Managers are loaded and ready");
			StopwatchManager.TraceElapsed("AppInit", "{0} - Application initialization completed in: {1}");
		}

		[Conditional("DEBUG")]
		private static void SetupTraceListeners()
		{
			try	{
				Stream file = new FileStream("nexusim_log.txt", FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write);
				Trace.Listeners.Add(new TextWriterTraceListener(file, "Local File Logger"));
			} catch (IOException) { }

			Trace.Listeners.Add(new SocketTraceListener("pub.nexus-im.com", 6536));
			//Trace.Listeners.Add(new SocketTraceListener("192.101.0.197", 6536));
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Debug.WriteLine(e.Exception.ToString());
		}

		// Variables
		private Mutex mSingleInstanceMutex;
	}
}