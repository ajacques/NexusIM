using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using InstantMessage;
using NexusIM.Windows;
using NexusIMWPF;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Reflection;

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

			SysTrayIcon = new TaskbarIcon();

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NexusIM.Resources.nexusim.ico");

			SysTrayIcon.Icon = new Icon(stream);
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
		public static TaskbarIcon SysTrayIcon
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