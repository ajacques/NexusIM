using System;
using System.Collections.Specialized;
using InstantMessage;
using InstantMessage.Events;
using NexusIM.Controls;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace NexusIM.Managers
{
	class IMMessageProcessor
	{
		public static void Setup()
		{
			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}

		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (IMProtocolExtraData extraData in e.NewItems)
				{
					extraData.Protocol.onMessageReceive += new EventHandler<IMMessageEventArgs>(IMProtocol_OnMessageReceived);
				}
			}
		}

		public static IEnumerable<ChatInline> ProcessComplexMessage(IEnumerable<ChatInline> source)
		{
			List<ChatInline> processed = new List<ChatInline>();
			foreach (ChatInline inline in source)
			{
				if (!(inline is IMRun))
				{
					processed.Add(inline);
					continue;
				}

				IMRun run = (IMRun)inline;
				int index = run.Body.IndexOf("http://");

				index = index == -1 ? run.Body.IndexOf("https://") : index;
				index = index == -1 ? run.Body.IndexOf("ftp://") : index;

				if (index != -1)
				{
					int endIndex = run.Body.IndexOf(' ', index);
					string trailing = endIndex != -1 ? run.Body.Substring(endIndex) : null;

					endIndex = endIndex != -1 ? endIndex : run.Body.Length;

					string hyperlink = run.Body.Substring(index, endIndex - index);

					Uri href;
					try	{
						href = new Uri(hyperlink);
					} catch (UriFormatException) {
						Debug.WriteLine("Uri " + hyperlink + " failed to parse due to invalid format");
						processed.Add(run);
						continue;
					}

					run.Body = run.Body.Substring(0, index);

					HyperlinkInline hinline = new HyperlinkInline(new Uri(hyperlink), hyperlink);
					processed.Add(hinline);
				} else
					processed.Add(run);
			}

			return processed;
		}

		private static void IMProtocol_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			ContactChatArea area;
			bool showNotification = false;
			if (!WindowSystem.ContactChatAreas.TryGetValue(e.Sender, out area))
			{
				area = WindowSystem.OpenContactWindow(e.Sender, false);
				showNotification = true;
			}
			
			area.ProcessChatMessage(e);

			if (false && showNotification)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback((object s) => NotificationQueue.EnqueueChatMessageArea(e.Sender, e.Message)), null);
			}
		}
	}
}