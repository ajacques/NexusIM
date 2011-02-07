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
		public App()
		{
			// Single instance Lock
			mSingleInstanceMutex = new Mutex(true, "Local\\NexusIM");
			if (!mSingleInstanceMutex.WaitOne(0, false))
			{
				string[] commands = Environment.GetCommandLineArgs();

				if (commands.Length == 1) // Bring to front
					IPCHandler.BringToFront();
				else {
					IPCHandler.OpenConnection();
				}

				this.Shutdown();
			}
			WindowSystem.RegisterApp(this);
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
		}		

		public void Dispose()
		{
			if (mSingleInstanceMutex != null)
				mSingleInstanceMutex.Dispose();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			mStopwatch = new Stopwatch();
			mStopwatch.Start();

			SetupTraceListeners();

			Trace.AutoFlush = true;
			// Log some random information for debugging
			Trace.WriteLine("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
			Trace.WriteLine("Process ID: " + Process.GetCurrentProcess().Id);
			Trace.WriteLine("Operating System: " + Environment.OSVersion.ToString());
			Trace.WriteLine("Working Directory: " + Environment.CurrentDirectory);
			Trace.WriteLine("CLR Version: " + Environment.Version.ToString());
			Trace.WriteLine(string.Format("OS Is64Bit: {0} (Current Process Is64Bit: {1})", Environment.Is64BitOperatingSystem, Environment.Is64BitProcess));

			string configuri = Path.Combine(Environment.CurrentDirectory, "UserData.sdf");
			Trace.WriteLine("Configuration File: " + configuri);
			IMSettings.Setup(new SQLCESettings("Data Source=\"UserProfile.sdf\";Persist Security Info=False;"));

			AggregateContactList.Setup();
			if (FirstRunSetup.IsFirstRun)
			{
				InitialSetupWindow window = new InitialSetupWindow();
				window.Show();
				AccountManager.Setup();
			} else {
				AccountManager.Setup();
				foreach (IMProtocolExtraData protocol in IMSettings.Accounts)
					AccountManager.Accounts.Add(protocol);

				WindowSystem.OpenContactListWindow();
				WindowSystem.ShowSysTrayIcon();
			}

			Trace.WriteLine("Configuration file loaded and parsed.");

			RestartManager.Setup();

			Trace.WriteLine("All Managers are loaded and ready");
			Trace.WriteLine("Load Stopwatch: Application Initialization complete in: " + mStopwatch.Elapsed);
			mStopwatch.Stop();
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			mSingleInstanceMutex.Close();
			SuperTaskbarManager.Shutdown();
		}

		[Conditional("DEBUG")]
		private static void SetupTraceListeners()
		{
			Trace.Listeners.Add(new SocketTraceListener("5.64.115.83", 6536));
			Trace.Listeners.Add(new SocketTraceListener("192.101.0.197", 6536));
						
			try	{
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
		private Stopwatch mStopwatch;
	}
}