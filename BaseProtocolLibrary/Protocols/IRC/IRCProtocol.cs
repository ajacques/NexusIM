using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using InstantMessage.Events;

namespace InstantMessage.Protocols.Irc
{
	[IMNetwork("irc")]
	public sealed class IRCProtocol : IMProtocol, IDisposable, IHasMUCRooms<IRCChannel>, IRequiresUsername, IRequiresPassword
	{
		public IRCProtocol()
		{
			protocolType = "IRC";
			mProtocolTypeShort = "irc";
			needPassword = false;
			Port = 6667;

			CreatePingThread();

			mOnDuplicateName = (protocol, origin) => origin + '_';
		}
		public IRCProtocol(string hostname, int port = 6667, bool useSsl = false) : this()
		{
			if ((port <= 0) && (port > 0xffff))
				throw new ArgumentOutOfRangeException("port", "The port number " + port + " is not a valid port number");
			if (String.IsNullOrEmpty(hostname))
				throw new ArgumentNullException("hostname");

			Server = hostname;
			Port = port;
			SslEnabled = useSsl;
		}
		public void Dispose()
		{
			if (mProtocolStatus != IMProtocolStatus.Offline)
				Disconnect();

			Debug.WriteLine("Dispose Requested... Cleaning-up resources");

			if (mPendingHostLookup != null)
				mPendingHostLookup.Dispose();

			if (mTextStream != null)
				mTextStream.Dispose();
			mTextStream = null;
			if (mWriter != null)
				mWriter.Dispose();
			mWriter = null;
			client.Close();
			client = null;
			mDataQueue = null;
		}
		public override void BeginLogin()
		{
			if (mProtocolStatus != IMProtocolStatus.Offline)
				return;

			if (mServer == null)
				return;

			base.BeginLogin();

			Trace.WriteLine(String.Format("IRC: Beginning Login (Nickname: {0}, Server: {1})", Username, Server));

			IPAddress targetServer;
			if (IPAddress.TryParse(mServer, out targetServer))
				client = new Socket(targetServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			else
				client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			mOngoingResult = client.BeginConnect(mServer, Port, new AsyncCallback(OnSocketConnect), null);
			mLoginWaitHandle = new System.Threading.ManualResetEvent(false);
		}
		public override bool IsReady()
		{
			return !String.IsNullOrEmpty(mUsername) && !String.IsNullOrEmpty(mServer);
		}
		public override void IsReady(out IMRequiredDetail reason)
		{
			reason = IMRequiredDetail.None;
			if (String.IsNullOrEmpty(mUsername))
				reason |= IMRequiredDetail.Username;

			if (String.IsNullOrEmpty(mNickname))
				reason |= IMRequiredDetail.Nickname;

			if (String.IsNullOrEmpty(mServer))
				reason |= IMRequiredDetail.Server;
		}
		public override void Disconnect()
		{
			if (mProtocolStatus != IMProtocolStatus.Online)
				return;

			try	{
				sendData("QUIT");
			} catch (IOException) {
			} catch (NullReferenceException) {}

			client.Disconnect(false);

			triggerOnDisconnect(null);
			mProtocolStatus = IMProtocolStatus.Offline;
		}
		IChatRoom IHasMUCRooms.JoinChatRoom(string room)
		{
			return this.JoinChatRoom(room);
		}
		void IHasMUCRooms.JoinChatRoom(IChatRoom room)
		{
			this.JoinChatRoom((IRCChannel)room);
		}
		public IRCChannel JoinChatRoom(string room)
		{
			IRCChannel qchannel;
			if (mChannels.TryGetValue(room, out qchannel))
				return qchannel;

			sendData("JOIN " + room);

			IRCChannel channel = new IRCChannel(room, this);
			//mChannels.Add(channel);

			return channel;
		}
		public void JoinChatRoom(IRCChannel room)
		{
			if (mChannels.ContainsKey(room.Name))
				return;

			sendData("JOIN " + room.Name);
		}
		public void FindRoomByName(string query, RoomListCompleted onComplete)
		{
			mRoomSearchResults = new LinkedList<string>();

			SendRawMessage(String.Format("LIST {0}", query));
		}

		public override void SendMessage(string friendName, string message)
		{
			sendData(String.Format("PRIVMSG {0} :{1}", friendName, message));
		}
		public void BeginFindByHostMask(string channelName, AsyncCallback callback, object userState)
		{
			mPendingHostLookup = new HostMaskFindResult(channelName, this, callback, userState);
		}
		public IEnumerable<IRCUserMask> EndFindByHostMask(IAsyncResult result)
		{
			if (!(result is HostMaskFindResult))
				throw new ArgumentException("The result parameter must be of type IRCProtocol.HostMaskFindresult");

			var args = result as HostMaskFindResult;

			if (!args.IsCompleted)
				args.AsyncWaitHandle.WaitOne();

			return args.Results;
		}

		public void QueryServer(string command, int dataNumeric, int endNumeric, ServerResponse handler)
		{
			ServerResponse endHandler = null;
			
			endHandler = (int id, string line) =>
				{
					mRespHandlers.Remove(dataNumeric);
					mRespHandlers.Remove(endNumeric);
					endHandler = null;
					handler(endNumeric, line);
				};

			if (mRespHandlers.ContainsKey(dataNumeric))
				mRespHandlers[dataNumeric] = handler;
			else
				mRespHandlers.Add(dataNumeric, handler);

			if (mRespHandlers.ContainsKey(endNumeric))
				mRespHandlers[endNumeric] = endHandler;
			else
				mRespHandlers.Add(endNumeric, endHandler);

			SendRawMessage(command);
		}
		public void QueryServer(string command, int[] dataNumerics, int endNumeric, ServerResponse handler)
		{
			ServerResponse endHandler = null;

			dataNumerics = (int[])dataNumerics.Clone(); // Get our own copy

			endHandler = (int id, string line) =>
			{
				for (int i = 0; i < dataNumerics.Length; i++)
					mRespHandlers.Remove(dataNumerics[i]);
				mRespHandlers.Remove(endNumeric);
				endHandler = null;
				handler(endNumeric, line);
			};
			for (int i = 0; i < dataNumerics.Length; i++)
				mRespHandlers.Add(dataNumerics[i], handler);
			mRespHandlers.Add(endNumeric, endHandler);

			SendRawMessage(command);
		}

		public void Disconnect(string reason)
		{
			sendData("QUIT :" + reason);
		}
		public void LoginAsOperator(string username, string password, Action<bool, string> callback = null)
		{
			if (callback != null)
			{
				ServerResponse resp = new ServerResponse((code, msg) =>
					{
						msg = msg.Substring(msg.LastIndexOf(':') + 1);
						if (code == 491)
							callback(false, msg);
						else
							callback(true, msg);

						mRespHandlers.Remove(491);
						mRespHandlers.Remove(381);
					});
				mRespHandlers.Add(491, resp);
				mRespHandlers.Add(381, resp);
			}

			sendData(String.Format("OPER {0} {1}", username, password));
		}
		public void ApplyIRCModeToUser(string username, string channel, IRCUserModes mode)
		{
			int numModes;
			string modemask = UserModeToString(mode, out numModes);
			string userMask = "";

			for (int i = 0; i < numModes; i++)
				userMask += username + " ";

			sendData(String.Format(CultureInfo.InvariantCulture, "MODE {0} +{1} {2}", channel, modemask, userMask));
		}
		public void ApplyIRCMode(string mChannelName, string modes)
		{
			sendData(String.Format(CultureInfo.InvariantCulture, "MODE {0} {1}", mChannelName, modes));
		}
		public void RemoveIRCModeFromUser(string username, string channel, IRCUserModes mode)
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
			try {
				return mChannels[channelName];
			} catch (KeyNotFoundException) {
				return null;
			}
		}

