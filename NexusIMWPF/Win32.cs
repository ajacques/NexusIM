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
		public static void FlashWindow(IntPtr handle)
		{
			//FlashWindow(handle, false);

			 FLASHWINFO fInfo = new FLASHWINFO();
			fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
			fInfo.hwnd = handle;
			fInfo.dwFlags = (uint)WindowFlashFlags.Flash_Tray;
			fInfo.uCount = 3;
			fInfo.dwTimeout = 0;

			FlashWindowEx(ref fInfo);
		}

		#region Window P/Invoke Declarations
		
		/// <summary>
		/// Flashes the window taskbar button to get the user's attention
		/// </summary>
		/// <param name="hWnd">A handle to the window to be flashed. The window can be either open or minimized.</param>
		/// <param name="bInvert">If this parameter is TRUE, the window is flashed from one state to the other. If it is FALSE, the window is returned to its original state (either active or inactive). </param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

		[Flags]
		private enum WindowFlashFlags
		{
			Stop = 0,
			Flash_Caption = 1,
			Flash_Tray = 2,
			Flash_Until_Stop = 4,
			Flash_Until_Focus = 12
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FLASHWINFO
		{
			public UInt32 cbSize;
			public IntPtr hwnd;
			public UInt32 dwFlags;
			public UInt32 uCount;
			public UInt32 dwTimeout;
		}

		private const int SW_SHOWNOACTIVATE = 4;
		private const int HWND_TOPMOST = -1;
		private const uint SWP_NOACTIVATE = 0x0010;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd">Window Handle</param>
		/// <param name="hWndInsertAfter">Placement-order Handle</param>
		/// <param name="X">Horizontal Position</param>
		/// <param name="Y">Vertical Position</param>
		/// <param name="cx">Window Width</param>
		/// <param name="cy">Window Height</param>
		/// <param name="uFlags">Window Positioning Flags</param>
		/// <returns></returns>
		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		#endregion

		#region P/Invoke Declarations

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

#endregion

		public static TimeSpan GetDoubleClickSpeed()
		{
			return TimeSpan.FromMilliseconds(GetDoubleClickTime());
		}

		public static bool IsWinVistaAndUp()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32Windows && Environment.OSVersion.Version.Major >= 6;
		}
		public static bool IsWin7AndUp()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32Windows && ((Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 7);
		}
	}
}