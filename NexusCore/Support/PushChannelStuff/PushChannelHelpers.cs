using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NexusCore.DataContracts;

namespace NexusCore.PushChannel
{
	[Flags]
	enum PushMessageClass
	{
		Internal,
		Toast,
		Tile
	}

	internal delegate void PushMessageErrorOccurred(PushChannelErrorState state);

	internal static class PushChannel
	{
		static PushChannel()
		{
			mQueueWatchTimer.Elapsed += new ElapsedEventHandler(delegate(object sender, ElapsedEventArgs e)
				{
					CleanMessageQueue();
				});
			mQueueWatchTimer.AutoReset = true;
		}

		public static void PushMessage(IPushMessage message, IPushChannel channel)
		{
			bool newVersion = mMessageQueue.Any(t => t.Message.Equals(message));

			mMessageQueue.RemoveAll(t => t.Message.Equals(message));

			mMessageQueue.Add(new MessageQueueItem(message, channel));
			mQueueWatchTimer.Start();
		}

		/// <summary>
		/// Forces a message to pushed to the client instantly. Disables any anti-flood queuing code.
		/// </summary>
		public static void PushMessageNow(PushChannelType type, Uri uri, IPushMessage message)
		{
			throw new NotImplementedException();
		}
		public static void PushMessagesNow(PushChannelType type, Uri uri, IEnumerable<IPushMessage> messages)
		{
			throw new NotImplementedException();
		}
		
		private static void CleanMessageQueue()
		{
			if (mQueueInCleanUp)
				return;

			mQueueInCleanUp = true;
			try	{
				DateTime now = DateTime.UtcNow;
				
				var immediatePush = mMessageQueue.Where(t => now.Subtract(t.QueuedAt) > t.Message.MaxQueuePeriod); // These messages need to be pushed
				var friendPush = mMessageQueue.Where(t => immediatePush.Any(t2 => t.Channel == t2.Channel)); //Join(immediatePush, t => t.Item2, t => t.Item2, (left, inner) => inner); // doesn't seem to work. Generates 14 Messages out of only 6 Messages

				if (!mMessageQueue.Any())
					mQueueWatchTimer.Stop();

				var dictionary = friendPush.GroupBy(t => t.Channel, t => t.Message);

				foreach (var pusher in dictionary)
				{
					pusher.Key.PushMessages(pusher);
				}
				mMessageQueue.RemoveAll(t => friendPush.Contains(t));
			} finally {
				mQueueInCleanUp = false;
			}			
		}
		private static bool CanBeCombined(PushChannelType type, IPushMessage message)
		{
			if (type == PushChannelType.GenericTcp)
				return true;
			else if (type == PushChannelType.MicrosoftPN && message.MessageClass == PushMessageClass.Internal)
				return true;
			else
				return false;
		}

		private class MessageQueueItem
		{
			public MessageQueueItem(IPushMessage message, IPushChannel channel)
			{
				Channel = channel;
				QueuedAt = DateTime.UtcNow;
				Message = message;
			}
			public DateTime QueuedAt
			{
				get;
				private set;
			}
			public IPushMessage Message
			{
				get;
				private set;
			}
			public IPushChannel Channel
			{
				get;
				private set;
			}
		}

		private static Timer mQueueWatchTimer = new Timer(500);
		private static bool mQueueInCleanUp = false;
		private static List<MessageQueueItem> mMessageQueue = new List<MessageQueueItem>();
	}
}