using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using NexusIM.Managers;
using NexusIM.Windows;

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
			TrayMouseDoubleClick += new RoutedEventHandler(SysTrayIcon_TrayMouseDoubleClick);
		}

		public void ShowTrayTip(SocketErrorTrayTip tip)
		{
			ShowCustomBalloon(tip, System.Windows.Controls.Primitives.PopupAnimation.Slide, null);
		}

		private void SysTrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
		{
			WindowSystem.OpenContactListWindow();
		}
	}
}