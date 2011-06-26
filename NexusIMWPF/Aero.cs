using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NexusIM
{
	static class Aero
	{
		public static void ExtendGlass(Window window, Thickness thickness)
		{
			try	{
				if (Environment.OSVersion.Version.Major > 5 && DwmIsCompositionEnabled())
				{
					// Get the window handle
					WindowInteropHelper helper = new WindowInteropHelper(window);
					HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);
					mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

					// Get the dpi of the screen
					System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowSrc.Handle);
					float dpiX = desktop.DpiX / 96;
					float dpiY = desktop.DpiY / 96;

					// Set Margins
					MARGINS margins = new MARGINS();
					margins.LeftWidth = (int)(thickness.Left * dpiX);
					margins.RightWidth = (int)(thickness.Right * dpiX);
					margins.BottomHeight = (int)(thickness.Bottom * dpiY);
					margins.TopHeight = (int)(thickness.Top * dpiY);

					window.Background = Brushes.Transparent;

					DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
				} else
					window.Background = SystemColors.WindowBrush;
			} catch (DllNotFoundException) {

			}
		}

		public const int WM_DWMCOMPOSITIONCHANGED = 798;

		private enum DWM_BB
		{
			Enable = 1,
			BlurRegion = 2,
			TransitionMaximized = 4
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct DWM_BLURBEHIND
		{
			public DWM_BB dwFlags;
			public bool fEnable;
			public IntPtr hRgnBlur;
			public bool fTransitionOnMaximized;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct MARGINS
		{
			public int LeftWidth;
			public int RightWidth;
			public int TopHeight;
			public int BottomHeight;
		}
		[DllImport("dwmapi.dll")]
		private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);
		[DllImport("dwmapi.dll")]
		private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
		[DllImport("dwmapi.dll", PreserveSig = false)]
		private static extern bool DwmIsCompositionEnabled();
	}
}
