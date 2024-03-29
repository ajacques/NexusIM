﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using NexusIM;

namespace NexusIM.Managers
{
	static class RestartManager
	{
		public static void Setup()
		{
			if (!Win32.IsWinVistaAndUp()) // Vista and up
				return;

			ApplicationRecovery.OnApplicationCrash += new ApplicationRecovery.ApplicationCrashHandler(BeginRestart);
			ApplicationRecovery.RegisterForRestart();
		}

		private static void BeginRestart()
		{
			Trace.WriteLine("Possible Crash Detected. Saving program state");
			FileStream fstream = File.Create(Path.GetTempPath() + "\\nexusim_recovery.txt");
			StreamWriter writer = new StreamWriter(fstream);
			
			writer.WriteLine("-windows");
			if (WindowSystem.ContactListWindow.IsVisible)
				writer.WriteLine("contactlist");

			writer.Flush();
			writer.Close();

			ApplicationRecovery.ApplicationRecoveryFinished(true);

			Trace.WriteLine("Recovery information saved. Shutting down");
		}

		private class ApplicationRecovery
		{
			#region Consts and Externs
			const string APPLICATION_CRASHED = "-restartmgr";

			[Flags]
			private enum RestartFlags
			{
				NONE = 0,
				RESTART_NO_CRASH = 1,
				RESTART_NO_HANG = 2,
				RESTART_NO_PATCH = 4,
				RESTART_NO_REBOOT = 8
			}

			[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.U4)]
			private static extern int RegisterApplicationRestart(string pwsCommandLine, RestartFlags dwFlags);

			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.U4)]
			private static extern int RegisterApplicationRecoveryCallback(IntPtr pRecoveryCallback, IntPtr pvParameter, int dwPingInterval, int dwFlags);

			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.U4)]
			private static extern int ApplicationRecoveryInProgress(out bool pbCancelled);

			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.U4)]
			public static extern int ApplicationRecoveryFinished(bool bSuccess);
			#endregion

			#region Delegates & Events
			private delegate int ApplicationRecoveryCallback(IntPtr pvParameter);
			public delegate void ApplicationCrashHandler();

			/// <summary>
			/// Handle this event.  
			/// This is where you will attempt to persist your data.
			/// </summary>
			public static event ApplicationCrashHandler OnApplicationCrash;
			#endregion

			#region Register the application for restart notification.
			private static ApplicationRecoveryCallback RecoverApplication;

			/// <summary>
			/// Registers the application for notification by windows of a failure.
			/// </summary>
			/// <returns>true if successfully registered for restart notification</returns>   
			public static bool RegisterForRestart()
			{
				int i = RegisterApplicationRestart(APPLICATION_CRASHED, RestartFlags.NONE);

				if (i == 0)
				{
					//Hook the callback function.
					RecoverApplication = new ApplicationRecoveryCallback(HandleApplicationCrash);
					IntPtr ptrOnApplicationCrash = Marshal.GetFunctionPointerForDelegate(RecoverApplication);

					i = RegisterApplicationRecoveryCallback(ptrOnApplicationCrash, IntPtr.Zero, 50000, 0);
				}

				return i == 0;
			}
			#endregion

			#region Data Persistance Methods
			/// <summary>
			/// This is the callback function that is executed in the event of the application crashing.
			/// It calls our event handler for OnPersistData.
			/// </summary>
			/// <param name="pvParameter"></param>
			/// <returns></returns>
			private static int HandleApplicationCrash(IntPtr pvParameter)
			{
				//Allow the user to cancel the recovery.  The timer polls for that cancel.
				using (System.Threading.Timer t = new System.Threading.Timer(CheckForRecoveryCancel, null, 1000, 1000))
				{
					//Handle this event in your own code
					if (OnApplicationCrash != null)
					{
						OnApplicationCrash();
						//Note: We will reload the data from persistant storage when the application restarts.
					}

					ApplicationRecoveryFinished(true);
				}

				return 0;
			}

			/// <summary>
			/// Checks to see if the user has cancelled the recovery.
			/// </summary>
			/// <param name="o"></param>
			private static void CheckForRecoveryCancel(object o)
			{
				bool userCancelled;
				ApplicationRecoveryInProgress(out userCancelled);

				if (userCancelled)
				{
					Environment.FailFast("User cancelled application recovery");
				}
			}
			#endregion
		}
	}
}