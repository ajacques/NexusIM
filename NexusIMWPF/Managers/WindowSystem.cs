using System;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using NexusIM.Controls;
using NexusIM.Properties;
using NexusIM.Windows;
using NexusIMWPF;
using InstantMessage;
using System.Collections.Generic;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		public static void OpenContactListWindow()
		{
			if (ContactListWindow == null)
			{
				Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ContactListWindow listWindow = new ContactListWindow();
					listWindow.Show();
					listWindow.Closed += new EventHandler(ContactListWindow_Closed);
					ContactListWindow = listWindow;
				}), DispatcherPriority.Normal);
			} else {
				Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ContactListWindow.Activate();
				}), DispatcherPriority.Normal);
			}
		}
		public static void OpenDummyWindow()
		{
			if (DummyWindow != null)
				throw new InvalidOperationException("Can't open another dummy window");

			DummyWindow = new DummyWindow();
			//Application.Dispatcher.BeginInvoke(new ThreadStart(() => DummyWindow.Show() ), DispatcherPriority.Background);
		}
		public static void OpenContactWindow(IContact contact)
		{
			Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ChatWindow window = new ChatWindow();
					window.DataContext = contact;
					window.Show();
				}));
		}
		public static void ShowSysTrayIcon()
		{
			if (SysTrayIcon != null)
				return;

			Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					SysTrayIcon = new TaskbarIcon();
					SysTrayIcon.Icon = Resources.app;
					SysTrayIcon.ContextMenu = new SysTrayContextMenu();
				}));
		}
		public static void RegisterApp(App app)
		{
			if (Application != null)
				throw new InvalidOperationException("Application may only be set once");

			Application = app;
		}

		// Event Handlers
		private static void ContactListWindow_Closed(object sender, EventArgs e)
		{
			ContactListWindow = null; // Kill it
		}

		// Properties
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