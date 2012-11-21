using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NexusIM.Controls
{
	public class HostValidationTextBox : TextBox
	{
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			ClearResults();

			DoVerify();
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			ClearResults();
		}

		private void ClearResults()
		{
			Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			ToolTip = null;
		}
		private void DoVerify()
		{
			string host = Text;

			if (String.IsNullOrWhiteSpace(host))
			{
				ClearResults();
				return;
			}

			IPAddress ip = null;
			if (IPAddress.TryParse(host, out ip))
			{
				if (ip.ToString() == host)
					ShowResults(Color.FromRgb(190, 255, 190), () => {
						Span span = new Span();
						span.Inlines.Add(new Run("The specified IP address is valid."));
						span.Inlines.Add(new LineBreak());
						span.Inlines.Add(new Run("Type: "));
						Run iptyperun = new Run();
						iptyperun.FontWeight = FontWeight.FromOpenTypeWeight(700);
						switch (ip.AddressFamily)
						{
							case AddressFamily.InterNetwork:
								iptyperun.Text = "IPv4";
								break;
							case AddressFamily.InterNetworkV6:
								iptyperun.Text = "IPv6";
								break;
							default:
								iptyperun.Text = ip.AddressFamily.ToString();
								break;
						}
						span.Inlines.Add(iptyperun);

						return span;
					});
				else {
					GenerateText exec = () => {
						Span span = new Span();
						span.Inlines.Add(new Run("The specified IP address is valid but may not have been parsed correctly."));
						span.Inlines.Add(new LineBreak());
						span.Inlines.Add(new Run("Parsed as: ") { FontWeight = FontWeight.FromOpenTypeWeight(700) });
						span.Inlines.Add(new Run(ip.ToString()));
						return span;
					};
					
					ShowResults(Color.FromRgb(230, 255, 90), exec);
				}
			} else {
				Dns.BeginGetHostAddresses(host, new AsyncCallback(OnDnsResolve), null);
			}
		}
		private void ShowResults(Color color, GenerateText exec)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ToolTip tip = new ToolTip();
				StackPanel panel = new StackPanel();

				panel.Children.Add(new TextBlock(exec()));

				tip.Content = panel;
				ToolTip = tip;

				Background = new SolidColorBrush(color);
			}));
		}
		private void OnDnsResolve(IAsyncResult e)
		{
			try {
				Dns.EndGetHostAddresses(e);
			} catch (SocketException x) {
				GenerateText exec = () => {
					Span span = new Span();
					span.Inlines.Add(new Run("The specified address might be invalid."));
					span.Inlines.Add(new LineBreak());
					span.Inlines.Add(new Run(x.Message));

					return span;
				};

				ShowResults(Color.FromRgb(255, 190, 190), exec);
				return;
			}

			ShowResults(Color.FromRgb(190, 255, 190), () => new Run("DNS Lookup succeeded.\r\nThe specified hostname appears to be valid."));
		}

		private delegate Inline GenerateText();
	}
}