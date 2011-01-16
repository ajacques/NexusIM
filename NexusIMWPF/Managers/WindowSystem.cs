using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusIM.Windows;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		public static void OpenContactList()
		{
			if (ContactList == null)
			{
				ContactList listWindow = new ContactList();
				listWindow.Show();
				ContactList = listWindow;
			}
		}

		public static ContactList ContactList
		{
			get;
			private set;
		}
	}
}