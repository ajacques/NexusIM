using System;
using System.Windows;
using System.Windows.Controls;
using InstantMessage.Protocols;
using NexusIM.Windows;
using InstantMessage.Events;

namespace NexusIM.Controls
{
	class GroupChatAreaHost : TabItem
	{
		private GroupChatAreaHost()
		{
			Grid headerGrid = new Grid();
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(18) });
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });

			mHeaderString = new TextBlock();
			mHeaderString.Padding = new Thickness(2, 0, 5, 0);
			Grid.SetColumn(mHeaderString, 1);

			Grid closeButtonGrid = new Grid();
			Grid.SetColumn(closeButtonGrid, 2);

			TextBlock closeButton = new TextBlock();
			closeButton.Text = "×";
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButton.Margin = new Thickness(0, -0.5, 0, 0);
			closeButton.UseLayoutRounding = false;
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButtonGrid.Children.Add(closeButton);
			
			StackPanel closeButtontt = new StackPanel();
			closeButtontt.Children.Add(new TextBlock() { Text = "Close Tab", FontWeight = FontWeight.FromOpenTypeWeight(700)});
			closeButton.ToolTip = closeButtontt;

			headerGrid.Children.Add(mHeaderString);
			headerGrid.Children.Add(closeButtonGrid);

			Header = headerGrid;

			ResourceDictionary dict = new ResourceDictionary();
			dict.Source = new Uri("/NexusIMWPF;component/Windows/ChatWindowResources.xaml", UriKind.RelativeOrAbsolute);
			Style = (Style)dict["CustomTabItem"];
		}
		public GroupChatAreaHost(IChatRoom context) : this()
		{
			MUCChatArea area = new MUCChatArea();
			area.PopulateUIControls(context, this);

			Content = mArea = area;
			mRoom = context;

			context.OnMessageReceived += new EventHandler<IMMessageEventArgs>(ChatRoom_OnMessageReceived);

			mHeaderString.Text = context.Name;
		}

		public override string ToString()
		{
			return mRoom.Name;
		}

		private void ChatRoom_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			if (HostWindow != null)
				HostWindow.IncrementUnread();
		}

		// User Interface Event Handlers
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			HostWindow.HandleTabClose(this);
		}
		private void ParentWindow_Closed(object sender, EventArgs e)
		{
			mRoom.Leave(String.Empty);
		}

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);

			ChatWindow window = (ChatWindow)Window.GetWindow(this);
			window.Closed += new EventHandler(ParentWindow_Closed);
			mWindow = window;
		}

		public MUCChatArea HostedArea
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
		}

		private ChatWindow mWindow;
		private TextBlock mHeaderString;
		private MUCChatArea mArea;
		private IChatRoom mRoom;
	}
}