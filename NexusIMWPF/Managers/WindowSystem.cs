using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusIM.Windows;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		public static void OpenContactListWindow()
		{
			if (ContactListWindow == null)
			{
				ContactListWindow listWindow = new ContactListWindow();
				listWindow.Show();
				ContactListWindow = listWindow;
			}
		}

		public static ContactListWindow ContactListWindow
		{
			get;
			private set;
		}
	}
}