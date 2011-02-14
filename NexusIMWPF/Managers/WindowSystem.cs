using System;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Properties;
using NexusIM.Windows;
using NexusIMWPF;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		static WindowSystem()
		{
			ContactChatAreas = new ChatAreaCollection();
		}
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
		public static void OpenContactWindow(IContact contact, bool getFocus = true)
		{
			ContactChatArea area;
			if (!ContactChatAreas.TryGetValue(contact, out area))
			{
				Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					area = new ContactChatArea();
					ContactChatAreas.Add(contact, area);
					if (ChatWindow == null)
					{
						ChatWindow = new ChatWindow();
						if (!getFocus)
							ChatWindow.WindowState = WindowState.Minimized;
						ChatWindow.Show();
						ChatWindow.Visibility = Visibility.Visible;
						if (!getFocus)
							Win32.FlashWindow(ChatWindow);
					}
					ChatWindow.AttachAreaAndShow(new ChatAreaHost(area, contact));
				}));
			}
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
		public static ChatAreaCollection ContactChatAreas
		{
			get;
			private set;
		}
		public static ChatWindow ChatWindow
		{
			get;
			private set;
		}
	}
}