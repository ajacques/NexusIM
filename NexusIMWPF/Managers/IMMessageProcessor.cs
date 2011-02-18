using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Collections.Specialized;
using InstantMessage.Events;
using NexusIM.Controls;
using System.Windows;

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

		private static void IMProtocol_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			ContactChatArea area;
			if (!WindowSystem.ContactChatAreas.TryGetValue(e.Sender, out area))
			{
				WindowSystem.OpenContactWindow(e.Sender, false);
			}
		}
	}
}