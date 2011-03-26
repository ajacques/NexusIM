using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using InstantMessage.Protocols;
using System.Windows.Input;
using InstantMessage.Events;
using System.Windows.Documents;
using System.Windows.Media;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for MUCChatArea.xaml
	/// </summary>
	public partial class MUCChatArea : UserControl
	{
		public MUCChatArea()
		{
			InitializeComponent();
		}

		public void PopulateUIControls(IChatRoom chatRoom)
		{
			mChatRoom = chatRoom;

			mChatRoom.OnMessageReceived += new EventHandler<IMMessageEventArgs>(mChatRoom_OnMessageReceived);
		}

		public void ProcessChatMessage(IMMessageEventArgs e)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = e.Sender.Username;
				inline.UsernameColor = Color.FromRgb(0, 0, 255);
				inline.MessageBody = e.Message;

				if (ChatHistoryBox.Inlines.Any())
					ChatHistoryBox.Inlines.Add(new LineBreak());

				ChatHistoryBox.Inlines.Add(inline);
				ChatHistoryContainer.ScrollToEnd();
			}));
		}

		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;

				string message = MessageBody.Text;
				MessageBody.Text = String.Empty;

				mChatRoom.SendMessage(message);
			}
		}
		private void mChatRoom_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			ProcessChatMessage(e);
		}

		private IChatRoom mChatRoom;
	}
}