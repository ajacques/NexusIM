using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using NexusIM.Managers;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for SocketErrorTrayTip.xaml
	/// </summary>
	public partial class SocketErrorTrayTip : UserControl
	{
		public SocketErrorTrayTip()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VerifyLink.TextDecorations = TextDecorations.Underline;
		}

		private void Hyperlink_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VerifyLink.TextDecorations = null;
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.SysTrayIcon.HideBalloonTip();
		}
	}
}