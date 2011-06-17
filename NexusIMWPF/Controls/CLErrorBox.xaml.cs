using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using InstantMessage;

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

		public void PopulateControls(IMProtocol protocol, SocketException exception)
		{
			PopulateProtocolControls(protocol);
		}

		private void PopulateProtocolControls(IMProtocol protocol)
		{

		}
	}
}