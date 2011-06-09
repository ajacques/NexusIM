﻿using System;
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
				int? poolId = IMSettings.ChatAreaPool.GetPool(contact.Protocol, contact.Username);
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
						chatWindow.Closed += new EventHandler(ChatWindow_Closed);
						area = new ContactChatArea();
						ChatAreaHost host = new ChatAreaHost(area);
						area.PopulateUIControls(contact);
						host.GetHeaderChangeSink("DisplayName", () => contact.Username)(contact, new System.ComponentModel.PropertyChangedEventArgs("DisplayName"));
						host.TabClosed += new EventHandler(ChatAreaHost_TabClosed);
						chatWindow.AttachAreaAndShow(host);
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
		public static void DispatcherInvoke(GenericEvent target)
		{
			if (!Application.Dispatcher.CheckAccess())
				Application.Dispatcher.BeginInvoke(target);
			else
				target();
		}
		public static void PlaceInCorrectWindowPool(IMProtocol protocol, string poolObjectId, ITabbedArea area)
		{
			int? poolId = IMSettings.ChatAreaPool.GetPool(protocol, poolObjectId);

			ChatWindow window = null;
			if (poolId.HasValue) // If it has an assigned pool
			{
				if (ChatWindows.TryGetValue(poolId.Value, out window)) // If the required window is already open
				{
					window.AttachAreaAndShow(new ChatAreaHost(area));
				} else {
					window = new ChatWindow();
					window.AttachAreaAndShow(new ChatAreaHost(area));
					ChatWindows.Add(poolId.Value, window);
				}
			} else {
				window = new ChatWindow();
				window.AttachAreaAndShow(new ChatAreaHost(area));
			}
			window.Show();
		}

		// Event Handlers
		private static void ContactListWindow_Closed(object sender, EventArgs e)
		{
			ContactListWindow.Closed -= new EventHandler(ContactListWindow_Closed);
			ContactListWindow = null; // Kill it
		}
		private static void ChatWindow_Closed(object sender, EventArgs e)
		{
			ChatWindow window = (ChatWindow)sender;
			
		}
		private static void ChatAreaHost_TabClosed(object sender, EventArgs e)
		{
			ChatAreaHost host = (ChatAreaHost)sender;
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
		/// <summary>
		/// Contains any other windows that the WindowSystem might have to handle.
		/// </summary>
		public static List<Window> OtherWindows
		{
			get;
			private set;
		}
	}
}