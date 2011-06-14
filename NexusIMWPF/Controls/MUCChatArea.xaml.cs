using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using InstantMessage.Events;
using InstantMessage.Protocols;
using NexusIM.Protocol;
using InstantMessage;
using System.Collections.Generic;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for MUCChatArea.xaml
	/// </summary>
	public partial class MUCChatArea : UserControl, ITabbedArea
	{
		public MUCChatArea()
		{
			InitializeComponent();
		}

		internal void PopulateUIControls(IChatRoom room, GroupChatAreaHost host)
		{
			mChatRoom = room;
			mProtocol = room.Protocol;

			mChatRoom.OnMessageReceived += new EventHandler<IMMessageEventArgs>(mChatRoom_OnMessageReceived);
			mChatRoom.OnUserListReceived += new EventHandler(mChatRoom_OnUserListReceived);
		}

		private void Host_TabClosed(object sender, EventArgs e)
		{
			ChatRoom.Leave(String.Empty);
		}

		public void ProcessChatMessage(IMMessageEventArgs e)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = e.Sender.Nickname;
				inline.UsernameColor = Color.FromRgb(0, 0, 255);
				inline.MessageBody = e.Message;

				if (ChatHistoryBox.Inlines.Any())
					ChatHistoryBox.Inlines.Add(new LineBreak());

				ChatHistoryBox.Inlines.Add(inline);
				ChatHistoryContainer.ScrollToEnd();
			}));
		}

		private void ProcessUserList()
		{
			IEnumerator<string> partEnumer =mChatRoom.Participants.GetEnumerator();

			Dispatcher.BeginInvoke(new GenericEvent(() => {
				while (partEnumer.MoveNext())
				{
					OccupantList.Items.Add(partEnumer.Current);
				}
			}));
		}

		// User Interface Event Handlers
		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.RightCtrl))
			{
				e.Handled = true;

				string message = MessageBody.Text;
				MessageBody.Text = String.Empty;

				mChatRoom.SendMessage(message);

				ProcessChatMessage(new IMMessageEventArgs(new SelfContact(mProtocol), message));
			}
		}

		// Chat Room Event Handlers
		private void mChatRoom_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			ProcessChatMessage(e);
		}
		private void mChatRoom_OnUserListReceived(object sender, EventArgs e)
		{
			ProcessUserList();
		}

		public IChatRoom ChatRoom
		{
			get	{
				return mChatRoom;
			}
		}

		private IChatRoom mChatRoom;
		private IMProtocol mProtocol;

		#region ITabbedArea Members

		public void PopulateUIControls(object context)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}