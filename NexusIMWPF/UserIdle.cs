using System;
using System.Runtime.InteropServices;
using System.Threading;
using InstantMessage;

namespace NexusIM.Managers
{
	static class UserIdle
	{
		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		private struct LASTINPUTINFO
		{
			public void Init()
			{
				cbSize = (uint)Marshal.SizeOf(this);
			}
			public uint cbSize;
			public uint dwTime;
		};

		// Public Methods
		/// <summary>
		/// Starts a thread that periodically checks to see if the user is idle
		/// </summary>
		public static void Setup()
		{
			idlecheck = new Thread(new ThreadStart(idleChecker));
			mSuspendEvent = new ManualResetEvent(true);

			idlecheck.Priority = ThreadPriority.Lowest;
			idlecheck.Name = "UserIdle Check";
			idlecheck.IsBackground = true;
			idlecheck.Start();
		}
		public static void Shutdown()
		{
			mShutdownRequested = true;
			mSuspendEvent.Reset();
		}
		public static void SuspendTimer()
		{
			mSuspendEvent.Set();
		}
		public static void ResumeTimer()
		{
			mSuspendEvent.Reset();
		}
		public static int UserIdleTime()
		{
			LASTINPUTINFO lastInput = new LASTINPUTINFO();
			lastInput.Init();
			GetLastInputInfo(ref lastInput);
			return Environment.TickCount - Convert.ToInt32(lastInput.dwTime);
		}
		
		// Properties
		public static bool IsIdle
		{
			get {
				int time = Convert.ToInt32(IMSettings.Settings["timetoidle"]);
				return UserIdleTime() / 1000 > (time * 60);
			}
		}

		// Private Methods
		private static void idleChecker()
		{
			while (idlecheck.ThreadState == ThreadState.Background)
			{
				mSuspendEvent.WaitOne(); // Wait for the go-ahead, used to prevent wasted cpu cycles when the user shouldn't go to idle.

				if (!IsIdle && idlestatus != 1)
				{
					if (onUserReturn != null)
						onUserReturn(null, null);
					idlestatus = 1;
				} else if (IsIdle && idlestatus != -1) {
					if (onUserIdle != null)
						onUserIdle(null, null);
					idlestatus = -1;
				}

				Thread.Sleep(5000);
			}
		}

		// Events
		public static event EventHandler onUserIdle;
		public static event EventHandler onUserReturn;

		// Variables
		private static int idlestatus = 1; // 1 is here; -1 is away
		private static Thread idlecheck;
		private static ManualResetEvent mSuspendEvent;
		private static bool mShutdownRequested;
	}
}