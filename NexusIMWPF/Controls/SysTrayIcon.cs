using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hardcodet.Wpf.TaskbarNotification;
using NexusIM.Properties;
using NexusIM.Windows;
using System.Windows.Input;
using System.Windows;

namespace NexusIM.Controls
{
	class SysTrayIcon : TaskbarIcon
	{
		public SysTrayIcon()
		{
			Icon = Properties.Resources.app;
			ContextMenu = new SysTrayContextMenu();
			SysTrayPeekWindow peekWindow = new SysTrayPeekWindow();
			TrayToolTip = peekWindow;
		}
	}
}