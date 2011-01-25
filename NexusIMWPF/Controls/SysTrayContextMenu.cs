using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using NexusIM.Managers;

namespace NexusIM.Controls
{
	class SysTrayContextMenu : ContextMenu
	{
		public SysTrayContextMenu()
		{
			SetupMenuItems();

			AccountManager.StatusChanged += new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
		}		

		private void SetupMenuItems()
		{
			// Show Contact List
			// My Availability
			//     - Available
			//     - Away
			//     - Busy
			//     - Invisible
			// Sign Out
			// ----
			// Exit

			ContactListItem = new MenuItem();
			ContactListItem.Header = "Show Contact List";
			ContactListItem.Click += new RoutedEventHandler(ContactListItem_Click);
			base.Items.Add(ContactListItem);

			AvailabilityGroupItem = new MenuItem();
			AvailabilityGroupItem.Header = "My Availability";
			base.Items.Add(AvailabilityGroupItem);

			AvailableStatusItem = new MenuItem();
			AvailableStatusItem.Header = "Available";
			AvailabilityGroupItem.Items.Add(AvailableStatusItem);

			BusyStatusItem = new MenuItem();
			BusyStatusItem.Header = "Busy";
			AvailabilityGroupItem.Items.Add(BusyStatusItem);

			InvisibleStatusItem = new MenuItem();
			BusyStatusItem.Header = "Invisible";
			AvailabilityGroupItem.Items.Add(InvisibleStatusItem);

			AvailabilityGroupItem.Items.Add(new Separator());

			MenuItem changeStatusMessage = new MenuItem();
			changeStatusMessage.Header = "My Status Message";
			AvailabilityGroupItem.Items.Add(changeStatusMessage);

			SignOutItem = new MenuItem();
			SignOutItem.Header = "Sign Out";
			base.Items.Add(SignOutItem);

			this.Items.Add(new Separator());

			ExitItem = new MenuItem();
			ExitItem.Header = "Exit";
			base.Items.Add(ExitItem);
		}

		public MenuItem ContactListItem
		{
			get;
			private set;
		}
		public MenuItem ExitItem
		{
			get;
			private set;
		}
		public MenuItem SignOutItem
		{
			get;
			private set;
		}
		public MenuItem AvailableStatusItem
		{
			get;
			private set;
		}
		public MenuItem BusyStatusItem
		{
			get;
			private set;
		}
		public MenuItem InvisibleStatusItem
		{
			get;
			private set;
		}
		public MenuItem AvailabilityGroupItem
		{
			get;
			private set;
		}

		private void ContactListItem_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.OpenContactListWindow();
		}
		private void AccountManager_onStatusChange(object sender, StatusUpdateEventArgs e)
		{
			int onlineCount = AccountManager.Accounts.Count(p => p.Enabled);

			if (onlineCount == 0)
			{
				AvailabilityGroupItem.IsEnabled = false;
				SignOutItem.Header = "Sign In";
			}
		}
	}
}