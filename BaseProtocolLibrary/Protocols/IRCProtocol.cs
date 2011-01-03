using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using InstantMessage.Events;
using System.Linq;
using System.Collections;

namespace InstantMessage.Protocols.Irc
{
	public class IRCModeChangeEventArgs : EventArgs
	{
		public IGrouping<string, IrcUserModes> AddedModes
		{
			get;
			internal set;
		}
		public IGrouping<string, IrcUserModes> RemovedModes
		{
			get;
			internal set;
		}
	}
	public class IRCChannel : IChatRoom
	{
		internal IRCChannel(string channelName, IRCProtocol protocol)
		{
			mChannelName = channelName;
			mProtocol = protocol;
			mInChannel = true;

			if (OnJoin != null)
				OnJoin(this, null);
		}

		// Methods
		public void SayMessage(string message)
		{
			mProtocol.SendRawMessage(String.Format("PRIVMSG {0} :{1}", mChannelName, message));	
		}
		public void KickUser(string username)
		{
			mProtocol.SendRawMessage(String.Format("KICK {0} {1}", mChannelName, username));
		}
		public void KickUser(string username, string message)
		{
			mProtocol.SendRawMessage(String.Format("KICK {0} {1} :{2}", mChannelName, username, message));;
		}
		public void Rejoin()
		{
			if (mInChannel)
				return;

			UserRequestedPart = false;

			mProtocol.SendRawMessage("JOIN " + mChannelName);
			if (OnJoin != null)
				OnJoin(this, null);
		}
		public void ApplyUserMode(string username, IrcUserModes modes)
		{
			mProtocol.ApplyIRCModeToUser(username, mChannelName, modes);
		}
		public void Leave(string reason)
		{
			if (!mInChannel)
				throw new InvalidOperationException();

			UserRequestedPart = true;

			if (reason != null)
				mProtocol.SendRawMessage(String.Format("PART {0} :{1}", mChannelName, reason));
			else
				mProtocol.SendRawMessage("PART " + mChannelName);
		}

		internal void ReceiveMessage(string sender, string message)
		{
			try {
				if (OnMessageReceived != null)
					OnMessageReceived(this, new IMMessageEventArgs(new IrcUserMask(mProtocol, sender), message));
			} catch (Exception e) {
				Trace.TraceError(e.Message);
			}
		}
		internal void SetParticipants(IList<string> participants)
		{
			mParticipants = participants;
			if (OnUserListReceived != null)
				OnUserListReceived(this, null);
		}
		internal void TriggerOnKicked(string kicker, string reason)
		{
			mInChannel = false;
			if (OnKickedFromChannel != null)
				OnKickedFromChannel(this, new IMChatRoomGenericEventArgs() { Username = kicker, Message = reason });
		}
		internal void TriggerOnUserJoin(string username)
		{
			if (OnUserJoin != null)
				OnUserJoin(this, new IMChatRoomGenericEventArgs() { Username = username});
		}
		internal void TriggerOnPart(string reason)
		{
			mInChannel = false;

			if (OnLeave != null)
				OnLeave(this, new IMChatRoomGenericEventArgs() { Message = reason, UserRequested = UserRequestedPart });
		}

		// Properties
		public string Name
		{
			get {
				return mChannelName;
			}
		}
		public IEnumerable<string> Participants
		{
			get {
				return mParticipants;
			}
		}
		public bool Joined
		{
			get	{
				return mInChannel;
			}
			internal set {
				mInChannel = value;
			}
		}
		internal bool UserRequestedPart
		{
			get;
			private set;
		}

		// Events
		public event EventHandler<IMMessageEventArgs> OnMessageReceived;
		public event EventHandler OnUserListReceived;
		public event EventHandler<IMChatRoomGenericEventArgs> OnKickedFromChannel;
		public event EventHandler OnJoin;
		public event EventHandler<IMChatRoomGenericEventArgs> OnUserJoin;
		public event EventHandler<IRCModeChangeEventArgs> OnModeChange;
		public event EventHandler<IMChatRoomGenericEventArgs> OnLeave;

