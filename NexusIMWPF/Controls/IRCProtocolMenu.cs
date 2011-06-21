using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessage.Protocols.Irc;
using NexusIM.Managers;
using NexusIM.Windows;

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

			IEnumerable<IRCProtocol> protocols = AccountManager.Accounts.Select(w => w.Protocol).OfType<IRCProtocol>();
			ItemCollection target = this.Items;

			foreach (IRCProtocol protocol in protocols)
			{
				GenerateItemSet(protocol, target);
			}
		}

		private void GenerateItemSet(IRCProtocol protocol, ItemCollection coll)
		{
			MenuItem item = new MenuItem();
			item.Header = String.Format("{0} ({1})", protocol.Nickname, protocol.Server);
			item.FontWeight = FontWeight.FromOpenTypeWeight(500);
			item.IsEnabled = false;
			coll.Add(item);

			coll.Add(new Separator());

			MenuItem joinItem = new MenuItem();
			joinItem.Header = "Join Chat Room";
			joinItem.Click += new RoutedEventHandler(joinItem_Click);

			coll.Add(joinItem);
		}
	}
}