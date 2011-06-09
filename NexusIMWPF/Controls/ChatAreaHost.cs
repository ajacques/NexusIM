using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	class ChatAreaHost : TabItem
	{
		private ChatAreaHost()
		{
			Grid a = new Grid();
			mHeaderString = new TextBlock();
			mHeaderString.HorizontalAlignment = HorizontalAlignment.Left;
			mHeaderString.VerticalAlignment = VerticalAlignment.Center;
			mHeaderString.Padding = new Thickness(0, 0, 10, 0);

			Button closeButton = new Button();
			closeButton.Content = new TextBlock() { Text = "x", VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButton.VerticalAlignment = VerticalAlignment.Center;
			closeButton.Height = 14;
			closeButton.Padding = new Thickness(0);
			closeButton.Margin = new Thickness(0, 0, -4, 0);
			closeButton.Click += new RoutedEventHandler(CloseButton_Click);
			
			StackPanel closeButtontt = new StackPanel();
			closeButtontt.Children.Add(new TextBlock() { Text = "Close Tab", FontWeight = FontWeight.FromOpenTypeWeight(700)});
			closeButton.ToolTip = closeButtontt;

			a.Children.Add(mHeaderString);
			a.Children.Add(closeButton);

			Header = a;
		}
		public ChatAreaHost(ITabbedArea area) : this()
		{
			Content = mArea = area;

			if (mArea is ContactChatArea)
			{
				ContactChatArea chatArea = (ContactChatArea)mArea;
				mHeaderString.Text = chatArea.Contact.Username;
			}
		}
		public ChatAreaHost(IContact context) : this()
		{
			ContactChatArea area = new ContactChatArea();
			area.PopulateUIControls(context);

			Content = mArea = area;
			mListenString = "DisplayName";
		}
		public ChatAreaHost(IChatRoom context, IMProtocol protocol) : this()
		{
			MUCChatArea area = new MUCChatArea();
			area.PopulateUIControls(context, protocol);

			Content = mArea = area;
			mListenString = "Name";
		}

		public PropertyChangedEventHandler GetHeaderChangeSink(string listenString, Func<string> nameSelector)
		{
			mListenString = listenString;
			mNameSelector = nameSelector;
			return new PropertyChangedEventHandler(listenSource_PropertyChanged);
		}

		private void listenSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == mListenString)
			{
				mHeaderString.Text = mNameSelector();
			}
		}
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			HostWindow.HandleTabClose(this);

			if (TabClosed != null)
				TabClosed(this, null);
		}

		public event EventHandler TabClosed;

		public ITabbedArea HostedArea
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
		private ITabbedArea mArea;
		private string mListenString;
		private Func<string> mNameSelector;
	}
}