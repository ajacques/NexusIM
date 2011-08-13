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
				return SafeNativeMethods.GetSystemMetrics(4096) == 1 || SafeNativeMethods.GetSystemMetrics(8193) == 1;
			}
		}

		public static void FlashWindow(Window window)
		{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			NativeMethods.FlashWindow(helper.Handle, false);
		}
		public static void FlashWindow(IntPtr handle)
		{
			if (!IsWindows())
				return;

			FLASHWINFO fInfo = new FLASHWINFO();
			fInfo.cbSize = Marshal.SizeOf(fInfo);
			fInfo.hwnd = handle;
			fInfo.flags = WindowFlashFlags.Flash_Tray;
			fInfo.uCount = 3;
			fInfo.dwTimeout = 0;

			NativeMethods.FlashWindowEx(ref fInfo);
		}

		[Flags]
		private enum WindowFlashFlags
		{
			/// <summary>
			/// Stop flashing. The system restores the window to its original state.
			/// </summary>
			Stop = 0,
			/// <summary>
			/// Flash the window caption.
			/// </summary>
			Flash_Caption = 1,
			/// <summary>
			/// Flash the taskbar button.
			/// </summary>
			Flash_Tray = 2,
			/// <summary>
			/// Flash continuously, until the Stop flag is set.
			/// </summary>
			Flash_Until_Stop = 4,
			/// <summary>
			/// Flash continuously until the window comes to the foreground.
			/// </summary>
			Flash_Until_Focus = 12,
			/// <summary>
			/// Flash both the window caption and taskbar button. This is equivalent to setting the Flash_Caption and Flash_Tray flags
			/// </summary>
			Flash_All = 3
		}

		public enum QUERY_USER_NOTIFICATION_STATE
		{
			QUNS_NOT_PRESENT = 1,
			QUNS_BUSY = 2,
			QUNS_RUNNING_D3D_FULL_SCREEN = 3,
			QUNS_PRESENTATION_MODE = 4,
			QUNS_ACCEPTS_NOTIFICATIONS = 5,
			QUNS_QUIET_TIME = 6
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct FLASHWINFO
		{
			[MarshalAs(UnmanagedType.U4)]
			public int cbSize;
			public IntPtr hwnd;
			[MarshalAs(UnmanagedType.U4)]
			public WindowFlashFlags flags;
			public uint uCount;
			public uint dwTimeout;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		}

		private static class NativeMethods
		{
			/// <summary>
			/// Flashes the window taskbar button to get the user's attention
			/// </summary>
			/// <param name="hWnd">A handle to the window to be flashed. The window can be either open or minimized.</param>
			/// <param name="bInvert">If this parameter is TRUE, the window is flashed from one state to the other. If it is FALSE, the window is returned to its original state (either active or inactive). </param>
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool FlashWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bInvert);

			/// <summary>
			/// Flashes the window taskbar button to get the user's attention
			/// </summary>
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

			/// <param name="hWnd">Window Handle</param>
			/// <param name="hWndInsertAfter">Placement-order Handle</param>
			/// <param name="X">Horizontal Position</param>
			/// <param name="Y">Vertical Position</param>
			/// <param name="cx">Window Width</param>
			/// <param name="cy">Window Height</param>
			/// <param name="uFlags">Window Positioning Flags</param>
			[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		}
		private static class SafeNativeMethods
		{
			[DllImport("user32.dll")]
			public static extern int GetSystemMetrics(int nIndex);
			
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetCursorPos(ref Win32Point pt);

			/// <summary>
			/// Determines whether the user resumed the computer or if the system did.
			/// </summary>
			/// <returns>True if this resume was triggered by the system</returns>
			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool IsSystemResumeAutomatic();

			[DllImport("shell32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SHQueryUserNotificationState(ref QUERY_USER_NOTIFICATION_STATE qUserState);

			[DllImport("user32.dll", ExactSpelling = true)]
			[return: MarshalAs(UnmanagedType.U4)]
			public static extern int GetDoubleClickTime();
		}

		public static TimeSpan GetDoubleClickSpeed()
		{
			return TimeSpan.FromMilliseconds(SafeNativeMethods.GetDoubleClickTime());
		}
		public static Point GetScreenMousePosition()
		{
			Win32Point w32Mouse = new Win32Point();
			SafeNativeMethods.GetCursorPos(ref w32Mouse);
			return new Point(w32Mouse.X, w32Mouse.Y);
		}
		public static QUERY_USER_NOTIFICATION_STATE SHQueryUserNotificationState()
		{
			QUERY_USER_NOTIFICATION_STATE returnState = 0;

			SafeNativeMethods.SHQueryUserNotificationState(ref returnState);

			return returnState;
		}

		public static bool IsWinVistaAndUp()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
		}
		public static bool IsWin7AndUp()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT && ((Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 7);
		}
		public static bool IsWindows()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		// Constants
		private const int WM_DISPLAYCHANGE = 0x007E;
		private const int WM_POWERBROADCAST = 0x0218;
		private const int WM_SIZING = 0x0214;
		private const int SW_SHOWNOACTIVATE = 4;
		private const uint SWP_NOACTIVATE = 0x0010;
		private const int HWND_TOPMOST = -1;
	}
}