using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstantMessage;
using NexusIM.Managers;
using NexusIM.Windows;

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

		public void PopulateControls(SocketException errorType, IMProtocol protocol)
		{
			mProtocol = protocol;

			AccountDetails.Text = String.Format("{0} - {1}", protocol.Username, protocol.Protocol);

			// We will supply our own error message
			switch (errorType.SocketErrorCode)
			{
				case SocketError.ConnectionRefused:
					ErrorDetail.Text = "The server actively refused the connection.";
					break;
				default:
					ErrorDetail.Text = errorType.Message;
					break;
			}
		}

		// UI Event Handlers
		private void Hyperlink_MouseEnter(object sender, MouseEventArgs e)
		{
			VerifyLink.TextDecorations = TextDecorations.Underline;
		}
		private void Hyperlink_MouseLeave(object sender, MouseEventArgs e)
		{
			VerifyLink.TextDecorations = null;
		}
		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			AccountsEdit window = new AccountsEdit();
			window.Show();

			WindowSystem.OtherWindows.Add(window);

			WindowSystem.SysTrayIcon.HideBalloonTip();
		}
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.SysTrayIcon.CloseBalloon();
		}

		// Variables
		private IMProtocol mProtocol;
	}
}