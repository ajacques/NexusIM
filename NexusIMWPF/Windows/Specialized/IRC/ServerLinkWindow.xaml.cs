using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using InstantMessage.Protocols.Irc;
using System.ComponentModel;
using NexusIM.Controls;
using System.Diagnostics;
using InstantMessage.Events;

namespace NexusIM.Windows.IRC
{
	/// <summary>
	/// Interaction logic for ServerLinkWindow.xaml
	/// </summary>
	public partial class ServerLinkWindow : Window
	{
		public ServerLinkWindow()
		{
			InitializeComponent();

			mServers = new SortedDictionary<string, ServerInfo>();
		}

		public void LoadData(IRCProtocol protocol)
		{
			LinksList.Items.Clear();
			mServers.Clear();

			mProtocol = protocol;

			LinksList.ContextMenu = new LinkContextMenu(mProtocol, this);

			protocol.QueryServer("STATS C", new int[] { 213, 244 }, 219, new IRCProtocol.ServerResponse(OnResponse));
			protocol.OnNoticeReceive += new EventHandler<IMChatRoomGenericEventArgs>(protocol_OnNoticeReceive);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Placeholder.SetText(ConnectHostname, "Hostname");
			Placeholder.SetText(ConnectPort, "Port");
		}

		// Private Methods
		private void RefreshStatus()
		{
			mProtocol.QueryServer("STATS X", 247, 219, new IRCProtocol.ServerResponse(OnUnlinkResponse));
		}
		private void OnUnlinkResponse(int numeric, string data)
		{
			switch (numeric)
			{
				case 247:
					{
						string[] sData = data.Split(' ');

						ServerInfo info = mServers[sData[1]];
						info.Status = "Down";

						break;
					}
				case 219:
					mProtocol.QueryServer("STATS l", 211, 219, new IRCProtocol.ServerResponse(OnLinkResponse));
					break;
			}
		}
		private void OnLinkResponse(int numeric, string data)
		{
			switch (numeric)
			{
				case 211:
					{
						string[] sData = data.Split(' ');

						if (sData[0].IndexOf('[') == -1)
							return;

						string server = sData[0].Substring(0, sData[0].IndexOf('['));

						ServerInfo info;
						if (mServers.TryGetValue(server, out info))
							info.Status = "Up";

						break;
					}
			}
		}
		private void OnResponse(int numeric, string data)
		{
			switch (numeric)
			{
				case 213:
					{
						string[] sData = data.Split(' ');
						ServerInfo info = new ServerInfo();

						info.Status = "Unknown";
						info.Address = sData[1].Substring(sData[1].IndexOf('@') + 1);
						info.ServerName = sData[3];
						info.Port = sData[4];

						Dispatcher.InvokeIfRequired(() =>
							{
								int index = LinksList.Items.Add(info);
							});

						mServers.Add(info.ServerName, info);

						break;
					}
				case 219:
					{
						RefreshStatus();
						break;
					}
			}
		}

		// Event Handlers
		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			RefreshStatus();
		}
		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			Trace.WriteLine(String.Format("User has requested connect to irc server : {0}:{1}", ConnectHostname.Text, ConnectPort.Text));
			mProtocol.SendRawMessage(string.Format("CONNECT {0} {1}", ConnectHostname.Text, ConnectPort.Text));

			ConnectHostname.Text = String.Empty;
			ConnectPort.Text = String.Empty;
		}
		private void protocol_OnNoticeReceive(object sender, IMChatRoomGenericEventArgs e)
		{
			if (e.Message.StartsWith("*** Connect"))
				Dispatcher.InvokeIfRequired(() => ConnectOutput.Text = e.Message);
		}

		// Nested Classes
		private sealed class LinkContextMenu : ContextMenu
		{
			public LinkContextMenu(IRCProtocol protocol, ServerLinkWindow view)
			{
				mProtocol = protocol;
				mView = view;
			}

			protected override void OnOpened(RoutedEventArgs e)
			{
				base.OnOpened(e);
				Items.Clear();

				ServerInfo mServerInfo = mView.LinksList.SelectedItem as ServerInfo;

				if (mServerInfo == null)
					return;

				if (mServerInfo.Status == "Up")
				{
					MenuItem disconnect = new MenuItem();
					disconnect.Header = "Disconnect";
					disconnect.Click += new RoutedEventHandler(DisconnectServer_Click);
					Items.Add(disconnect);
				} else if (mServerInfo.Status == "Down") {
					MenuItem connect = new MenuItem();
					connect.Header = "Connect";
					connect.Click += new RoutedEventHandler(ConnectServer_Click);
					Items.Add(connect);
				}
			}

			private void DisconnectServer_Click(object sender, RoutedEventArgs e)
			{
				ServerInfo mServerInfo = mView.LinksList.SelectedItem as ServerInfo;

				mProtocol.SendRawMessage(String.Format("SQUIT {0}", mServerInfo.ServerName));

				mView.RefreshStatus();
			}
			private void ConnectServer_Click(object sender, RoutedEventArgs e)
			{
				ServerInfo mServerInfo = mView.LinksList.SelectedItem as ServerInfo;

				mProtocol.SendRawMessage(string.Format("CONNECT {0} {1}", mServerInfo.ServerName, mServerInfo.Port));
			}
			
			private IRCProtocol mProtocol;
			private ServerLinkWindow mView;
		}
		private sealed class ServerInfo : IComparable, IComparable<ServerInfo>, INotifyPropertyChanged
		{
			int IComparable.CompareTo(object other)
			{
				return CompareTo((ServerInfo)other);
			}
			public int CompareTo(ServerInfo other)
			{
				return ServerName.CompareTo(other);
			}

			public event PropertyChangedEventHandler PropertyChanged;

			private void NotifyPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}

			public string ServerName
			{
				get	{
					return mServerName;
				}
				set	{
					if (mServerName != value)
					{
						mServerName = value;

						NotifyPropertyChanged("ServerName");
					}
				}
			}
			public string Status
			{
				get	{
					return mStatus;
				}
				set	{
					if (mStatus != value)
					{
						mStatus = value;

						NotifyPropertyChanged("Status");
					}
				}
			}
			public string Port
			{
				get	{
					return mPort;
				}
				set	{
					if (mPort != value)
					{
						mPort = value;

						NotifyPropertyChanged("Port");
					}
				}
			}
			public string Address
			{
				get	{
					return mAddress;
				}
				set	{
					if (mAddress != value)
					{
						mAddress = value;

						NotifyPropertyChanged("Address");
					}
				}
			}
			public ContextMenu ContextMenu
			{
				get;
				set;
			}

			private string mServerName;
			private string mStatus;
			private string mPort;
			private string mAddress;
		}

		// Variables
		private SortedDictionary<string, ServerInfo> mServers; // Sorts a reference to all known links. Allows for fast search and updating of data
		private IRCProtocol mProtocol; // A reference to the protocol that this window will work with
	}
}