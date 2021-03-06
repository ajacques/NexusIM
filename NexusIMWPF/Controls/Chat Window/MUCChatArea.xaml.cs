﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using InstantMessage.Events;
using InstantMessage.Protocols;
using InstantMessage.Protocols.Irc;
using NexusIM.Controls.Inlines;
using NexusIM.Protocol;
using NexusIM.Windows;
using NexusIM.Managers;
using System.Globalization;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for MUCChatArea.xaml
	/// </summary>
	public sealed partial class MUCChatArea : UserControl, IDisposable
	{
		public MUCChatArea()
		{
			InitializeComponent();

			MessageBody.MessageSend += new EventHandler<SendMessageEventArgs>(MessageBody_MessageSend);
		}
		public MUCChatArea(IChatRoom chatRoom) : this()
		{
			PopulateUIControls(chatRoom);
		}

		internal void PopulateUIControls(IChatRoom room)
		{
			mChatRoom = room;
			mProtocol = room.Protocol;

			mChatRoom.OnMessageReceived += new EventHandler<IMMessageEventArgs>(ChatRoom_OnMessageReceived);
			mChatRoom.OnUserListReceived += new EventHandler(ChatRoom_OnUserListReceived);
			mChatRoom.OnUserJoin += new EventHandler<IMChatRoomGenericEventArgs>(ChatRoom_OnUserJoin);

			mProtocol.onDisconnect += new EventHandler<IMDisconnectEventArgs>(Protocol_OnDisconnect);
			mProtocol.LoginCompleted += new EventHandler(Protocol_LoginCompleted);

			if (mProtocol is IRCProtocol)
			{
				IRCProtocol ircProtocol = (IRCProtocol)mProtocol;
				IRCChannel ircChannel = (IRCChannel)room;
				ircProtocol.OnNoticeReceive += new EventHandler<IMChatRoomGenericEventArgs>(IrcProtocol_OnNoticeReceive);
				ircChannel.OnKickedFromChannel += new EventHandler<IMChatRoomGenericEventArgs>(IrcChannel_OnKicked);
				ircChannel.TopicChanged += new EventHandler<IMChatRoomGenericEventArgs>(IrcChannel_TopicChanged);
				ircChannel.OnModeChange += new EventHandler<IRCModeChangeEventArgs>(IrcChannel_OnModeChange);
				ircChannel.OnUserPart += new EventHandler<IMChatRoomGenericEventArgs>(IrcChannel_OnUserPart);

				RoomDescription.Text = ircChannel.Topic;
				mNickSearch = new Regex(String.Format(CultureInfo.InvariantCulture, @"(?:^|\ ){0}(?:$|\ |')", ircProtocol.Nickname));

				mUserContextMenu = new IrcChanUserContextMenu();
			} else
				mNickSearch = new Regex(String.Format(CultureInfo.InvariantCulture, @"(?:^|\ ){0}(?:$|\ |')", mProtocol.Username));
			RoomName.Text = mChatRoom.Name;

			UpdateParentWindowData();
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
		public override string ToString()
		{
			if (ChatRoom == null)
				return base.ToString();

			return ChatRoom.Name;
		}

		/// <summary>
		/// Converts an HSV color to the RGB color model.
		/// </summary>
		/// <param name="hue"></param>
		private Color HsvToRgb(double hue, double saturation, double value)
		{
			if (saturation == 0)
			{
				byte v = (byte)Math.Round(value * 255);
				return Color.FromRgb(v, v, v);
			}

			double r, g, b;
			double i;
			double f, p, q, t;

			hue = hue / 60;
			i = Math.Floor(hue);
			f = hue - i;
			p = value * (1 - saturation);
			q = value * (1 - saturation * f);
			t = value * (1 - saturation * (1 - f));

			switch ((int)i)
			{
				case 0:
					r = value;
					g = t;
					b = p;
					break;
				case 1:
					r = q;
					g = value;
					b = p;
					break;
				case 2:
					r = p;
					g = value;
					b = t;
					break;
				case 3:
					r = p;
					g = q;
					b = value;
					break;
				case 4:
					r = t;
					g = p;
					b = value;
					break;
				default:
					r = value;
					g = p;
					b = q;
					break;
			}

			byte br = (byte)(r * 255);
			byte bg	= (byte)(g * 255);
			byte bb = (byte)(b * 255);

			return Color.FromRgb(br, bg, bb);
		}

		private Color ComputeUserColor(string username)
		{
			int hash = Math.Abs(username.GetHashCode());
			hash = hash % 3504; // 1009
			hash = hash % 360;

			return HsvToRgb(hash, 83, 76);
		}

		public void ProcessChatMessage(IMMessageEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() =>
			{
				Inline result = IMMessageProcessor.ProcessMessage(e.Message, hyperlinkMouseEnter: new MouseEventHandler(ChatHistory.IMHyperlink_MouseEnter), hyperlinkMouseLeave: new MouseEventHandler(ChatHistory.IMHyperlink_MouseLeave), hyperlinkMouseClick: new MouseButtonEventHandler(ChatHistory.IMHyperlink_MouseClick));

				if (e.Flags == MessageFlags.None)
				{
					ChatMessageInline inline = new ChatMessageInline();
					inline.Username = e.Sender.Nickname;
					if (e.Sender is SelfContact)
						inline.UsernameColor = Color.FromRgb(255, 100, 0);
					else
						inline.UsernameColor = ComputeUserColor(e.Sender.Username); //Color.FromRgb(0, 0, 145);
					inline.Inlines.Add(result);

					AppendChatInline(inline);
				} else if (e.Flags == MessageFlags.UserAction) {
					UserActionInline inline = new UserActionInline();
					inline.Username = e.Sender.Nickname;
					if (e.Sender is SelfContact)
						inline.UsernameColor = Color.FromRgb(255, 100, 0);
					else
						inline.UsernameColor = Color.FromRgb(0, 0, 145);
					inline.Inlines.Add(result);

					AppendChatInline(inline);
				}
			});

			if (!(e.Sender is SelfContact) && mNickSearch.IsMatch(e.Message))
				Win32.FlashWindow(mWindowPointer);
		}

		private void ProcessUserList()
		{
			using (IEnumerator<IContact> partEnumer = mChatRoom.Participants.GetEnumerator())
			{
				Dispatcher.BeginInvoke(new GenericEvent(() => {
					OccupantList.Items.Clear();
					int ct = 0;
					while (partEnumer.MoveNext())
					{
						AddUser(partEnumer.Current);
						ct++;
					}
					OccupantCount.Text = ct.ToString();
				}));
			}
		}
		private void AppendChatInline(Inline inline)
		{
			//bool autoScroll = false;
			//if (1 - (HistoryScroller.VerticalOffset / HistoryScroller.ViewportHeight) > 0.2)
			//	autoScroll = true;
			
			ChatHistory.AppendInline(inline);

			HistoryScroller.ScrollToBottom();
		}
		private bool ProcessSendMessage(string message)
		{
			MessageFlags flags = MessageFlags.None;

			if (message[0] == '/')
			{
				int spaceindex = message.IndexOf(' ');
				if (spaceindex >= 2)
				{
					string cmd = message.Substring(1, spaceindex - 1).ToLowerInvariant();
					if (mChatRoom is IRCChannel)
					{
						IRCChannel channel = (IRCChannel)mChatRoom;
						IRCProtocol protocol = (IRCProtocol)mProtocol;
						switch (cmd)
						{
							case "me":
								if (message.Length < 4)
									break;
								flags = MessageFlags.UserAction;
								message = message.Substring(4);
								break;
							case "mode":
								{
									// Possible options:
									// /mode +o Penguin
									// /mode +b Penguin!*@*
									// /mode Penguin +X

									string[] chunks = message.Split(new char[] { ' ' }, 2);

									if (chunks[1][0] == '+' || chunks[1][0] == '-')
										channel.ApplyUserMode(chunks[1]);

									return true;
								}
							case "kick":
								{
									// Possible options:
									// /kick Penguin die
									// /kick Penguin
									// /kick Penguin go away

									string[] chunks = message.Split(new char[] { ' ' }, 3);

									if (chunks.Length < 2)
									{
										Run errorRun = new Run("Not enough parameters (Format: /kick nickname [reason])");
										errorRun.Foreground = Brushes.Red;
										AppendChatInline(errorRun);
										return false;
									} else if (chunks.Length == 3)
										channel.KickUser(chunks[1], chunks[2]);
									else
										channel.KickUser(chunks[1]);

									return true;
								}
							case "oper":
								{
									string[] chunks = message.Split(new char[] { ' ' }, 3);

									if (chunks.Length < 3)
									{
										Run errorRun = new Run("Not enough parameters (Format: /oper username password)");
										errorRun.Foreground = Brushes.Red;
										AppendChatInline(errorRun);
										return false;
									}

									protocol.LoginAsOperator(chunks[1], chunks[2], IrcProtocol_LoginAsOperatorResult);

									return true;
								}
							default:
								{
									protocol.SendRawMessage(message.Substring(1));

									return true;
								}
						}
					}
				} else {
					switch (message)
					{
						case "clear":
							ChatHistory.Clear();
							return true;
					}
				}
			}
			
			mChatRoom.SendMessage(message, flags);

			ProcessChatMessage(new IMMessageEventArgs(new SelfContact(mProtocol), message, flags));
			return true;
		}
		private void HideAllOverlays()
		{
			OverlayRect.Visibility = Visibility.Collapsed;

			// Close any other popups
			InvitePopup.Visibility = Visibility.Collapsed;
		}
		private void AddUser(IContact contact)
		{
			TextBlock user = new TextBlock();
			user.Text = contact.Nickname;
			user.ContextMenu = mUserContextMenu;

			OccupantList.Items.Add(user);
		}
		private void UpdateParentWindowData()
		{
			if (mWindow == null)
				return;

			mWindow.UpdateWindowTitle();
		}

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);

			mWindow = (ChatWindow)Window.GetWindow(this);
			mWindowPointer = new WindowInteropHelper(mWindow).Handle;
			MessageBody.Focus();

			mWindow.Closed += new EventHandler(Window_Closed);

			UpdateParentWindowData();
		}

		// User Interface Event Handlers
		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				e.Handled = true;
				string lastword = MessageBody.Text.Substring(MessageBody.Text.LastIndexOf(' ') + 1);
				IEnumerable<IContact> matches = mChatRoom.Participants.Where(c => c.Nickname.StartsWith(lastword));
				if (matches.Count() == 1)
				{
					string match = matches.First().Nickname;
					MessageBody.Text += match.Substring(lastword.Length);
					MessageBody.CaretIndex = MessageBody.Text.Length;
				}
			}
		}
		private void MessageBody_MessageSend(object sender, SendMessageEventArgs e)
		{
			ProcessSendMessage(e.Message);
			HistoryScroller.ScrollToBottom();
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
			ChatHistory.RemoveLast(); // Remove the message

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
		private void ChatHistoryContainer_SelectionChanged(object sender, RoutedEventArgs e)
		{
			string selection = MessageBody.SelectedText;

			MessageBody.Select(0, 0);

			Clipboard.SetText(selection);
		}
		private void RoomOptions_MouseDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement element = (FrameworkElement)sender;
			element.ContextMenu.IsOpen = true;
		}
		private void InviteUser_Click(object sender, RoutedEventArgs e)
		{
			Storyboard animIn = (Storyboard)FindResource("InviteUserOpen");
			
			EventHandler handler = null;
			handler = (f, g) => {
				InviteUsername.Focus();
				animIn.Completed -= handler;
				handler = null;
			};
			animIn.Completed += handler;

			animIn.Begin();
		}
		private void InviteUserButton_Click(object sender, RoutedEventArgs e)
		{
			string username = InviteUsername.Text;
			InviteUsername.Text = String.Empty;
			string message = InviteMessage.Text;
			InviteMessage.Text = String.Empty;

			HideAllOverlays();

			ChatRoom.InviteUser(username, message);
		}
		private void OverlayRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			HideAllOverlays();
		}
		private void Window_Closed(object sender, EventArgs e)
		{
			ChatRoom.Leave(null);
		}

		// Chat Room Event Handlers
		private void ChatRoom_OnMessageReceived(object sender, IMMessageEventArgs e)
		{
			mWindow.IncrementUnread();
			ProcessChatMessage(e);
		}
		private void ChatRoom_OnUserListReceived(object sender, EventArgs e)
		{
			ProcessUserList();
		}
		private void ChatRoom_OnUserJoin(object sender, IMChatRoomGenericEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() => {
				Span span = new TimestampedInline();
				Run user = new Run(e.Username.Nickname);
				Run message = new Run(" has entered the room.");

				span.FontStyle = FontStyles.Italic;
				span.Foreground = Brushes.Green;

				span.Inlines.Add(user);
				span.Inlines.Add(message);
				AppendChatInline(span);

				AddUser(e.Username);
			});
		}
		private void IrcChannel_OnUserPart(object sender, IMChatRoomGenericEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() =>
			{
				Span span = new TimestampedInline();
				Run user = new Run(e.Username.Nickname);
				Run message = new Run();
				if (String.IsNullOrEmpty(e.Message))
					message.Text = " has left the room.";
				else
					message.Text = String.Format(" has left the room. [{0}]", e.Message);

				span.FontStyle = FontStyles.Italic;
				span.Foreground = Brushes.IndianRed;

				span.Inlines.Add(user);
				span.Inlines.Add(message);
				AppendChatInline(span);

				OccupantList.Items.Remove(e.Username);
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

				OccupantList.Items.Remove(e.Username);
			});
		}
		private void IrcChannel_TopicChanged(object sender, IMChatRoomGenericEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() =>
				{
					RoomDescription.Text = e.Message;
				});
		}
		private void IrcChannel_OnModeChange(object sender, IRCModeChangeEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() =>
			{
				Span span = new TimestampedInline();
				span.Inlines.Add(new Run(e.Username.Nickname));

				span.Foreground = Brushes.Green;
				span.Inlines.Add(new Run(" sets mode: "));
				span.Inlines.Add(new Run(e.RawMode));

				ChatHistory.AppendInline(span);
			});
		}
		private void IrcProtocol_LoginAsOperatorResult(bool success, string message)
		{
			Dispatcher.InvokeIfRequired(() => {
				if (success)
				{
					Run msg = new Run(String.Format("Login succeeded (Message: {0})", message));
					msg.Foreground = Brushes.Green;

					AppendChatInline(msg);
				} else {
					Run msg = new Run(String.Format("Login failed (Message: {0})", message));
					msg.Foreground = Brushes.Red;

					AppendChatInline(msg);
				}
			});
		}
		private void Protocol_OnDisconnect(object sender, IMDisconnectEventArgs e)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() => ChatHistory.AppendInline(new AccountDisconnectedInline())));
		}
		private void Protocol_LoginCompleted(object sender, EventArgs e)
		{
			IHasMUCRooms iface = (IHasMUCRooms)mProtocol;
			iface.JoinChatRoom(mChatRoom);
		}
		
		// Protocol Event Handlers
		private void IrcProtocol_OnNoticeReceive(object sender, IMChatRoomGenericEventArgs e)
		{
			IRCProtocol ircProtocol = (IRCProtocol)sender;
			
			Dispatcher.InvokeIfRequired(() => {
				Span span = new TimestampedInline();
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

		private ChatWindow mWindow;
		private IntPtr mWindowPointer;
		private IrcChanUserContextMenu mUserContextMenu;
		private IChatRoom mChatRoom;
		private IMProtocol mProtocol;
		private Regex mNickSearch;
	}
}