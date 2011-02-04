﻿using System;
using System.Windows;
using System.Windows.Input;
using InstantMessage;
using NexusIM.Controls;
using InstantMessage.Events;
using System.Windows.Media;
using System.Windows.Media.Animation;
using NexusIM.Managers;
using System.ComponentModel;

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
			Contact.PropertyChanged += new PropertyChangedEventHandler(Contact_PropertyChanged);
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
				MessageLogger.LogMessageToRemote(Contact, message);
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Contact.Status == IMBuddyStatus.Offline)
				UserOfflineWarning.Visibility = Visibility.Visible;

			if (Contact.Protocol.Status == IMStatus.Invisible)
				SelfInvisibleWarning.Visibility = Visibility.Visible;
		}
		private void AppearOnline_Click(object sender, RoutedEventArgs e) {}
		private void AppearOnlineToAll_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Status = IMStatus.Available;
			SelfInvisibleWarning.Visibility = Visibility.Collapsed;
		}
		private void Contact_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Avatar")
			{

			}
		}
	}
}