		public void SendRawMessage(string message)
		{
			if (mProtocolStatus == IMProtocolStatus.Online)
				sendData(message);
		}
		private string UserModeToString(IRCUserModes modes, out int numModes)
		{
			int modeCount = 0;
			StringBuilder builder = new StringBuilder();
			if (modes.HasFlag(IRCUserModes.Protected))
			{
				builder.Append("a");
				modeCount++;
			}
			if (modes.HasFlag(IRCUserModes.Banned))
			{
				builder.Append("b");
				modeCount++;
			}
			if (modes.HasFlag(IRCUserModes.Invisible))
			{
				builder.Append("i");
				modeCount++;
			}
			if (modes.HasFlag(IRCUserModes.Operator))
			{
				builder.Append("o");
				modeCount++;
			}
			if (modes.HasFlag(IRCUserModes.Voice))
			{
				builder.Append("v");
				modeCount++;
			}
			if (modes.HasFlag(IRCUserModes.HalfOperator))
			{
				builder.Append("h");
				modeCount++;
			}

			numModes = modeCount;

			return builder.ToString();
		}
		private IRCUserModes CharToUserMode(char mode)
		{
			switch (mode)
			{
				case 'a':
					return IRCUserModes.Protected;
				case 'b':
					return IRCUserModes.Banned;
				case 'i':
					return IRCUserModes.Invisible;
				case 'o':
					return IRCUserModes.Operator;
				default:
					return IRCUserModes.Unknown;
			}
		}

