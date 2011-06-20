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
using NexusIM.Misc;
using InstantMessage.Protocols.Irc;

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

			mChatRoom.OnMessageReceived += new EventHandler<IMMessageEventArgs>(ChatRoom_OnMessageReceived);
			mChatRoom.OnUserListReceived += new EventHandler(ChatRoom_OnUserListReceived);
			mChatRoom.OnUserJoin += new EventHandler<IMChatRoomGenericEventArgs>(ChatRoom_OnUserJoin);

			if (mProtocol is IRCProtocol)
			{
				IRCProtocol ircProtocol = (IRCProtocol)mProtocol;
				ircProtocol.OnNoticeReceive += new EventHandler<IMChatRoomGenericEventArgs>(IrcProtocol_OnNoticeReceive);
				mUserContextMenu = new IrcChanUserContextMenu();
			}
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
			IEnumerator<string> partEnumer = mChatRoom.Participants.GetEnumerator();

			Dispatcher.BeginInvoke(new GenericEvent(() => {
				while (partEnumer.MoveNext())
				{
					string username = partEnumer.Current;
					TextBlock user = new TextBlock();
					user.Text = username;
					user.ContextMenu = mUserContextMenu;

					OccupantList.Items.Add(user);
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

				if (message[0] == '/')
				{
					return;
				}

				mChatRoom.SendMessage(message);

				ProcessChatMessage(new IMMessageEventArgs(new SelfContact(mProtocol), message));
			}
		}

		// Chat Room Event Handlers
		private void ChatRoom_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			ProcessChatMessage(e);
		}
		private void ChatRoom_OnUserListReceived(object sender, EventArgs e)
		{
			ProcessUserList();
		}
		private void ChatRoom_OnUserJoin(object sender, IMChatRoomGenericEventArgs e)
		{
			ChatHistoryBox.Dispatcher.InvokeIfRequired(() => {
				Span span = new Span();
				Run user = new Run(e.Username);
				Run message = new Run(" has entered the room.");

				span.Inlines.Add(user);
				span.Inlines.Add(message);
				ChatHistoryBox.Inlines.Add(new LineBreak());
				ChatHistoryBox.Inlines.Add(span);
			});
		}

		private void IrcProtocol_OnNoticeReceive(object sender, IMChatRoomGenericEventArgs e)
		{
			IRCProtocol ircProtocol = (IRCProtocol)sender;
			
			Dispatcher.InvokeIfRequired(() => {
				Span span = new Span();
				span.Inlines.Add(new LineBreak());
				Run notice = new Run();
				notice.Text = e.Message;

				span.Inlines.Add(notice);

				ChatHistoryBox.Inlines.Add(span);
			});
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

		private IrcChanUserContextMenu mUserContextMenu;
	}
}