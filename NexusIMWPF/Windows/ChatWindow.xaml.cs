using System;
using System.Windows;
using System.Windows.Input;
using InstantMessage;
using NexusIM.Controls;
using InstantMessage.Events;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatWindow()
		{
			this.InitializeComponent();
			this.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
			// Insert code required on object creation below this point.
		}

		private IMBuddy Contact
		{
			get {
				return DataContext as IMBuddy;
			}
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Contact.onReceiveMessage += new EventHandler<IMMessageEventArgs>(OnReceiveMessage);
		}
		private void OnReceiveMessage(object sender, IMMessageEventArgs e)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() => {
				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = e.Sender.Username;
				inline.UsernameColor = Color.FromRgb(0, 0, 255);
				inline.MessageBody = e.Message;
				ChatHistoryBox.Inlines.Add(inline);
			}));
		}
		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;

				string message = MessageBody.Text;
				MessageBody.Text = String.Empty;

				Contact.SendMessage(message);

				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = "Me";
				inline.MessageBody = message;
				ChatHistoryBox.Inlines.Add(inline);
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Contact.Status == IMBuddyStatus.Offline)
			{
				Storyboard fadeIn = FindResource("UserOfflineWarningFade") as Storyboard;
				fadeIn.Begin();
			}
		}
	}
}