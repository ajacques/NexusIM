using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Windows;
using System;
using InstantMessage.Events;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace NexusIM.Managers
{
	static class NotificationQueue
	{
		static NotificationQueue()
		{
			mPendingQueue = new Queue<UserControl>();
			mVisibleQueue = new Queue<ToasterNotification>();
		}

		public static void EventWireup()
		{
			IMProtocol.AnyMessageReceived += new EventHandler<IMMessageEventArgs>(IMProtocol_AnyMessageReceived);
		}

		private static void IMProtocol_AnyMessageReceived(object sender, IMMessageEventArgs e)
		{
			EnqueueChatMessageArea(e.Sender, e.Message);
		}

		public static void EnqueueChatMessageArea(IContact source, string message)
		{
			ChatMessageNotification chatmsgnotif = null;
			WindowSystem.Application.Dispatcher.Invoke(new GenericEvent(() => {
				chatmsgnotif = new ChatMessageNotification();
				chatmsgnotif.PopulateUI(source, message);
			}));

			EnqueueArea(chatmsgnotif);
		}

		public static void EnqueueArea(UserControl area)
		{
			mPendingQueue.Enqueue(area);
			ProcessQueue();
		}

		private static void ProcessQueue()
		{
			if (!mPendingQueue.Any())
				return;

			if (mVisibleQueue.Count >= 5)
				return;

			UserControl area = mPendingQueue.Dequeue();
			ToasterNotification notifWindow = null;
			Popup popup = new Popup();
			
			WindowSystem.Application.Dispatcher.Invoke(new GenericEvent(() => {
				notifWindow = new ToasterNotification();
				notifWindow.NotificationContent = area;
				notifWindow.Show();
			}));

			mVisibleQueue.Enqueue(notifWindow);
		}

		private static void PositionPopup(Popup popup)
		{
		}

		private static Queue<UserControl> mPendingQueue;
		private static Queue<ToasterNotification> mVisibleQueue;
	}
}