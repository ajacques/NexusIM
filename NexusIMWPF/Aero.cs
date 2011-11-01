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
				if (Environment.OSVersion.Version.Major > 5 && NativeMethods.DwmIsCompositionEnabled())
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

					NativeMethods.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
				} else
					window.Background = SystemColors.WindowBrush;
			} catch (DllNotFoundException) {

			}
		}

		public static Color GetColorizationColor()
		{
			int color;
			bool isOpaque;
			NativeMethods.DwmGetColorizationColor(out color, out isOpaque);

			// Use some bitwise operator magic to extract each separate color value
			// Format: 0xAARRGGBB
			byte blue = (byte)(color & 0xff); // 0xBB
			byte green = (byte)((color & 0xff00) >> 8); // 0x0000GG00 -> 0xGG
			byte red = (byte)((color & 0xff0000) >> 16); // 0x00RR0000 -> 0xRR
			byte alpha = (byte)((color & 0xff000000) >> 24); // 0xAA000000 -> 0xAA

			return Color.FromArgb(alpha, red, green, blue);
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

		private static class NativeMethods
		{
			[DllImport("dwmapi.dll")]
			public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

			[DllImport("dwmapi.dll")]
			public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

			[DllImport("dwmapi.dll", PreserveSig = false)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool DwmIsCompositionEnabled();

			[DllImport("dwmapi.dll", PreserveSig = false)]
			public static extern void DwmGetColorizationColor([MarshalAs(UnmanagedType.U4)] out int ColorizationColor, [MarshalAs(UnmanagedType.Bool)] out bool ColorizationOpaqueBlend); 
		}		
	}
}
