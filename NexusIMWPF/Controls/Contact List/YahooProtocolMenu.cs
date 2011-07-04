using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InstantMessage;
using InstantMessage.Protocols.Yahoo;

namespace NexusIM.Controls
{
	class YahooProtocolMenu : ProtocolMenu<IMYahooProtocol>
	{
		protected override string ProtocolName
		{
			get {
				return "Yahoo! Messenger";
			}
		}

		protected override void GenerateItemSet(IMProtocolWrapper wrapper, ItemCollection coll)
		{
			IMYahooProtocol protocol = wrapper.Protocol as IMYahooProtocol;

			MenuItem item = new MenuItem();
			item.Header = protocol.Username;
			item.FontWeight = FontWeight.FromOpenTypeWeight(500);
			item.IsEnabled = false;
			coll.Add(item);

			coll.Add(new Separator());
		}
		protected override ImageSource GetImage()
		{
			Uri uriSource = new Uri("pack://application:,,,/NexusIMWPF;component/Resources/yahoo.png");
			BitmapImage image = new BitmapImage(uriSource);

			return image;
		}
	}
}