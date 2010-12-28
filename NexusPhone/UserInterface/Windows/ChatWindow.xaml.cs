using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace NexusPhone.UserInterface
{
	partial class ChatWindow : PhoneApplicationPage
	{
		public ChatWindow()
		{
			InitializeComponent();
		}

		public IMBuddy Contact
		{
			get {
				return mContact;
			}
			set	{
				mContact = value;

				DataContext = mContact;
				mContact.Dispatcher = Dispatcher;
			}
		}

		private IMBuddy mContact;

		// Methods
		private void SendMessage()
		{
			string message = ChatMessage.Text;

			if (String.IsNullOrEmpty(message))
				return;

			ChatMessage.Text = String.Empty;

			IMMessage msg = new IMMessage(mContact, message);
			
			mContact.SendMessage(msg);
		}

		// Event Handlers
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			SendMessage();
		}
		private void ChatMessage_KeyUp(object sender, KeyEventArgs e)
		{
			string message = ChatMessage.Text;

			if (String.IsNullOrEmpty(message))
			{
				MessageHelpText.Visibility = Visibility.Visible;
				return;
			}

			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				SendMessage();
				MessageHelpText.Visibility = Visibility.Visible;
			} else
				MessageHelpText.Visibility = Visibility.Collapsed;
		}
		private void PhoneNumber_BeginDial(object sender, MouseButtonEventArgs e)
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = "15555550101";
			call.Show();
		}
	}
}