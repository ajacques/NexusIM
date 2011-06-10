using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	class ContactChatAreaHost : TabItem
	{
		private ContactChatAreaHost()
		{
			Grid headerGrid = new Grid();
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(18) });
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });

			mHeaderString = new TextBlock();
			mHeaderString.Padding = new Thickness(2, 0, 5, 0);
			Grid.SetColumn(mHeaderString, 1);

			TextBlock closeButton = new TextBlock();
			closeButton.Text = "×";
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;			
			Grid.SetColumn(closeButton, 2);
			
			StackPanel closeButtontt = new StackPanel();
			closeButtontt.Children.Add(new TextBlock() { Text = "Close Tab", FontWeight = FontWeight.FromOpenTypeWeight(700)});
			closeButton.ToolTip = closeButtontt;

			headerGrid.Children.Add(mHeaderString);
			headerGrid.Children.Add(closeButton);

			Header = headerGrid;

			ResourceDictionary dict = new ResourceDictionary();
			dict.Source = new Uri("/NexusIMWPF;component/Windows/ChatWindowResources.xaml", UriKind.RelativeOrAbsolute);
			Style = (Style)dict["CustomTabItem"];
		}
		public ContactChatAreaHost(IContact context) : this()
		{
			ContactChatArea area = new ContactChatArea();
			area.Contact = context;
			mHeaderString.Text = area.Contact.Username;

			Content = mArea = area;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			HostWindow.HandleTabClose(this);

			if (TabClosed != null)
				TabClosed(this, null);
		}

		public event EventHandler TabClosed;

		public ContactChatArea HostedArea
		{
			get	{
				return mArea;
			}
		}
		public ChatWindow HostWindow
		{
			get	{
				return mWindow;
			}
			set	{
				mWindow = value;
			}
		}

		private ChatWindow mWindow;
		private TextBlock mHeaderString;
		private ContactChatArea mArea;
	}
}