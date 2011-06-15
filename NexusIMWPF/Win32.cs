using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NexusIM
{
	internal static class Win32
	{
		public static bool IsInRdpSession
		{
			get	{
				return GetSystemMetrics(4096) == 1 || GetSystemMetrics(8193) == 1;
			}
		}

		public static void FlashWindow(Window window)
		{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			FlashWindow(helper.Handle, false);
		}

		/// <summary>
		/// Flashes the window taskbar button to get the user's attention
		/// </summary>
		/// <param name="hWnd">A handle to the window to be flashed. The window can be either open or minimized.</param>
		/// <param name="bInvert">If this parameter is TRUE, the window is flashed from one state to the other. If it is FALSE, the window is returned to its original state (either active or inactive). </param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int nIndex);

		public enum QUERY_USER_NOTIFICATION_STATE
		{
			QUNS_NOT_PRESENT = 1,
			QUNS_BUSY = 2,
			QUNS_RUNNING_D3D_FULL_SCREEN = 3,
			QUNS_PRESENTATION_MODE = 4,
			QUNS_ACCEPTS_NOTIFICATIONS = 5,
			QUNS_QUIET_TIME = 6
		}

		public static int WM_DISPLAYCHANGE = 0x007E;
		public static int WM_POWERBROADCAST = 0x0218;
		public static int WM_SIZING = 0x0214;

		public static Point GetScreenMousePosition()
		{
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			return new Point(w32Mouse.X, w32Mouse.Y);
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPos(ref Win32Point pt);

		/// <summary>
		/// Determines whether the user resumed the computer or if the system did.
		/// </summary>
		/// <returns>True if this resume was triggered by the system</returns>
		[DllImport("kernel32.dll")]
		public static extern bool IsSystemResumeAutomatic();

		[DllImport("shell32.dll")]
		private static extern bool SHQueryUserNotificationState(ref QUERY_USER_NOTIFICATION_STATE qUserState);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int PostMessage(IntPtr hwnd, int msg, int wparam, int lparam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetLastError();

		[DllImport("user32.dll", ExactSpelling = true)]
		private static extern int GetDoubleClickTime();

		public static QUERY_USER_NOTIFICATION_STATE SHQueryUserNotificationState()
		{
			QUERY_USER_NOTIFICATION_STATE returnState = 0;

			SHQueryUserNotificationState(ref returnState);

			return returnState;
		}

		public static TimeSpan GetDoubleClickSpeed()
		{
			return TimeSpan.FromMilliseconds(GetDoubleClickTime());
		}

		public static bool IsWinVistaAndUp()
		{
			return (Environment.OSVersion.Version.Major >= 6);
		}
		public static bool IsWin7AndUp()
		{
			return ((Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 7);
		}
	}
}