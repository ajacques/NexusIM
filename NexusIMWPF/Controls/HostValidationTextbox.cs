using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Net.Sockets;

namespace NexusIM.Controls
{
	public class HostValidationTextbox : TextBox
	{
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			DoVerify();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

		}

		private void DoVerify()
		{
			string host = Text;

			IPAddress ip = null;
			if (IPAddress.TryParse(host, out ip))
			{
				
			} else {
				Dns.BeginGetHostAddresses(host, new AsyncCallback(OnDnsResolve), null);
			}
		}

		private void OnDnsResolve(IAsyncResult e)
		{
			IPAddress[] addr;
			try {
				addr = Dns.EndGetHostAddresses(e);
			} catch (SocketException x) {
				
			}
		}
	}
}