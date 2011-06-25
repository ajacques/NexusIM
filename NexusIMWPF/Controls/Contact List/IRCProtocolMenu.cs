using System;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Protocols.Irc;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	class IRCProtocolMenu : ProtocolMenu<IRCProtocol>
	{
		// Event handlers
		private void joinItem_Click(object sender, RoutedEventArgs e)
		{
			JoinChatRoom window = new JoinChatRoom();
			window.Owner = Window.GetWindow(this);
			window.ShowDialog();
		}
		
		protected override void GenerateItemSet(IMProtocolWrapper wrapper, ItemCollection coll)
		{
			IRCProtocol protocol = wrapper.Protocol as IRCProtocol;

			MenuItem item = new MenuItem();
			item.Header = String.Format("{0} ({1})", protocol.Nickname, protocol.Server);
			item.FontWeight = FontWeight.FromOpenTypeWeight(500);
			item.IsEnabled = false;
			coll.Add(item);

			coll.Add(new Separator());

			if (wrapper.Enabled)
			{
				if (protocol.ProtocolStatus == IMProtocolStatus.Connecting)
				{
					MenuItem connError = new MenuItem();
					connError.Header = "Account is connecting...";
					connError.IsEnabled = false;
					coll.Add(connError);
				} else if (protocol.ProtocolStatus == IMProtocolStatus.Online) {
					MenuItem joinItem = new MenuItem();
					joinItem.Header = "Join Chat Room";
					joinItem.Click += new RoutedEventHandler(joinItem_Click);

					coll.Add(joinItem);
				}
			} else {
				MenuItem connItem = new MenuItem();
				connItem.Header = "Connect";
				RoutedEventHandler handler = null;
				handler = new RoutedEventHandler((sender, args) =>
				{
					wrapper.Enabled = true;
					connItem.Click -= handler;
					handler = null;
				});

				connItem.Click += handler;

				coll.Add(connItem);
			}
		}

		protected override string ProtocolName
		{
			get {
				return "Internet Relay Chat";
			}
		}
	}
}