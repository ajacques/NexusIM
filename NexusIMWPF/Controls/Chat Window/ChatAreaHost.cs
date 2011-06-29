using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Windows;
using System.Windows.Input;

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
		public ContactChatAreaHost(IContact context) : this()
		{
			ContactChatArea area = new ContactChatArea();
			area.Contact = context;
			mHeaderString.Text = area.Contact.Username;

			Content = mArea = area;
		}

		public override string ToString()
		{
			return "testusername";
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			HostWindow.HandleTabClose(this);
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			base.OnGiveFeedback(e);

			if (e.Effects.HasFlag(DragDropEffects.Copy))
				Mouse.SetCursor(Cursors.Cross);
			else if (e.Effects.HasFlag(DragDropEffects.Move))
				Mouse.SetCursor(Cursors.Pen);
			else
				Mouse.SetCursor(Cursors.No);
			e.Handled = true;
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				//DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
			}
		}

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