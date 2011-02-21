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
			TrayMouseMove += new RoutedEventHandler(SysTrayIcon_TrayMouseMove);
			
		}

		private SysTrayPeekWindow PeekWindow
		{
			get;
			set;
		}

		private void SysTrayIcon_TrayMouseMove(object sender, RoutedEventArgs e)
		{
			if (PeekWindow == null)
			{
				PeekWindow = new SysTrayPeekWindow();
				PeekWindow.Left = SystemParameters.PrimaryScreenWidth - (this.Width + 20);
				PeekWindow.Top = SystemParameters.PrimaryScreenHeight - this.Height;
				PeekWindow.Show();
			}
		}
	}
}