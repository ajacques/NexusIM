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
using System.Windows;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for MUCChatArea.xaml
	/// </summary>
	public partial class MUCChatArea : UserControl, IDisposable
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
				IRCChannel ircChannel = (IRCChannel)room;
				ircProtocol.OnNoticeReceive += new EventHandler<IMChatRoomGenericEventArgs>(IrcProtocol_OnNoticeReceive);
				ircChannel.OnKickedFromChannel += new EventHandler<IMChatRoomGenericEventArgs>(IrcChannel_OnKicked);
				ircChannel.TopicChanged += new EventHandler<IMChatRoomGenericEventArgs>(ircChannel_TopicChanged);

				RoomDescription.Text = ircChannel.Topic;

				mUserContextMenu = new IrcChanUserContextMenu();
			}

			RoomName.Text = mChatRoom.Name;
		}

		public void Dispose()
		{
			// Clean up all event handlers
			mChatRoom.OnMessageReceived -= new EventHandler<IMMessageEventArgs>(ChatRoom_OnMessageReceived);
			mChatRoom.OnUserListReceived -= new EventHandler(ChatRoom_OnUserListReceived);
			mChatRoom.OnUserJoin -= new EventHandler<IMChatRoomGenericEventArgs>(ChatRoom_OnUserJoin);

			if (mProtocol is IRCProtocol)
			{
				IRCProtocol ircProtocol = (IRCProtocol)mProtocol;
				IRCChannel ircChannel = (IRCChannel)mChatRoom;
				ircProtocol.OnNoticeReceive -= new EventHandler<IMChatRoomGenericEventArgs>(IrcProtocol_OnNoticeReceive);
				ircChannel.OnKickedFromChannel -= new EventHandler<IMChatRoomGenericEventArgs>(IrcChannel_OnKicked);
			}
		}

		public void ProcessChatMessage(IMMessageEventArgs e)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = e.Sender.Nickname;
				inline.UsernameColor = Color.FromRgb(0, 0, 255);
				inline.MessageBody = e.Message;

				AppendChatInline(inline);
			}));
		}

		private void ProcessUserList()
		{
			using (IEnumerator<string> partEnumer = mChatRoom.Participants.GetEnumerator())
			{
				Dispatcher.BeginInvoke(new GenericEvent(() => {
					OccupantList.Items.Clear();
					int ct = 0;
					while (partEnumer.MoveNext())
					{
						string username = partEnumer.Current;
						TextBlock user = new TextBlock();
						user.Text = username;
						user.ContextMenu = mUserContextMenu;

						OccupantList.Items.Add(user);
						ct++;
					}
					OccupantCount.Text = ct.ToString();
				}));
			}
		}
		private void AppendChatInline(Inline inline)
		{
			if (ChatHistoryBox.Inlines.Any())
				ChatHistoryBox.Inlines.Add(new LineBreak());

			ChatHistoryBox.Inlines.Add(inline);
			ChatHistoryContainer.ScrollToEnd();
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
		private void Hyperlink_MouseEnter(object sender, MouseEventArgs e)
		{
			Hyperlink hyperlink = (Hyperlink)sender;
			if (hyperlink.IsMouseOver)
				hyperlink.TextDecorations = TextDecorations.Underline;
			else
				hyperlink.TextDecorations = null;
		}
		private void RejoinLink_Click(object sender, RoutedEventArgs e)
		{
			ChatHistoryBox.Inlines.Remove(ChatHistoryBox.Inlines.LastInline); // Remove the message

			if (!ChatRoom.Joined)
				((IHasMUCRooms)mProtocol).JoinChatRoom(ChatRoom);
		}
		private void OccupantList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (OccupantList.SelectedItem != null)
			{
				mUserContextMenu.PopulateMenu(OccupantList.SelectedItem.ToString());
			}
		}
		private void OccupantList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (OccupantList.SelectedItem != null)
			{
				mUserContextMenu.PopulateMenu(OccupantList.SelectedItem.ToString());
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
				Run user = new Run(e.Username.Nickname);
				Run message = new Run(" has entered the room.");

				span.FontStyle = FontStyles.Italic;
				span.Foreground = Brushes.Green;

				span.Inlines.Add(user);
				span.Inlines.Add(message);
				AppendChatInline(span);
			});
		}
		private void IrcChannel_OnKicked(object sender, IMChatRoomGenericEventArgs e)
		{
			// Build and display a block of text to show the user who kicked them and why
			// And allow them to rejoin
			Dispatcher.InvokeIfRequired(() => {
				Span span = new Span();
				Run messageRun = new Run();
				messageRun.Text = String.Format("You have been kicked by {0} (Reason: {1})", e.Username.Nickname, e.Message);
				messageRun.Foreground = Brushes.Red;

				span.Inlines.Add(messageRun);
				span.Inlines.Add(new LineBreak());

				Hyperlink rejoinLink = new Hyperlink(new Run("Rejoin"));
				span.Inlines.Add(rejoinLink);
				span.Inlines.Add(new Run("   ")); // Spacer
				
				rejoinLink.TextDecorations = null;
				rejoinLink.BaselineAlignment = BaselineAlignment.Top; // Align the text with the checkbox - It looks weird when it's offset
				rejoinLink.MouseEnter += new MouseEventHandler(Hyperlink_MouseEnter);
				rejoinLink.MouseLeave += new MouseEventHandler(Hyperlink_MouseEnter);
				rejoinLink.Click += new RoutedEventHandler(RejoinLink_Click);

				InlineUIContainer container = new InlineUIContainer();
				span.Inlines.Add(container);

				CheckBox autoRejoin = new CheckBox();
				autoRejoin.Content = new TextBlock() { Text = "Automatically rejoin"};
				container.Child = autoRejoin;

				AppendChatInline(span);
			});
		}
		private void ircChannel_TopicChanged(object sender, IMChatRoomGenericEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() =>
				{
					RoomDescription.Text = e.Message;
				});
		}
		
		// Protocol Event Handlers
		private void IrcProtocol_OnNoticeReceive(object sender, IMChatRoomGenericEventArgs e)
		{
			IRCProtocol ircProtocol = (IRCProtocol)sender;
			
			Dispatcher.InvokeIfRequired(() => {
				Span span = new Span();
				Run notice = new Run();
				notice.Text = e.Message;

				span.Foreground = Brushes.Gray;

				span.Inlines.Add(notice);

				AppendChatInline(span);
			});
		}

		public IChatRoom ChatRoom
		{
			get	{
				return mChatRoom;
			}
		}

		private IrcChanUserContextMenu mUserContextMenu;
		private IChatRoom mChatRoom;
		private IMProtocol mProtocol;
	}
}