using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InstantMessage
{
	class IMIRCProtocol : IMProtocol
	{
		public IMIRCProtocol()
		{
			protocolType = "IRC";
			mProtocolTypeShort = "irc";
			supportsMUC = true;
			needPassword = false;
		}
		public override void BeginLogin()
		{
			if (IMSettings.GetAccountSetting(this, "server", "null") == "null")
			{
				IMSettings.SetAccountSetting(this, "server", mServer);
			} else {
				mServer = IMSettings.GetAccountSetting(this, "server", "null");
			}
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			socket.BeginConnect(mServer, 6667, new AsyncCallback(this.ConnectCallback), socket);

		}
		public override void Disconnect()
		{
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			
		}
		public override void JoinChatRoom(string room)
		{
			sendData("JOIN " + room + separator);

			ChatRoomContainer chatroom = new ChatRoomContainer(room, new List<string>());
			frmMUCChatWindow win = new frmMUCChatWindow();
			win.ChatRoomContainer = chatroom;
			win.Protocol = this;

			MethodInvoker invoker = new MethodInvoker(delegate() { win.Show(); });
			frmMain.Instance.BeginInvoke(invoker);

			chatwins.Add(room, win);
		}
		public override void LeaveChatRoom(string room)
		{
			sendData("PART " + room + separator);

			chatwins[room].Close();
			chatwins.Remove(room);
		}
		public override void SendMessageToRoom(string roomName, string message)
		{
			sendData("PRIVMSG " + roomName + " :" + message + separator);
		}

		private void readDataAsync(IAsyncResult e)
		{
			string streamBuf = Encoding.Default.GetString(dataqueue);

			if (streamBuf.Contains(" "))
			{
				string type = streamBuf.Substring(0, streamBuf.IndexOf(" "));

				if (type == "PING")
				{
					// Not sure what this is... playing ping pong with a computer? this could go on all day
					sendData("PONG " + streamBuf.Substring(streamBuf.IndexOf(" ")));
				}

				// Perform Regex matches on all incoming messages
				if (Regex.IsMatch(streamBuf, @":([\w\S]*) 353 ?[a-zA-Z]* . ([a-zA-Z0-9#]*) :(([a-zA-Z@ ]*)*)"))
				{
					// User just joined a room.. Get user list
					Match members = Regex.Match(streamBuf, @":([\w\S]*) 353 ?[a-zA-Z]* . ([a-zA-Z0-9#]*) :(([a-zA-Z@ ]*)*)");
					List<string> people = new List<string>();
					string[] users = members.Groups[3].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					// User list will now be in a space-separated list (ex. SirSpam slashquit @ChanServ)
					foreach (string user in users)
					{
						people.Add(user);
					}

					frmMUCChatWindow win = chatwins[members.Groups[2].Value];
					win.ChatRoomContainer.Occupants.InsertRange(0, people);
					win.UpdateRoom();
				} else if (Regex.IsMatch(streamBuf, @":([\w]*)!([\w\S]*) PRIVMSG (#[a-zA-Z0-9]*) :(.*)")) {
					// Chat Room received message
					Match message = Regex.Match(streamBuf, @":([\w]*)!([\w\S]*) PRIVMSG (#[a-zA-Z0-9]*) :(.*)");

					chatwins[message.Groups[3].Value].AppendChatMessage(message.Groups[1].Value, message.Groups[4].Value);
				} else if (Regex.IsMatch(streamBuf, @":([\w]*)!([\w\S]*) PART (#[a-zA-Z0-9]*) :(.*)")) {
					Match leavemsg = Regex.Match(streamBuf, @":([\w]*)!([\w\S]*) PART (#[a-zA-Z0-9]*) :(.*)");

					frmMUCChatWindow win = chatwins[leavemsg.Groups[3].Value];
					win.ChatRoomContainer.Occupants.Remove(leavemsg.Groups[1].Value);
					win.UpdateRoom();
					win.AppendCustomMessage(new UserRoomLeaveMessage(leavemsg.Groups[1].Value));
				} else if (Regex.IsMatch(streamBuf, @":([\w]*)!n=([\w\S]*) JOIN :(#[a-zA-Z0-9]*)")) {
					Match joinmsg = Regex.Match(streamBuf, @":([\w]*)!n=([\w\S]*) JOIN :(#[a-zA-Z0-9]*)");

					frmMUCChatWindow win = chatwins[joinmsg.Groups[3].Value];
					win.ChatRoomContainer.Occupants.Add(joinmsg.Groups[1].Value);
					win.UpdateRoom();
					win.AppendCustomMessage(new UserRoomJoinMessage(joinmsg.Groups[1].Value));
				} else if (Regex.IsMatch(streamBuf, @":([\w]*)!([\w\S]*) KICK (#[a-zA-Z0-9]*) [^ ]* :([^ ]*)")) {
					Match kickmsg = Regex.Match(streamBuf, @":([\w]*)!([\w\S]*) KICK (#[a-zA-Z0-9]*) [^ ]* :([^ ]*)");

					frmMUCChatWindow win = chatwins[kickmsg.Groups[3].Value];
					//win.AppendCustomMessage(new )
				} else if (Regex.IsMatch(streamBuf, @":([\w]*)!n=([\w\S]*) MODE (#[a-zA-Z0-9]*) ((\-|\+)([a-z])*) (.*)")) {
					Match modemsg = Regex.Match(streamBuf, @":([\w]*)!n=([\w\S]*) MODE (#[a-zA-Z0-9]*) ((\-|\+)([a-z])*) (.*)");

					frmMUCChatWindow win = chatwins[modemsg.Groups[3].Value];
				}
			}

			dataqueue = new byte[512];
			socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(this.readDataAsync), this);
		}
		private void ConnectCallback(IAsyncResult e)
		{
			sendData("NICK " + mUsername);
			sendData(separator + "USER " + mUsername + " unknown unknown :" + mUsername + separator);

			socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(this.readDataAsync), this);
			status = IMProtocolStatus.ONLINE;

			triggerOnLogin(null);
		}
		private void sendData(string data)
		{
			socket.Send(Encoding.UTF8.GetBytes(data));
		}

		private byte[] dataqueue = new byte[512];
		private Socket socket;
		private string separator = Encoding.Default.GetString(new byte[] { 0x0d, 0x0a });
		private Dictionary<string, frmMUCChatWindow> chatwins = new Dictionary<string,frmMUCChatWindow>();
	}
}