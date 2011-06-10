using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using InstantMessage;
using System.Windows;

namespace NexusIM.Controls
{
	class ContactItemContextMenu : ContextMenu
	{
		public ContactItemContextMenu(IContact source)
		{
			SetupMenuItems();
		}

		private void SetupMenuItems()
		{
			SendIMItem = new MenuItem();
			SendIMItem.Header = "Instant Message...";
			SendIMItem.FontWeight = FontWeight.FromOpenTypeWeight(700);
			SendIMItem.Click += new RoutedEventHandler(SendIMItem_Click);
			base.Items.Add(SendIMItem);
		}

		private void SendIMItem_Click(object sender, RoutedEventArgs e)
		{
			
		}

		public MenuItem SendIMItem
		{
			get;
			private set;
		}
	}
}