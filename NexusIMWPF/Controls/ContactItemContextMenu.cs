using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using InstantMessage;

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
			SendIMItem.Header = "Send Instant Message";
			base.Items.Add(SendIMItem);
		}

		public MenuItem SendIMItem
		{
			get;
			private set;
		}
	}
}