using System;
using System.IO;
using System.Reflection;
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

		public static void WriteMiniDump()
		{
			string fileName = String.Format("{0}_mini.dmp", Assembly.GetEntryAssembly().Location, DateTime.Now);

			FileStream file = new FileStream(fileName, FileMode.Create);
			MinidumpExceptionInfo info = new MinidumpExceptionInfo();
			info.ClientPointers = false;
			info.ExceptionPointers = Marshal.GetExceptionPointers();
			info.ThreadId = SafeNativeMethods.GetCurrentThreadId();
			MiniDumpType type = MiniDumpType.WithoutOptionalData | MiniDumpType.IncludeDataSegments | MiniDumpType.WithThreadInfo;
			NativeMethods.MiniDumpWriteDump(NativeMethods.GetCurrentProcess(), SafeNativeMethods.GetCurrentProcessId(), file.SafeFileHandle.DangerousGetHandle(), type, ref info, IntPtr.Zero, IntPtr.Zero);
			file.Close();
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
		private static QUERY_USER_NOTIFICATION_STATE SHQueryUserNotificationState()
		{
			QUERY_USER_NOTIFICATION_STATE returnState = 0;

			SafeNativeMethods.SHQueryUserNotificationState(ref returnState);

			return returnState;
		}

		// OS Version Helper Functions
		public static bool IsWinVistaAndUp()
		{
			return IsWindows() && Environment.OSVersion.Version.Major >= 6;
		}
		public static bool IsWin7AndUp()
		{
			return IsWindows() && (IsWinVistaAndUp() || Environment.OSVersion.Version.Major >= 7);
		}
		public static bool IsWindows()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		[Flags]
		private enum MiniDumpType
		{
			Normal,
			IncludeDataSegments = 1,
			FullMemory = 2,
			WithHandleData = 4,
			FilterMemory = 8,
			ScanMemory = 16,
			WithUnloadedModules = 32,
			WithIndirectlyReferencedMemory = 64,
			FilterModulePaths = 128,
			WithProcessThreadData = 256,
			WithPrivateReadWriteMemory = 512,
			WithoutOptionalData = 1024,
			WithFullMemoryInfo = 2048,
			WithThreadInfo = 4096,
			WithCodeSegs = 8192,
			WithoutAuxiliaryState = 16384,
			WithFullAuxiliaryState = 0x8000,
			WithPrivateWriteCopyMemory = 0x10000,
			IgnoreInaccessibleMemory = 0x20000,
			WithTokenInformation = 0x40000
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

		private enum QUERY_USER_NOTIFICATION_STATE
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
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct MinidumpExceptionInfo
		{
			public uint ThreadId;
			public IntPtr ExceptionPointers;
			[MarshalAs(UnmanagedType.Bool)]
			public bool ClientPointers;
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

			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

			[DllImport("kernel32.dll")]
			public static extern IntPtr GetCurrentProcess();

			[DllImport("dbghelp.dll")]
			public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, IntPtr hFile, MiniDumpType DumpType, ref MinidumpExceptionInfo ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);
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

			[DllImport("kernel32.dll")]
			public static extern uint GetCurrentProcessId();

			[DllImport("kernel32.dll")]
			public static extern uint GetCurrentThreadId();
		}
	}
}