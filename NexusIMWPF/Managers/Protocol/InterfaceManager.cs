using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using InstantMessage;
using System.Collections.Specialized;
using NexusIM.Controls;
using NexusIM.Windows;
using System.Threading;
using System.Windows.Threading;

namespace NexusIM.Managers
{
	static class InterfaceManager
	{
		public static void Setup()
		{
			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}
		public static void Shutdown()
		{
		}
		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (IMProtocolExtraData item in e.NewItems)
				item.Protocol.ContactList.CollectionChanged += new NotifyCollectionChangedEventHandler(IMProtocol_ContactListChanged);
		}
		private static void IMProtocol_ContactListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (WindowSystem.ContactListWindow == null || e.Action != NotifyCollectionChangedAction.Add)
				return;

			foreach (IContact contact in e.NewItems)
			{
				
			}
		}
	}
}