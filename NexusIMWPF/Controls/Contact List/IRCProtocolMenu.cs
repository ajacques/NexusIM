using System;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Protocols.Irc;
using NexusIM.Windows;
using NexusIM.Windows.IRC;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NexusIM.Controls
{
	sealed class IRCProtocolMenu : ProtocolMenu<IRCProtocol>
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

			if (protocol.ProtocolStatus == IMProtocolStatus.Online)
			{
				MenuItem joinItem = new MenuItem();
				joinItem.Header = "Join Chat Room";
				joinItem.Click += new RoutedEventHandler(joinItem_Click);
				coll.Add(joinItem);

				if (protocol.IsOperator)
					coll.Add(GenerateAdminMenu(protocol));
			}
		}
		protected override ImageSource GetImage()
		{
			Uri uriSource = new Uri("pack://application:,,,/NexusIMWPF;component/Resources/crown.png");
			BitmapImage image = new BitmapImage(uriSource);

			return image;
		}
		protected override string AccountDescription(IMProtocolWrapper wrapper)
		{
			return String.Format("{0}@{1}", wrapper.Protocol.Username, wrapper.Protocol.Server);
		}

		private MenuItem GenerateAdminMenu(IRCProtocol protocol)
		{
			MenuItem root = new MenuItem();
			root.Header = "Net Admin";

			MenuItem links = new MenuItem();
			links.Header = "Show Server Links";
			links.Click += new RoutedEventHandler((sender, e) => OpenServerLinkWindow(protocol));
			root.Items.Add(links);

			return root;
		}

		private void OpenServerLinkWindow(IRCProtocol protocol)
		{
			ServerLinkWindow window = new ServerLinkWindow();
			//window.Owner = Window.GetWindow(this);
			window.LoadData(protocol);
			window.Show();
		}

		protected override string ProtocolName
		{
			get {
				return "Internet Relay Chat";
			}
		}
		protected override bool AutoCreateConnectItem
		{
			get	{
				return true;
			}
		}
	}
}