		// Nested Classes
		private class HostMaskFindResult : IAsyncResult, IDisposable
		{
			public HostMaskFindResult(string channelName, IRCProtocol protocol, AsyncCallback callback, object userState)
			{
				AsyncState = userState;
				mResults = new List<IRCUserMask>();
				AsyncCallback = callback;
				protocol.SendRawMessage(String.Format("WHO {0}", channelName));
				mResetEvent = new ManualResetEvent(false);
			}

			public void Dispose()
			{
				if (mResetEvent != null)
					mResetEvent.Dispose();

				mResults = null;
				mResetEvent = null;
			}

			public void AppendResult(IRCUserMask mask)
			{
				mResults.Add(mask);
			}
			public void Trigger()
			{
				mResetEvent.Set();

				if (AsyncCallback != null)
					AsyncCallback(this);
			}
			public IEnumerable<IRCUserMask> Results
			{
				get	{
					return mResults;
				}
			}

			public object AsyncState
			{
				get;
				private set;
			}
			public AsyncCallback AsyncCallback
			{
				get;
				private set;
			}
			public WaitHandle AsyncWaitHandle
			{
				get	{
					return mResetEvent;
				}
			}
			public bool CompletedSynchronously
			{
				get {
					return false;
				}
			}
			public bool IsCompleted
			{
				get;
				private set;
			}

			private ManualResetEvent mResetEvent;
			private IList<IRCUserMask> mResults;
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
				if (mNickname != value)
				{
					mActualNickname = mNickname = value;

					NotifyPropertyChanged("Nickname");

					if (mProtocolStatus == IMProtocolStatus.Online)
						sendData("NICK " + mNickname);
				}
			}
		}
		public IEnumerable<IRCChannel> Channels
		{
			get	{
				return mChannels.Values;
			}
		}
		IEnumerable<IChatRoom> IHasMUCRooms.Channels
		{
			get	{
				return mChannels.Values;
			}
		}
		public int Port
		{
			get;
			set;
		}
		public bool SslEnabled
		{
			get;
			set;
		}
		public bool IsOperator
		{
			get	{
				return mIsOperator;
			}
		}
		public DuplicateNickname NicknameCollisionHandler
		{
			get	{
				return mOnDuplicateName;
			}
			set {
				mOnDuplicateName = value;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}@{1} - IRC", Nickname, Server);
		}

		/// <summary>
		/// A thread that will periodically wake up and send a ping to verify that the server is still alive
		/// </summary>
		private void PingThreadLoop()
		{
			TimeSpan mMaxIdle = TimeSpan.FromSeconds(mIdlePeriod.TotalSeconds * 2);

			while (true)
			{
				Thread.Sleep(mIdlePeriod);
				if (mProtocolStatus == IMProtocolStatus.Online && DateTime.UtcNow - mLastCommunication > mMaxIdle)
				{
					// Only throw this error if we think we are connected 
					//triggerOnError(new IMErrorEventArgs(IMProtocolErrorReason.CONNERROR, "Ping timeout"));
					Trace.WriteLine("IRC: Idle timeout");
					triggerOnError(new SocketErrorEventArgs(new SocketException(10054)));
					break;
				}
				sendData(String.Format("PING :{0}", Math.Round((DateTime.UtcNow - mConnectTime).TotalSeconds, 0)));
			}
		}