		//Variables
		private bool mInChannel;
		private IList<string> mParticipants;
		private IRCProtocol mProtocol;
		private string mChannelName;
	}
	public class IrcUserMask : IContact
	{
		public IrcUserMask(IRCProtocol protocol, string input)
		{
			Protocol = protocol;
			int exclaim = input.IndexOf("!");
			int at = input.IndexOf("@");

			Nickname = input.Substring(0, exclaim);
			Username = input.Substring(exclaim + 1, at - exclaim - 1);
			Hostname = new Uri("irc://" + input.Substring(at + 1));
		}
		public void sendMessage(string message)
		{
		}

		public override string ToString()
		{
			return String.Format("{0}!{1}@{2}", Nickname, Username, Hostname.Host);
		}
		public string Nickname
		{
			get;
			set;
		}
		public string Username
		{
			get;
			set;
		}
		public Uri Hostname
		{
			get;
			set;
		}
		public IRCProtocol Protocol
		{
			get;
			private set;
		}
	}
	[Flags]
	public enum IrcUserModes
	{
		None = 0,
		Invisible = 1,
		Protected = 2, // a
		Operator = 4, // o
		Voice = 8 // v
	}
	[Flags]
	public enum IrcChannelModes
	{
		None,
		Moderated,
		InviteOnly,
	}
	public class IRCProtocol : IMProtocol, IDisposable
	{
		public IRCProtocol()
		{
			protocolType = "IRC";
			mProtocolTypeShort = "irc";
			supportsMUC = true;
			needPassword = false;
		}
		public IRCProtocol(string hostname, int port) : this()
		{
			Server = hostname;
		}
		public void Dispose()
		{
			if (status != IMProtocolStatus.OFFLINE)
			{
				Debug.WriteLine("Dispose Requested... Cleanup resources");
				triggerOnDisconnect(this, null);
				status = IMProtocolStatus.OFFLINE;
			}

			mTextStream = null;
			mWriter = null;
			client.Close();
			client = null;
			mDataQueue = null;
		}
		public override void BeginLogin()
		{
			client = new TcpClient();

			client.BeginConnect(mServer, 6667, new AsyncCallback(OnSocketConnect), null);
			mLoginWaitHandle = new System.Threading.ManualResetEvent(false);
		}
		public override void Disconnect()
		{
			sendData("SQUIT");

			Dispose();
		}
		public override IChatRoom JoinChatRoom(string room)
		{
			if (mChannels.Any(chan => chan.Name == room))
				return mChannels.First(chan => chan.Name == room);

			sendData("JOIN " + room);

			IRCChannel channel = new IRCChannel(room, this);
			mChannels.Add(channel);

			return channel;
		}
		public override void SendMessage(string friendName, string message)
		{
			sendData(String.Format("PRIVMSG {0} :{1}", friendName, message));
		}

		public void Disconnect(string reason)
		{
			sendData("SQUIT :" + reason);
			Dispose();
		}
		public void LoginAsOperator(string username, string password)
		{
			sendData(String.Format("OPER {0} {1}", username, password));
		}
		public void ApplyIRCModeToUser(string username, string channel, IrcUserModes mode)
		{
			int numModes;
			string modemask = UserModeToString(mode, out numModes);
			string userMask = "";

			for (int i = 0; i < numModes; i++)
				userMask += username + " ";

			sendData(String.Format("MODE {0} +{1} {2}", channel, modemask, userMask));
		}
		public void RemoveIRCModeFromUser(string username, string channel, IrcUserModes mode)
		{
			int numModes;
			string modemask = UserModeToString(mode, out numModes);

			sendData(String.Format("MODE {0} -{1} {2}", channel, modemask, username));
		}
		/// <summary>
		/// Finds a known channel by the name (ex. #main)
		/// </summary>
		/// <param name="channelName">The name of the channel to find (ex. #help)</param>
		/// <returns>Null if no results</returns>
		public IRCChannel FindChannelByName(string channelName)
		{
			return mChannels.FirstOrDefault(chan => chan.Name == channelName);
		}

