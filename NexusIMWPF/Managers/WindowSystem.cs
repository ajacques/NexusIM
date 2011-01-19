using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using InstantMessage;
using NexusIM.Windows;
using NexusIMWPF;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		public static void OpenContactListWindow()
		{
			if (ContactListWindow == null)
			{
				Application.Dispatcher.BeginInvoke(new GenericEvent(() => {
					ContactListWindow listWindow = new ContactListWindow();
					listWindow.Show();
					ContactListWindow = listWindow;
				}), DispatcherPriority.Normal);				
			}
		}
		public static void OpenDummyWindow()
		{
			if (DummyWindow != null)
				throw new InvalidOperationException();

			DummyWindow = new DummyWindow();
			//Application.Dispatcher.BeginInvoke(new ThreadStart(() => DummyWindow.Show() ), DispatcherPriority.Background);
		}
		public static void ShowSysTrayIcon()
		{
			if (SysTrayIcon != null)
				return;

			SysTrayIcon = new NotifyIcon();

			Stream s = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/nexusim.ico")).Stream;

			SysTrayIcon.Icon = new Icon(s);
			SysTrayIcon.Visible = true;
		}
		public static void RegisterApp(App app)
		{
			if (Application != null)
				throw new InvalidOperationException("Application may only be set once");

			Application = app;
		}

		public static ContactListWindow ContactListWindow
		{
			get;
			private set;
		}
		public static App Application
		{
			get;
			private set;
		}
		public static NotifyIcon SysTrayIcon
		{
			get;
			private set;
		}
		public static DummyWindow DummyWindow
		{
			get;
			private set;
		}
	}
}