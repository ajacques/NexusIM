using System;
using System.Diagnostics;
using System.IO;
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
	/// Interaction logic for App.xaml
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
			if (!mSingleInstanceMutex.WaitOne(0, false))
			{
				string[] commands = Environment.GetCommandLineArgs();

				if (commands.Length == 1) // Bring to front
					IPCHandler.BringToFront();
				else
					IPCHandler.OpenConnection();

				this.Shutdown();
				return;
			}
			IPCHandler.Setup();
			WindowSystem.RegisterApp(this);
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			DoInit();
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
		}

		private static void LoadAccounts(object state)
		{
			StopwatchManager.Start("AccDBLoad");
			foreach (IMProtocolExtraData protocol in IMSettings.Accounts)
				AccountManager.Accounts.Add(protocol);
			StopwatchManager.Stop("AccDBLoad");
		}
		private static void DoInit()
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

			string configuri = Path.Combine(Environment.CurrentDirectory, "UserData.sdf");
			Trace.WriteLine("Configuration File: " + configuri);
			IMSettings.Setup(new SQLCESettings("Data Source=\"UserProfile.sdf\";Persist Security Info=False;"));

			AggregateContactList.Setup();
			IMMessageProcessor.Setup();
			if (FirstRunSetup.IsFirstRun)
			{
				InitialSetupWindow window = new InitialSetupWindow();
				window.Show();
				StopwatchManager.TraceElapsed("AppInit", "{0} - InitialSetup Opened in: {1}");
				AccountManager.Setup();
			} else
			{
				AccountManager.Setup();
				StopwatchManager.TraceElapsed("AppInit", "{0} - AccountManager loaded in: {1}");
				ThreadPool.QueueUserWorkItem(new WaitCallback(LoadAccounts), null); // We don't need the accounts loaded immediately since they slow down the application startup due to SQLCE module loading. Schedule them to be loaded later

				WindowSystem.OpenContactListWindow();
				WindowSystem.ShowSysTrayIcon();
			}

			Trace.WriteLine("Configuration file loaded and parsed.");

			RestartManager.Setup();

			Trace.WriteLine("All Managers are loaded and ready");
			StopwatchManager.TraceElapsed("AppInit", "{0} - Application initialization completed in: {1}");
		}

		[Conditional("DEBUG")]
		private static void SetupTraceListeners()
		{
			Trace.Listeners.Add(new SocketTraceListener("5.64.115.83", 6536));
			Trace.Listeners.Add(new SocketTraceListener("192.101.0.197", 6536));

			try
			{
				Stream file = new FileStream("nexusim_log.txt", FileMode.OpenOrCreate, FileAccess.Write);
				Trace.Listeners.Add(new TextWriterTraceListener(file, "Local File Logger"));
			} catch (IOException) { }
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Debug.WriteLine(e.Exception.ToString());
		}

		// Variables
		private Mutex mSingleInstanceMutex;
	}
}