		internal void SendRawMessage(string message)
		{
			sendData(message);
		}
		private string UserModeToString(IrcUserModes modes, out int numModes)
		{
			int modeCount = 0;
			StringBuilder builder = new StringBuilder();
			if (modes.HasFlag(IrcUserModes.Protected))
			{
				builder.Append("a");
				modeCount++;
			}
			if (modes.HasFlag(IrcUserModes.Invisible))
			{
				builder.Append("i");
				modeCount++;
			}
			if (modes.HasFlag(IrcUserModes.Operator))
			{
				builder.Append("o");
				modeCount++;
			}
			if (modes.HasFlag(IrcUserModes.Voice))
			{
				builder.Append("v");
				modeCount++;
			}

			numModes = modeCount;

			return builder.ToString();
		}

		// Properties
		public string RealName
		{
			get	{
				return mRealName;
			}
			set	{
				mRealName = value;
			}
		}
		public string Nickname
		{
			get	{
				return mNickname;
			}
			set	{
				mNickname = value;

				NotifyPropertyChanged("Nickname");

				if (status == IMProtocolStatus.ONLINE)
					sendData("NICK " + mNickname);
			}
		}
		public IEnumerable<IRCChannel> Channels
		{
			get	{
				return mChannels;
			}
		}

		private void ParseLine(string line)
		{
			Debug.WriteLine("<-- " + line);

			if (line[0] == ':') // Standard message type
			{
				string[] parameters = line.Substring(1).Split(' ');
				// First param will be the from server
				// Second param is the command

				int numericReply;

				if (Int32.TryParse(parameters[1], out numericReply))
				{
					switch (numericReply)
					{
						case 353: // Users in the channel
							{
								HandleChannelNamesList(parameters.First(s => s.StartsWith("#") | s.StartsWith("&")), line.Substring(line.LastIndexOf(':') + 1));
								break;
							}
					}
				} else {
					switch (parameters[1].ToUpper())
					{
						case "PRIVMSG":
							{
								HandleMessagePacket(line, parameters);
								break;
							}
						case "KICK":
							{
								HandleKickPacket(parameters);
								break;
							}
						case "JOIN":
							{
								HandleUserJoinPacket(parameters);
								break;
							}
						case "PART":
							{
								HandlePartPacket(line, parameters);
								break;
							}
					}
				}
			} else {
				if (line.StartsWith("PING"))
				{
					string destination = line.Substring(line.IndexOf(":") + 1);
					HandlePingPacket(destination);
				}
			}
		}

		private void HandlePartPacket(string line, string[] parameters)
		{
			IrcUserMask mask = new IrcUserMask(this, parameters[0]);

			if (mask.Nickname == mNickname)
			{
				string channelName = parameters[2];

				IRCChannel channel = mChannels.First(chan => chan.Name == channelName);
				channel.TriggerOnPart(line.Substring(line.IndexOf(':', 5) + 1));
			}
		}
		private void HandleModeChangePacket(string[] parameters)
		{
			string channelName = parameters[3];
			string mode = parameters[4];


		}
		private void HandleUserJoinPacket(string[] parameters)
		{
			string name = parameters[2].Replace(":", "");
			if (ExtractNickname(parameters[0]) != mNickname)
				mChannels.First(c => c.Name == name).TriggerOnUserJoin(parameters[0]);
			else {
				if (!mChannels.Any(chan => chan.Name == name))
				{
					IRCChannel channel = new IRCChannel(name, this);
					mChannels.Add(channel);

					if (OnForceJoinChannel != null)
						OnForceJoinChannel(channel, null);
				} else{
					IRCChannel channel = mChannels.First(chan => chan.Name == name);

					if (channel.Joined)
						return;
					channel.Joined = true;
				}

				
			}

		}
		private void HandleKickPacket(string[] parameters)
		{
			if (parameters[3] == mNickname) // It's us! 
			{
				mChannels.First(c => c.Name == parameters[2]).TriggerOnKicked(parameters[0], "");
			}
		}
		private void HandlePingPacket(string destination)
		{
			sendData("PONG " + destination);
		}
		private void HandleChannelNamesList(string channelName, string list)
		{
			IRCChannel channel = mChannels.FirstOrDefault(chan => chan.Name == channelName);

			channel.SetParticipants(list.Split(' '));
		}
		private void HandleMessagePacket(string line, string[] parameters)
		{
			string recipient = parameters[2];

			if (recipient[0] == '#') // IRC Channel
			{
				IRCChannel channel = mChannels.FirstOrDefault(chan => chan.Name == recipient);

				int messageStartIndex = line.IndexOf(':', 5);
				channel.ReceiveMessage(parameters[0], line.Substring(messageStartIndex + 1));
			} else {
				int messageStartIndex = line.IndexOf(':', 5);
				triggerOnMessageReceive(new IMMessageEventArgs(new IrcUserMask(this, parameters[0]), line.Substring(messageStartIndex + 1)));
			}
		}

