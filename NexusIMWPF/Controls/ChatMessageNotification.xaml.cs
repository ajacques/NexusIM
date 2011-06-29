using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using InstantMessage.Events;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ChatMessageNotification.xaml
	/// </summary>
	public partial class ChatMessageNotification : UserControl
	{
		public ChatMessageNotification()
		{
			this.InitializeComponent();
		}

		public void PopulateUI(IContact contact, string message)
		{
			mContact = contact;
			Dispatcher.InvokeIfRequired(new StringInvoke(PopulateUIImpl), args: message);
		}

		private void PopulateUIImpl(string message)
		{
			DisplayName.Text = mContact.Nickname;
			Username.Text = mContact.Username;
			MessageBody.Text = message;
		}

		private delegate void StringInvoke(string message);

		private IContact mContact;
	}
}