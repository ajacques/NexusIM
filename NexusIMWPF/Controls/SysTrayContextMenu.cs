using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstantMessage;
using NexusIM.Managers;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	class SysTrayContextMenu : ContextMenu
	{
		public SysTrayContextMenu()
		{
			SetupMenuItems();
			SetupEventHandlers();

			AccountManager.StatusChanged += new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			AccountManager_onStatusChange(null, null);
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
			ContactListItem.FontWeight = FontWeight.FromOpenTypeWeight(700);
			base.Items.Add(ContactListItem);

			MenuItem sendMessageItem = new MenuItem();
			sendMessageItem.Header = "Send Instant Message";
			sendMessageItem.Click += new RoutedEventHandler(SendMessageItem_Click);
			base.Items.Add(sendMessageItem);

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
			InvisibleStatusItem.Header = "Invisible";
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
		private void SetupEventHandlers()
		{
			AvailableStatusItem.Click += new RoutedEventHandler(AvailableStatusItem_Click);
			BusyStatusItem.Click += new RoutedEventHandler(BusyStatusItem_Click);
			InvisibleStatusItem.Click += new RoutedEventHandler(InvisibleStatusItem_Click);
			SignOutItem.Click += new RoutedEventHandler(SignOutItem_Click);
			ExitItem.Click += new RoutedEventHandler(ExitItem_Click);
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

		private void SignOutItem_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Connected = !AccountManager.Connected;
		}
		private void SendMessageItem_Click(object sender, RoutedEventArgs e)
		{
			SelectRecipientWindow window = new SelectRecipientWindow();
			window.Show();
		}
		private void ExitItem_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Connected = false;
			WindowSystem.Application.Shutdown();
		}
		private void ContactListItem_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.OpenContactListWindow();
		}
		private void AccountManager_onStatusChange(object sender, StatusUpdateEventArgs e)
		{
			bool connected = AccountManager.Connected;

			if (!connected)
			{
				Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					AvailabilityGroupItem.IsEnabled = false;
					SignOutItem.Header = "Sign In";
				}));
			} else {
				int onlineCount = AccountManager.Accounts.Count(s => s.Protocol.ProtocolStatus == IMProtocolStatus.Online);
				Dispatcher.BeginInvoke(new GenericEvent(() => {
					AvailabilityGroupItem.IsEnabled = true;
					SignOutItem.Header = "Sign Out";
				}));
		
				switch (AccountManager.Status)
				{
					case IMStatus.Available:
						AvailableStatusItem.Icon = Properties.Resources.point;
						break;
				}
			}
		}
		private void AvailableStatusItem_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Status = IMStatus.Available;
		}
		private void BusyStatusItem_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Status = IMStatus.Busy;
		}
		private void InvisibleStatusItem_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Status = IMStatus.Invisible;
		}
	}
}