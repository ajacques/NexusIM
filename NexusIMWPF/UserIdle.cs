using System;
using System.Runtime.InteropServices;
using System.Threading;
using InstantMessage;

namespace NexusIM.Managers
{
	static class UserIdle
	{
		[DllImport("user32.dll")]
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
			idleTime = new TimeSpan(0, 5, 0);
			mSuspendEvent = new ManualResetEvent(true);

			idlecheck.Priority = ThreadPriority.Lowest;
			idlecheck.Name = "UserIdle Check";
			idlecheck.IsBackground = true;
			idlecheck.Start();
		}
		private static TimeSpan UserIdleTime()
		{
			LASTINPUTINFO lastInput = new LASTINPUTINFO();
			lastInput.Init();
			GetLastInputInfo(ref lastInput);
			return TimeSpan.FromTicks(Environment.TickCount - Convert.ToInt32(lastInput.dwTime));
		}
		
		// Private Methods
		private static void idleChecker()
		{
			while (idlecheck.ThreadState == ThreadState.Background)
			{
				mSuspendEvent.WaitOne(); // Wait for the go-ahead, used to prevent wasted cpu cycles when the user shouldn't go to idle.

				Thread.Sleep(10000);
			}
		}

		// Variables
		private static TimeSpan idleTime;
		private static Thread idlecheck;
		private static ManualResetEvent mSuspendEvent;
	}
}