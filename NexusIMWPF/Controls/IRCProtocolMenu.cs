using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessage.Protocols.Irc;
using NexusIM.Managers;
using NexusIM.Windows;
using InstantMessage;

namespace NexusIM.Controls
{
	class IRCProtocolMenu : ContextMenu
	{
		public IRCProtocolMenu()
		{
			
		}

		// Event handlers
		private void joinItem_Click(object sender, RoutedEventArgs e)
		{
			JoinChatRoom window = new JoinChatRoom();
			window.Owner = Window.GetWindow(this);
			window.ShowDialog();
		}

		protected override void OnOpened(RoutedEventArgs e)
		{
			base.OnOpened(e);

			this.Items.Clear();

			IEnumerable<IMProtocolWrapper> protocols = AccountManager.Accounts.Where(w => w.Protocol is IRCProtocol);

			if (protocols.Count() >= 2)
			{
				MenuItem header = new MenuItem();
				header.Header = "Internet Relay Chat";
				header.FontWeight = FontWeight.FromOpenTypeWeight(500);
				header.IsEnabled = false;
				this.Items.Add(header);
				this.Items.Add(new Separator());

				foreach (IMProtocolWrapper wrapper in protocols)
				{
					MenuItem main = new MenuItem();
					this.Items.Add(main);
					main.Header = wrapper.Protocol.ToString();
					GenerateItemSet(wrapper, main.Items);
				}
			} else
				GenerateItemSet(protocols.FirstOrDefault(), this.Items);
		}

		private void GenerateItemSet(IMProtocolWrapper wrapper, ItemCollection coll)
		{
			if (wrapper == null)
				return;

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
	}
}