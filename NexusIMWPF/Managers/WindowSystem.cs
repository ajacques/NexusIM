using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Windows;
using NexusIMWPF;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		static WindowSystem()
		{
			ContactChatAreas = new ChatAreaCollection();
			ChatWindows = new Dictionary<int, ChatWindow>();
			OtherWindows = new List<Window>();
		}
		public static void OpenContactListWindow()
		{
			if (ContactListWindow == null)
			{
				Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ContactListWindow = new ContactListWindow();
					ContactListWindow.Show();
					ContactListWindow.Closed += new EventHandler(ContactListWindow_Closed);
				}), DispatcherPriority.Normal);
			} else {
				Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ContactListWindow.Show();
					ContactListWindow.Activate();
				}), DispatcherPriority.Normal);
			}
		}
		public static ContactChatArea OpenContactWindow(IContact contact, bool getFocus = true)
		{
			ContactChatArea area;
			if (ContactChatAreas.TryGetValue(contact, out area))
			{
				
			} else {
				int? poolId = IMSettings.ChatAreaPool.GetPool(contact);
				ChatWindow chatWindow = null;
				if (poolId.HasValue)
				{
					// This contact has a pool
					if (!ChatWindows.TryGetValue(poolId.Value, out chatWindow))
					{
						// Window not yet open. We need to open it
						Application.Dispatcher.Invoke(new GenericEvent(() =>
						{
							chatWindow = new ChatWindow();
							chatWindow.Show();
						}));
						ChatWindows.Add(poolId.Value, chatWindow);
					}
				} else {
					// This contact doesn't have a designated pool
					Application.Dispatcher.Invoke(new GenericEvent(() =>
					{
						chatWindow = new ChatWindow();
						area = new ContactChatArea();
						chatWindow.AttachAreaAndShow(new ChatAreaHost(area, contact));
						chatWindow.Show();
					}));
					ContactChatAreas.Add(contact, area);
				}
			}

			return area;
		}
		public static void ShowSysTrayIcon()
		{
			if (SysTrayIcon != null)
				return;

			Application.Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					SysTrayIcon = new SysTrayIcon();
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
			ContactListWindow.Closed -= new EventHandler(ContactListWindow_Closed);
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
		public static SysTrayIcon SysTrayIcon
		{
			get;
			private set;
		}
		public static ChatAreaCollection ContactChatAreas
		{
			get;
			private set;
		}
		public static Dictionary<int, ChatWindow> ChatWindows
		{
			get;
			private set;
		}
		public static List<Window> OtherWindows
		{
			get;
			private set;
		}
	}
}