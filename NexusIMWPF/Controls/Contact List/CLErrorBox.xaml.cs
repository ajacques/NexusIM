using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
		}

		public void PopulateControls(IMProtocolWrapper protocol, SocketException exception)
		{
			PopulateProtocolControls(protocol.Protocol);
			ErrorString.Text = exception.Message;
			mProtocol = protocol;

			AddLink("Reconnect", new RoutedEventHandler(ReconnectLink_Click));
			AddLink("Edit", new RoutedEventHandler(EditLink_Click));
			AddLink("Disable", new RoutedEventHandler(DisableLink_Click));
		}
		public void PopulateProtocolControls(IMProtocol protocol)
		{
			Dispatcher.InvokeIfRequired(() => ProtocolString.Text = protocol.ToString());
		}
		public void SetErrorString(string message)
		{
			Dispatcher.InvokeIfRequired(() => ErrorString.Text = message);
		}
		private void Close()
		{
			Panel panel = (Panel)Parent;
			panel.Children.Remove(this);
		}
		public void AddLink(string body, RoutedEventHandler handler)
		{
			if (LinkBox.Inlines.Count >= 1)
				LinkBox.Inlines.Add(new Run(" ▪ "));;

			Hyperlink link = new Hyperlink();
			link.Inlines.Add(new Run(body));
			link.TextDecorations = null;
			link.Click += handler;
			LinkBox.Inlines.Add(link);
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
			window.FocusProtocol(mProtocol);
		}
		private void DisableLink_Click(object sender, RoutedEventArgs e)
		{
			mProtocol.Enabled = false;
			Close();
		}

		private IMProtocolWrapper mProtocol;
	}
}