using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Windows;

namespace NexusIM.Managers
{
	static class NotificationQueue
	{
		static NotificationQueue()
		{
			mPendingQueue = new Queue<UserControl>();
			mVisibleQueue = new Queue<ToasterNotification>();
		}

		public static void EnqueueChatMessageArea(IContact source, string message)
		{
			ChatMessageNotification chatmsgnotif = null;
			WindowSystem.Application.Dispatcher.Invoke(new GenericEvent(() => {
				chatmsgnotif = new ChatMessageNotification();
				chatmsgnotif.ContactSource = source;
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
			WindowSystem.Application.Dispatcher.Invoke(new GenericEvent(() => {
				notifWindow = new ToasterNotification();
				notifWindow.NotificationContent = area;
				notifWindow.Show();
			}));

			mVisibleQueue.Enqueue(notifWindow);
		}

		private static Queue<UserControl> mPendingQueue;
		private static Queue<ToasterNotification> mVisibleQueue;
	}
}