		// Protocol Handlers
		private void ParseLine(string line)
		{
			if (String.IsNullOrEmpty(line))
				return;

			if (line[0] == ':') // Standard message type
			{
				string[] parameters = line.Substring(1).Split(' ');
				string[] param = line.Substring(1).Split(mLineSplitSep, 3);
				// First param will be the from server
				// Second param is the command

				int numericReply;

				if (Int32.TryParse(parameters[1], out numericReply))
				{
					switch (numericReply)
					{
						case 001: // Connection setup stuff
							mActualServer = parameters[0];
							break;
						case 322: // LIST item

							break;
						case 323: // End /LIST
							if (mOnRoomListComplete != null)
							{
								mOnRoomListComplete(mRoomSearchResults);
								mRoomSearchResults = null;
								mOnRoomListComplete = null;
							}
							break;
						case 332: // Channel Topic
							{
								string[] chunks = param[2].Split(mLineSplitSep, 3);
								string channelName = chunks[1];
								string message = chunks[2].Substring(1);

								IRCChannel channel = FindChannelByName(channelName);
								channel.Topic = message;
							
								break;
							}
						case 333: // Channel Topic Setting
							// Penguin #main The_Problems 1308501050
							{
								string[] chunks = param[2].Split(mLineSplitSep, 4);
								string channelName = chunks[1];
								string username = chunks[2];
								long stamp = Int64.Parse(chunks[3], CultureInfo.InvariantCulture);

								IRCChannel channel = FindChannelByName(channelName);
								channel.TriggerTopicChange(channel.Topic, username);

								break;
							}
						case 353: // Users in the channel
							HandleChannelNamesList(parameters.First(s => s.StartsWith("#", StringComparison.Ordinal)), line.Substring(line.LastIndexOf(':') + 1));
							break;
						case 352:
							HandleWhoReply(parameters.Skip(3).ToArray());
							break;
						case 315:
							if (mPendingHostLookup != null)
								mPendingHostLookup.Trigger();
							break;
						case 381: // Login as Operator Success
							mIsOperator = true;
							break;
						case 433: // Nickname in Use
							HandleNicknameInUse();
							break;
						case 473: // Invite only
							HandleJoinFailedPacket(IRCJoinFailedReason.InviteOnly, parameters.Skip(3).ToArray(), line.Substring(line.IndexOf(':', 1)));
							break;
						case 474: // Banned
							HandleJoinFailedPacket(IRCJoinFailedReason.Banned, parameters.Skip(3).ToArray(), line.Substring(line.IndexOf(':', 1)));
							break;
					}
					ServerResponse resp = null;
					if (mRespHandlers.TryGetValue(numericReply, out resp))
						resp(numericReply, param[2].Split(mLineSplitSep, 2)[1]);
				} else {
					switch (parameters[1].ToUpperInvariant())
					{
						case "PRIVMSG":
							HandleMessagePacket(line, parameters);
							break;
						case "KICK":
							HandleKickPacket(parameters, line.Substring(line.IndexOf(':', 1) + 1));
							break;
						case "JOIN":
							HandleUserJoinPacket(parameters);
							break;
						case "PART":
							HandlePartPacket(line, parameters);
							break;
						case "MODE":
							HandleModeChangePacket(param[0], param[2]);
							break;
						case "NOTICE":
							HandleNotice(line.Substring(line.IndexOf(':', 1) + 1));
							break;
						case "PONG":
							HandlePongPacket(line.Substring(line.IndexOf(':', 1) + 1));
							break;
						case "TOPIC":
							break;
					}
				}
			} else {
				if (line.StartsWith("PING", StringComparison.Ordinal))
				{
					string destination = line.Substring(line.IndexOf(':') + 1);
					HandlePingPacket(destination);
				}
			}
		}
		private void HandleNotice(string message)
		{
			try	{
				if (OnNoticeReceive != null)
					OnNoticeReceive(this, new IMChatRoomGenericEventArgs() { Message = message });
			} catch (Exception) {

			}
		}
		private void HandleJoinFailedPacket(IRCJoinFailedReason reason, string[] line, string message)
		{
			string channelName = line[0];
			IRCChannel channel = FindChannelByName(channelName);

			try	{
				if (OnChannelJoinFailed != null)
					OnChannelJoinFailed(channel, new ChatRoomJoinFailedEventArgs() { Message = message, Reason = reason });
			} catch (Exception e) {
				Debug.WriteLine(e);
			}
		}
		private void HandleWhoReply(string[] line)
		{
			//{channelName} {username} {hostname} {server} {nickname} {flags} :{hopcount} {realname}
			string channelName = line[0];

			if (mPendingHostLookup != null)
			{
				IRCUserMask mask = new IRCUserMask(this);
				mask.Hostname = line[2];
				mask.Nickname = line[4];
				mask.Username = line[1];
				mPendingHostLookup.AppendResult(mask);
			}
		}
		private void HandlePartPacket(string line, string[] parameters)
		{
			IRCUserMask mask = new IRCUserMask(this, parameters[0]);

			if (mask.Nickname == mActualNickname)
			{
				string channelName = parameters[2];

				IRCChannel channel = FindChannelByName(channelName);
				channel.TriggerOnPart(line.Substring(line.IndexOf(':', 5) + 1));

				mChannels.Remove(channel.Name);
			} else {
				string channelName = parameters[2];

				IRCChannel channel = FindChannelByName(channelName);

				int index = line.IndexOf(':', 5);
				if (index == -1)
					channel.TriggerOnPart(mask, null);
				else
					channel.TriggerOnPart(mask, line.Substring(index + 1));
			}
		}
		private void HandleModeChangePacket(string username, string param)
		{
			// param will be in format: [channelName] [modes] [<user>]
			string[] parameters = param.Split(mLineSplitSep, 3);

			string target = parameters[0];

			if (target[0] == '#')
			{
				string modes = parameters[1];

				IRCChannel channel = FindChannelByName(target);

				bool isApply = false;

				string[] people = parameters[2].Split(' ');
				List<IRCUserModeChange> modeChanges = new List<IRCUserModeChange>();
				int i = 0;

				foreach (char mode in modes)
				{
					if (mode == '-')
						isApply = false;
					else if (mode == '+')
						isApply = true;
					else
					{
						if (i > people.Length - 1)
						{
						} else {
							IRCUserModeChange change = new IRCUserModeChange();
							change.UserMask = new IRCUserMask(this, people[i]);
							change.Mode = CharToUserMode(mode);
							change.IsAdd = isApply;
							modeChanges.Add(change);
							i++;
						}
					}
				}

				IRCModeChangeEventArgs args = new IRCModeChangeEventArgs();
				args.Username = new IRCUserMask(this, username);
				args.UserModes = modeChanges;
				args.RawMode = param.Substring(param.IndexOf(' ') + 1);

				channel.TriggerModeChange(args);
			}
		}
		private void HandleUserJoinPacket(string[] parameters)
		{
			string name = parameters[2].Replace(":", "");
			if (ExtractNickname(parameters[0]) != mActualNickname)
				FindChannelByName(name).TriggerOnUserJoin(parameters[0]);
			else {
				if (!mChannels.ContainsKey(name))
				{
					IRCChannel channel = new IRCChannel(name, this);
					try	{
						mChannels.Add(channel);
					} catch (ArgumentException) {

					}

					if (OnJoinChannel != null)
						OnJoinChannel(this, new IMChatRoomEventArgs() { ChatRoom = channel });
				} else {
					IRCChannel channel = FindChannelByName(name);

					channel.Joined = true;

					if (OnJoinChannel != null)
						OnJoinChannel(this, new IMChatRoomEventArgs() { ChatRoom = channel });
				}				
			}

		}
		private void HandleKickPacket(string[] parameters, string reason)
		{
			if (parameters[3] == mNickname) // It's us! 
			{
				FindChannelByName(parameters[2]).TriggerOnKicked(parameters[0], reason);
			} else {
				IRCChannel channel = FindChannelByName(parameters[2]);
				
			}
		}
		private void HandlePingPacket(string destination)
		{
			sendData("PONG " + destination);
		}
		private void HandleChannelNamesList(string channelName, string list)
		{
			IRCChannel channel = FindChannelByName(channelName);

			if (channel == null)
				channel = (IRCChannel)JoinChatRoom(channelName);

			list = list.Trim(' ');

			channel.SetParticipants(list.Split(' '));
		}
		private void HandleMessagePacket(string line, string[] parameters)
		{
			string recipient = parameters[2];

			if (recipient[0] == '#') // IRC Channel
			{
				IRCChannel channel = FindChannelByName(recipient);

				int messageStartIndex = line.IndexOf(':', line.IndexOf(' '));
				string message = line.Substring(messageStartIndex + 1);

				MessageFlags flags = MessageFlags.None;

				if (message[0] == '\x0001')
				{
					if (message.Substring(1, message.IndexOf(' ') - 1) == "ACTION")
					{
						flags = MessageFlags.UserAction;
						message = message.Substring(8, message.Length - 9);
					} else
						flags = MessageFlags.None;
				}

				IMMessageEventArgs args = new IMMessageEventArgs(new IRCUserMask(this, parameters[0]), message, flags);

				channel.ReceiveMessage(args);
			} else {
				int messageStartIndex = line.IndexOf(':', 5);
				try	{
					triggerOnMessageReceive(new IMMessageEventArgs(new IRCUserMask(this, parameters[0]), line.Substring(messageStartIndex + 1)));
				} catch (Exception) { } // Do not allow any bug in the client code to kill us
			}
		}
		private void HandlePongPacket(string pingdata)
		{
			DateTime stamp = DateTime.UtcNow;
			
			TimeSpan clientDiff = TimeSpan.FromSeconds(Convert.ToInt32(pingdata));
			TimeSpan respDiff = stamp - mConnectTime;

			TimeSpan offset = respDiff - clientDiff;

			Trace.WriteLineIf(offset.TotalSeconds > 5, "IRC: Latency Alert: " + offset.ToString());
		}
		private void HandleNicknameInUse()
		{
			triggerOnError(new IMErrorEventArgs(IMProtocolErrorReason.Unknown, "Duplicate Nickname"));
			mProtocolStatus = IMProtocolStatus.Offline;
			if (mOnDuplicateName != null)
			{
				string newNick = mOnDuplicateName(this, mActualNickname);
				if (String.IsNullOrEmpty(newNick))
					return;
				mActualNickname = newNick;
				BeginLogin();
			}
		}

