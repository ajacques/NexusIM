using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusIM.Windows;
using NexusIMWPF;
using InstantMessage;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace NexusIM.Managers
{
	static class WindowSystem
	{
		public static void OpenContactListWindow()
		{
			if (ContactListWindow == null)
			{
				Application.Dispatcher.BeginInvoke(new GenericEvent(() => {
					ContactListWindow listWindow = new ContactListWindow();
					listWindow.Show();
					ContactListWindow = listWindow;
				}), DispatcherPriority.Normal);				
			}
		}

		public static void ShowSysTrayIcon()
		{
			if (SysTrayIcon != null)
				return;

			SysTrayIcon = new NotifyIcon();

			Stream s = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/nexusim.ico")).Stream;

			SysTrayIcon.Icon = new Icon(s);
			SysTrayIcon.Visible = true;
		}

		public static void RegisterApp(App app)
		{
			if (Application != null)
				throw new InvalidOperationException("Application may only be set once");

			Application = app;
		}

		public static ContactListWindow ContactListWindow
		{
			get;
			private set;
		}
		public static App Application
		{
			get;
			private set;
		}
		public static NotifyIcon SysTrayIcon
		{
			get;
			private set;
		}
	}
}