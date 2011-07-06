using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Misc;
using NexusIM.Windows;
using NexusIMWPF;
using InstantMessage.Protocols;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		static WindowSystem()
		{
			ChatWindows = new ChatWindowCollection();
			OtherWindows = new List<Window>();
			ChatAreas = new SortedDictionary<AreaSortPoolKey, Tuple<ChatWindow, UIElement>>();
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
		public static void OpenContactWindow(IContact contact, bool getFocus = true)
		{
			Tuple<ChatWindow, UIElement> tuple = PlaceInCorrectWindowPool(contact.Protocol, contact.Username, () => new ContactChatArea(contact));
			
			if (getFocus && !tuple.Item1.IsVisible)
				DispatcherInvoke(() => tuple.Item1.Show());
			else if (!tuple.Item1.IsVisible)
				DispatcherInvoke(() => tuple.Item1.Show());
		}
		public static void OpenGroupChatWindow(IChatRoom chatRoom)
		{
			Tuple<ChatWindow, UIElement> tuple = PlaceInCorrectWindowPool(chatRoom.Protocol, chatRoom.Name, () => new GroupChatAreaHost(chatRoom));

			if (!tuple.Item1.IsVisible)
				DispatcherInvoke(() => tuple.Item1.Show());
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
		public static void DispatcherInvoke(GenericEvent target, bool asAsync = true)
		{
			if (!Application.Dispatcher.CheckAccess())
			{
				if (asAsync)
					Application.Dispatcher.BeginInvoke(target);
				else
					Application.Dispatcher.Invoke(target);
			} else
				target();
		}
		public static Window OpenSingletonWindow(Type windowType)
		{
			foreach (Window window in OtherWindows)
			{
				if (window.GetType() == windowType)
				{
					window.Activate();
					return window;
				}
			}

			Window newWin = (Window)Activator.CreateInstance(windowType);
			EventHandler closeHandler = null;
			closeHandler = (sender, args) => {
				newWin.Closed -= closeHandler;
				OtherWindows.Remove(newWin);
			};

			newWin.Closed += closeHandler;
			OtherWindows.Add(newWin);
			newWin.Show();

			return newWin;
		}
		/// <summary>
		/// Places a TabItem in the correct window pool or returns the window if it's already open.
		/// </summary>
		/// <param name="protocol">Specifies which window pool to search.</param>
		/// <param name="poolObjectId">Specifies the object id that defines which window pool the TabItem goes in.</param>
		/// <param name="mutator">Specifies a factory function that will return a newly created ChatArea (ex. ContactChatArea). Will be called if the ChatArea isn't already open.</param>
		/// <returns>A tuple containing the ChatWindow and ChatArea.</returns>
		public static Tuple<ChatWindow, UIElement> PlaceInCorrectWindowPool(IMProtocol protocol, string poolObjectId, Func<UIElement> mutator)
		{
			AreaSortPoolKey key = new AreaSortPoolKey(protocol, poolObjectId); // Used to sort the binary tree

			Tuple<ChatWindow, UIElement> tabItem = null;
			if (ChatAreas.TryGetValue(key, out tabItem)) // Quickly check to see if the Area is already open
				return tabItem;

			int? poolId = IMSettings.ChatAreaPool.GetPool(protocol, poolObjectId);

			ChatWindow window = null;
			if (poolId.HasValue) // If it has an assigned pool
			{
				if (!ChatWindows.TryGetValue(poolId.Value, out window)) // If the required window is already open
				{
					DispatcherInvoke(() => window = new ChatWindow(), false);
					
					ChatWindows.Add(poolId.Value, window);
				}
			} else {
				DispatcherInvoke(() => window = new ChatWindow(), false);
				//window.AttachAreaAndShow(new ContactChatAreaHost(area));

				ChatWindows.Add(-1, window);
			}

			UIElement item = null;
			// Never do anything other than UI-code on the UI thread
			// We want to remain responsive at all times
			DispatcherInvoke(() =>
			{
				item = mutator();
				window.AttachAreaAndShow(item);
			}, false);

			var result = Tuple.Create(window, item);

			ChatAreas.Add(key, result);

			window.Closed += new EventHandler(ChatWindow_Closed);

			return result;
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
			
			var area = ChatAreas.Where(t => t.Value.Item1 == window).FirstOrDefault();
			ChatAreas.Remove(area.Key);
		}
		private static void ChatAreaHost_TabClosed(object sender, EventArgs e)
		{
			ContactChatAreaHost host = (ContactChatAreaHost)sender;
		}

		// Nested Classes
		private class AreaSortPoolKey : IComparable<AreaSortPoolKey>
		{
			public AreaSortPoolKey(IMProtocol protocol, string poolId)
			{
				mProtocol = protocol;
				mPoolId = poolId;
			}

			public int CompareTo(AreaSortPoolKey other)
			{
				if (mProtocol.Protocol != other.mProtocol.Protocol)
					return mProtocol.Protocol.CompareTo(other.mProtocol.Protocol);

				if (mProtocol.Username != other.mProtocol.Username)
					return mProtocol.Username.CompareTo(other.mProtocol.Username);

				if (mPoolId != other.mPoolId)
					return mPoolId.CompareTo(other.mPoolId);

				return 0;
			}

			private IMProtocol mProtocol;
			private string mPoolId;
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
		public static ChatWindowCollection ChatWindows
		{
			get;
			private set;
		}
		private static SortedDictionary<AreaSortPoolKey, Tuple<ChatWindow, UIElement>> ChatAreas
		{
			get;
			set;
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