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
		/// <summary>
		/// Registers this class the platform handling class
		/// Must be run before any Protocols are Created
		/// </summary>
		public static void Setup()
		{
			AccountManager.OnNewAccount += new EventHandler<NewAccountEventArgs>(AccountManager_OnNewAccount);
		}
		public static void Shutdown()
		{
			AccountManager.OnNewAccount -= new EventHandler<NewAccountEventArgs>(AccountManager_OnNewAccount);
		}
		internal static void OpenBuddyWindow(IMBuddy iMBuddy, bool p)
		{
			throw new NotImplementedException();
		}
		private static void AccountManager_OnNewAccount(object sender, NewAccountEventArgs e)
		{
			e.Account.ContactList.CollectionChanged += new NotifyCollectionChangedEventHandler(IMProtocol_ContactListChanged);
		}
		private static void IMProtocol_ContactListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (WindowSystem.ContactListWindow == null || e.Action != NotifyCollectionChangedAction.Add)
				return;

			foreach (IContact contact in e.NewItems)
			{
				WindowSystem.ContactListWindow.AddContact(contact);
			}
		}

		public static event EventHandler onWindowOpen;
	}
}