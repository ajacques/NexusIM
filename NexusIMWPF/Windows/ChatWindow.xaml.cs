using System;
using System.Windows;
using System.Windows.Input;
using InstantMessage;

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
			
			// Insert code required on object creation below this point.
		}

		private IMBuddy Contact
		{
			get {
				return DataContext as IMBuddy;
			}
		}

		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;

				string message = MessageBody.Text;
				MessageBody.Text = String.Empty;

				Contact.SendMessage(message);
			}
		}
	}
}