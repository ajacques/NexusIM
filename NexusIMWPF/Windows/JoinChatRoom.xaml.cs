using System.Windows;
using System.Windows.Input;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Controls;
using NexusIM.Managers;
using System.Linq;
using InstantMessage.Protocols.Irc;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for JoinChatRoom.xaml
	/// </summary>
	public partial class JoinChatRoom : Window
	{
		public JoinChatRoom()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
			AccountSelector.QueryClause = (extraData) => extraData.Enabled && extraData.Protocol is IHasMUCRooms && extraData.Protocol.ProtocolStatus == IMProtocolStatus.Online;

			if (AccountManager.Accounts.Any(AccountSelector.QueryClause))
			{
				NoAccountsMsg.Visibility = Visibility.Collapsed;
				MainGrid.Visibility = Visibility.Visible;
			} else {
				NoAccountsMsg.Visibility = Visibility.Visible;
				MainGrid.Visibility = Visibility.Collapsed;
			}

			mRooms = new LinkedList<RoomInfo>();
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			base.OnPreviewKeyUp(e);

			if (e.Key == Key.Enter)
				Button_Click(null, null);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolWrapper extraData = AccountSelector.SelectedProtocol;

			IHasMUCRooms muc = (IHasMUCRooms)extraData.Protocol;
			muc.JoinChatRoom(RoomName.Text);

			this.Close();
		}
		private void RoomName_KeyUp(object sender, KeyEventArgs e)
		{
			if (RoomName.Text.Length >= 1)
			{
				JoinButton.IsEnabled = true;

				if (RoomName.Text.Length >= 4)
				{
					IMProtocolWrapper wrapper = AccountSelector.SelectedProtocol;
					if (wrapper.Protocol is IRCProtocol)
					{
						IRCProtocol protocol = (IRCProtocol)wrapper.Protocol;
						mRooms.Clear();

						protocol.QueryServer(String.Format("LIST {0}*", RoomName.Text), 322, 323, new IRCProtocol.ServerResponse(OnRoomSearchResult));
					}
				}
			} else
				JoinButton.IsEnabled = false;
		}

		private void OnRoomSearchResult(int numeric, string line)
		{
			if (mRooms.Count >= 4 || numeric == 323)
			{
				// Draw the suggestions
				Dispatcher.InvokeIfRequired(() =>
				{
					SuggestResults.Children.Clear();
					bool isFirst = true;
					foreach (RoomInfo info in mRooms)
					{
						Grid outer = new Grid();
						Grid container = new Grid();
						TextBlock textblock = new TextBlock();
						TextBlock topicBlock = new TextBlock();

						container.Cursor = Cursors.Hand;
						container.ToolTip = "Join " + info.RoomName;
						container.MouseUp += new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) => { RoomName.Text = info.RoomName; Button_Click(sender, args); });

						container.RowDefinitions.Add(new RowDefinition());
						container.RowDefinitions.Add(new RowDefinition());

						container.Children.Add(textblock);
						container.Children.Add(topicBlock);

						textblock.Text = info.RoomName;
						topicBlock.Text = info.Topic;

						Grid.SetRow(topicBlock, 1);

						if (!isFirst)
						{
							Line sep = new Line();
							sep.X2 = 260;
							sep.VerticalAlignment = VerticalAlignment.Bottom;
							sep.Stroke = (Brush)FindResource("SuggestionSeparator");

							outer.Children.Add(sep);
							isFirst = false;
						}

						outer.Children.Add(container);

						SuggestResults.Children.Add(outer);
					}
				});
			}

			if (numeric == 322)
			{
				string[] parts = line.Split(new char[] { ' ' }, 4);

				mRooms.AddLast(new RoomInfo(parts[0], Int32.Parse(parts[1]), parts[3]));
			}
		}

		// Nested Classes
		private sealed class RoomInfo
		{
			public RoomInfo(string roomName, int userCount, string topic)
			{
				RoomName = roomName;
				UserCount = userCount;
				Topic = topic;
			}

			public string RoomName
			{
				get;
				private set;
			}
			public int UserCount
			{
				get;
				private set;
			}
			public string Topic
			{
				get;
				private set;
			}
		}

		// Variables
		private LinkedList<RoomInfo> mRooms;
	}
}