		// Socket Support Methods
		private void SocketProcessByteQueue(int bytesRead)
		{
			IEnumerable<string> lines = SocketHandleLineWrapAround(bytesRead);

			foreach (string line in lines)
				ParseLine(line);
		}
		private void HandleIOError(IOException exception)
		{
			Trace.WriteLine("IRC: IOException: " + exception.Message);
			Dispose();
			triggerOnError(new SocketErrorEventArgs(new SocketException((int)SocketError.ConnectionReset)));
			//triggerOnDisconnect(new IMDisconnectEventArgs(DisconnectReason.NetworkProblem) { Exception = exception });
		}
		private void SocketReadLoop()
		{
			StreamReader reader = new StreamReader(mTextStream);
			while (client.Connected)
			{
				string line = reader.ReadLine();
				ParseLine(line);
			}

			reader.Peek();
		}

		private IEnumerable<string> SocketHandleLineWrapAround(int bytesRead)
		{
			lock (mSocketProcessLock)
			{
				string streamBuf = mTextEncoder.GetString(mDataQueue, 0, bytesRead);
				IEnumerable<string> lines = streamBuf.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); // newline means a new command
				int lineCount = lines.Count();

				// Check to see if there was a command that was cutoff at the end of the previous buffer read.
				IEnumerable<string> readableLines = lines;

				if (streamBuf.StartsWith("\r\n", StringComparison.Ordinal)) // Uncommon - The line was completed, but the newlines fell right after the 1024 mark
				{
					string[] last = new string[] { mBufferCutoffMessage.ToString() };
					mBufferCutoffMessage = null;

					readableLines = last.Union(readableLines);
				}

				if (mBufferCutoffMessage != null)
				{
					mBufferCutoffMessage.Append(lines.First());

					if (lineCount >= 2) // Check to see if there are any other commands after that one
					{
						string[] last = new string[] { mBufferCutoffMessage.ToString() };
						mBufferCutoffMessage = null;

						readableLines = last.Union(readableLines.Skip(1)); // Skip the partial message at the beginning and append the other lines to the list
					}
				}

				if (bytesRead == mDataQueue.Length) // Check to see if we've read up until the buffer's end
				{
					mBufferCutoffMessage = new StringBuilder(lines.Last()); // The last message will be queued up for the rest of the message
					readableLines = lines.Take(lineCount - 1);
				}

				return readableLines;
			}
		}
		private void readDataAsync(IAsyncResult args)
		{
			if (client == null || !client.Connected || args != mOngoingResult)
				return;

			int bytesRead;
			try	{
				bytesRead = mTextStream.EndRead(args);
			} catch (SocketException e)	{
				Trace.WriteLine("IRC: SocketException: " + e.Message);
				Dispose();
				triggerOnError(new SocketErrorEventArgs(e));
				return;
			} catch (IOException e)	{
				HandleIOError(e);
				return;
			}

			if (bytesRead == 0) // If we don't read anything, there's a problem
			{
				Disconnect();
				return;
			}

			SocketProcessByteQueue(bytesRead);
			mLastCommunication = DateTime.UtcNow;

			try {
				mOngoingResult = mTextStream.BeginRead(mDataQueue, 0, mBufferSize, new AsyncCallback(readDataAsync), null);
			} catch (IOException e) {
				HandleIOError(e);
			}
		}
		private void OnSocketConnect(IAsyncResult e)
		{
			if (e != mOngoingResult)
				return;

			try {
				client.EndConnect(e);
			} catch (SocketException x) {
				Trace.WriteLine("IRC: SocketException: " + x.Message);
				triggerOnError(new SocketErrorEventArgs(x)); // We aren't equipped enough to handle this problem. Pass it up to the supervisor

				return;
			}

			Trace.WriteLine("IRC: Connected to server");

			mTextStream = new NetworkStream(client);

			if (SslEnabled)
			{
				Trace.WriteLine("IRC: SSL enabled - Beginning SSL handshake");
				SslStream sslstream = new SslStream(mTextStream, true, new RemoteCertificateValidationCallback(VerifyServerCertificate));
				sslstream.BeginAuthenticateAsClient(Server, new AsyncCallback(CompleteSslNegotiation), sslstream);
			} else
				SetupStreams();
		}
		private void CompleteSslNegotiation(IAsyncResult e)
		{
			SslStream sslstream = (SslStream)e.AsyncState;
			try	{
				sslstream.EndAuthenticateAsClient(e);
			} catch (AuthenticationException ex) {
				Trace.WriteLine("IRC: Failed to complete SSL Handshake: " + ex.Message.ToString());
				Disconnect();
				return;
			}

			Trace.WriteLine("IRC: SSL Handshake complete. Continuing");

			mTextStream = sslstream;

			SetupStreams();
		}
		private void SetupStreams()
		{
			mWriter = new StreamWriter(mTextStream);
			mWriter.AutoFlush = true;

			CompleteLogin();

			mDataQueue = new byte[mBufferSize];

			Thread loopThread = new Thread(new ThreadStart(SocketReadLoop));
			loopThread.Start();
			//mOngoingResult = mTextStream.BeginRead(mDataQueue, 0, mBufferSize, new AsyncCallback(readDataAsync), null);
		}
		private void CompleteLogin()
		{
			if (client.Available >= 3) // Check to see if the server has an error message for us..
			{
				// ERROR :Closing Link: [192.168.2.35] (Throttled: Reconnecting too fast) -Email adam@adrensoftware.com for more information.

				byte[] buffer = new byte[1024];
				mTextStream.ReadTimeout = 100;
				int bytesRead = mTextStream.Read(buffer, 0, buffer.Length);

				string line = mTextEncoder.GetString(buffer, 0, bytesRead);

				if (line.StartsWith("ERROR", StringComparison.Ordinal))
				{
					Trace.WriteLine("IRC: Server Reported error : " + line);
					triggerOnError(new IMErrorEventArgs(IMProtocolErrorReason.Unknown, line));
					return;
				}
			}

			if (!String.IsNullOrEmpty(Password))
				sendData("PASS " + Password);

			if (!String.IsNullOrEmpty(mNickname))
				sendData("NICK " + mActualNickname);

			if (client.Available >= 1)
			{
				byte[] buffer = new byte[1024];
				int bytesRead = mTextStream.Read(buffer, 0, buffer.Length);

				StringReader reader = new StringReader(mTextEncoder.GetString(buffer, 0, bytesRead));

				while (reader.Peek() != -1)
				{
					string line = reader.ReadLine();

					if (line[0] == ':')
					{
						string[] parts = line.Split(mLineSplitSep, 3);

						if (parts[1] == "433")
						{
							HandleNicknameInUse();
							return;
						}
					}
				}
			}

			if (String.IsNullOrEmpty(mRealName))
				mRealName = mUsername;

			sendData(String.Format("USER {0} {1} {2} :{3}", mUsername, "localhost", mServer, mRealName));

			mProtocolStatus = IMProtocolStatus.Online;
			mConnectTime = DateTime.UtcNow;

			OnLogin();

			//if (mWatchThread.ThreadState != System.Threading.ThreadState.Running)
				//mWatchThread.Start();
		}
		private void sendData(string data)
		{
			try {
				mWriter.WriteLine(data);
			} catch (IOException e) {
				Trace.WriteLine("IRC: IOException: " + e.Message);
				HandleIOError(e);
			} catch (SocketException e) {
				Trace.WriteLine("IRC: SocketException: " + e.Message);
				Dispose();
				triggerOnError(new SocketErrorEventArgs(e));
			}
		}
		private string ExtractNickname(string hostmask)
		{
			return hostmask.Substring(0, hostmask.IndexOf('!'));
		}
		private bool VerifyServerCertificate(object sender, X509Certificate certificates, X509Chain chain, SslPolicyErrors errors)
		{
			CertErrorEventArgs args = new CertErrorEventArgs(certificates, chain, errors);

			triggerOnError(args);

			return args.Continue; // Ignore all Ssl errors
		}
		private void CreatePingThread()
		{
			mWatchThread = new Thread(new ThreadStart(PingThreadLoop));
			mWatchThread.Priority = ThreadPriority.BelowNormal;
			mWatchThread.Name = "IRC Ping Watch Thread";
		}

		// Events
		public event EventHandler<IMChatRoomEventArgs> OnJoinChannel;
		public event EventHandler<ChatRoomJoinFailedEventArgs> OnChannelJoinFailed;
		public event EventHandler<IMChatRoomGenericEventArgs> OnNoticeReceive;

		// Delegates
		public delegate void ServerResponse(int id, string contents);
		/// <summary>
		/// Describes a function that returns a new nickname if the currently requested one is already in use on the server.
		/// </summary>
		/// <returns>Return null if you want to cancel the connection</returns>
		public delegate string DuplicateNickname(IRCProtocol protocol, string original);

		// Variables
		private HostMaskFindResult mPendingHostLookup;
		private string mRealName = "nexusim";
		private string mNickname;
		private string mActualNickname;
		private bool mIsOperator;
		private ChatRoomCollection<IRCChannel> mChannels = new ChatRoomCollection<IRCChannel>();
		private Thread mWatchThread;
		private DateTime mLastCommunication;
		private DateTime mConnectTime;
		private static TimeSpan mIdlePeriod = TimeSpan.FromSeconds(30);
		private char[] mLineSplitSep = new char[] { ' ' };
		private SortedDictionary<int, ServerResponse> mRespHandlers = new SortedDictionary<int, ServerResponse>();
		private DuplicateNickname mOnDuplicateName;

		// Channel Search Stuff
		private RoomListCompleted mOnRoomListComplete;
		private LinkedList<string> mRoomSearchResults;

		// Socket-related Variables
		private string mActualServer;
		private object mSocketProcessLock = new object();
		private byte[] mDataQueue;
		private Socket client;
		private StreamWriter mWriter;
		private Stream mTextStream;
		private StringBuilder mBufferCutoffMessage;
		private IAsyncResult mOngoingResult;
		private const int mBufferSize = 2048;
		private static Encoding mTextEncoder = Encoding.UTF8;
	}
}