using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using InstantMessage;
using NexusIM.Managers;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for CLErrorBox.xaml
	/// </summary>
	public partial class CLErrorBox : UserControl
	{
		public CLErrorBox()
		{
			InitializeComponent();

			ProtocolString.Text = "test";
		}

		public void PopulateControls(IMProtocolWrapper protocol, SocketException exception)
		{
			PopulateProtocolControls(protocol.Protocol);

			ErrorString.Text = exception.Message;

			mProtocol = protocol;
		}
		private void PopulateProtocolControls(IMProtocol protocol)
		{
			ProtocolString.Text = protocol.ToString();
		}
		private void Close()
		{
			Panel panel = (Panel)Parent;
			panel.Children.Remove(this);
		}

		// Event handlers
		private void ReconnectLink_Click(object sender, RoutedEventArgs e)
		{
			mProtocol.Protocol.BeginLogin();
			Close();
		}
		private void EditLink_Click(object sender, RoutedEventArgs e)
		{
			mProtocol.Enabled = false;

			AccountsEdit window = (AccountsEdit)WindowSystem.OpenSingletonWindow(typeof(AccountsEdit));
		}
		private void DisableLink_Click(object sender, RoutedEventArgs e)
		{
			mProtocol.Enabled = false;
			Close();
		}

		private IMProtocolWrapper mProtocol;
	}
}