		private void readDataAsync(IAsyncResult e)
		{
			int bytesRead = mTextStream.EndRead(e);

			if (bytesRead == 0)
			{
				Dispose();
				return;
			}
			
			string streamBuf = mTextEncoder.GetString(mDataQueue, 0, bytesRead);
			IEnumerable<string> lines = streamBuf.Split(new char[] { '\r', '\n' } ,StringSplitOptions.RemoveEmptyEntries);
			int lineCount = lines.Count();

			IEnumerable<string> readableLines = lines;
			if (mBufferCutoffMessage != null)
			{
				mBufferCutoffMessage.Append(lines.First());

				if (lineCount >= 2)
				{
					string[] last = new string[] { mBufferCutoffMessage.ToString() };
					mBufferCutoffMessage = null;

					readableLines = last.Skip(1).Union(readableLines);
				}
			}

			if (bytesRead == mBufferSize) // Check to see if we've read up until the buffer's end
				readableLines = lines.Take(lineCount - 1);

			foreach (string line in readableLines)
				ParseLine(line);

			if (bytesRead == mBufferSize)
				mBufferCutoffMessage = new StringBuilder(lines.Last()); // The last message will be queued up for the rest of the message

			mTextStream.BeginRead(mDataQueue, 0, mBufferSize, new AsyncCallback(readDataAsync), null);
		}
		private void OnSocketConnect(IAsyncResult e)
		{
			client.EndConnect(e);
			mTextStream = client.GetStream();
			mWriter = new StreamWriter(mTextStream);
			mWriter.AutoFlush = true;

			if (!String.IsNullOrEmpty(Password))
				sendData("PASS " + Password);

			if (!String.IsNullOrEmpty(mNickname))
				sendData("NICK " + mNickname);
			sendData(String.Format("USER {0} {1} {2} :{3}", mUsername, Environment.MachineName, mServer, mRealName));

			status = IMProtocolStatus.ONLINE;

			LoginWaitHandle.Set();
			triggerOnLogin(null);

			mDataQueue = new byte[mBufferSize];
			mTextStream.BeginRead(mDataQueue, 0, mBufferSize, new AsyncCallback(readDataAsync), null);
		}
		private void sendData(string data)
		{
			Debug.WriteLine("--> " + data);
			mWriter.WriteLine(data);
		}
		private string ExtractNickname(string hostmask)
		{
			return hostmask.Substring(0, hostmask.IndexOf("!"));
		}

		// Events
		public event EventHandler OnForceJoinChannel;

		// Variables
		private string mRealName = "nexusim";
		private string mNickname;
		private StringBuilder mBufferCutoffMessage;
		private IList<IRCChannel> mChannels = new List<IRCChannel>();
		private byte[] mDataQueue;
		private TcpClient client;
		private StreamWriter mWriter;
		private Stream mTextStream;
		private const int mBufferSize = 1024;
		private static Encoding mTextEncoder = Encoding.ASCII;
	}
}