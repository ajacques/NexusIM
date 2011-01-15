using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using InstantMessage.Events;
using System.Globalization;

namespace InstantMessage.Protocols.Irc
{
	public sealed class IRCProtocol : IMProtocol, IDisposable
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
				throw new InvalidOperationException("Disconnect from the server before you call Dispose");

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
			client = new TcpClient();

			client.BeginConnect(mServer, 6667, new AsyncCallback(OnSocketConnect), null);
			mLoginWaitHandle = new System.Threading.ManualResetEvent(false);
		}
		public override void Disconnect()
		{
			sendData("SQUIT");

			triggerOnDisconnect(this, null);
			status = IMProtocolStatus.OFFLINE;

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
		public void BeginFindByHostMask(string channelName, AsyncCallback callback, object userState)
		{
			mPendingHostLookup = new HostMaskFindResult(channelName, this, callback, userState);
		}
		public IEnumerable<IrcUserMask> EndFindByHostMask(IAsyncResult result)
		{
			if (!(result is HostMaskFindResult))
				throw new ArgumentException("The result parameter must be of type IRCProtocol.HostMaskFindresult");

			var args = result as HostMaskFindResult;

			if (!args.IsCompleted)
				args.AsyncWaitHandle.WaitOne();

			return args.Results;
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

			sendData(String.Format(CultureInfo.InvariantCulture, "MODE {0} +{1} {2}", channel, modemask, userMask));
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

		public void SendRawMessage(string message)
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
			if (modes.HasFlag(IrcUserModes.Banned))
			{
				builder.Append("b");
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
		private IrcUserModes CharToUserMode(char mode)
		{
			switch (mode)
			{
				case 'a':
					return IrcUserModes.Protected;
				case 'b':
					return IrcUserModes.Banned;
				case 'i':
					return IrcUserModes.Invisible;
				case 'o':
					return IrcUserModes.Operator;
				default:
					return IrcUserModes.Unknown;
			}
		}

		// Nested Classes
		private class HostMaskFindResult : IAsyncResult, IDisposable
		{
			public HostMaskFindResult(string channelName, IRCProtocol protocol, AsyncCallback callback, object userState)
			{
				AsyncState = userState;
				mResults = new List<IrcUserMask>();
				AsyncCallback = callback;
				protocol.SendRawMessage(String.Format("WHO {0}", channelName));
				mResetEvent = new ManualResetEvent(false);
			}

			public void Dispose()
			{
				if (mResetEvent != null)
					mResetEvent.Dispose();
			}

			public void AppendResult(IrcUserMask mask)
			{
				mResults.Add(mask);
			}
			public void Trigger()
			{
				mResetEvent.Set();

				if (AsyncCallback != null)
					AsyncCallback(this);
			}
			public IEnumerable<IrcUserMask> Results
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
				get { throw new NotImplementedException(); }
			}
			public bool IsCompleted
			{
				get;
				private set;
			}

			private string mMask;
			private ManualResetEvent mResetEvent;
			private IList<IrcUserMask> mResults;
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

		// Protocol Handlers
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
							HandleChannelNamesList(parameters.First(s => s.StartsWith("#") | s.StartsWith("&")), line.Substring(line.LastIndexOf(':') + 1));
							break;
						case 352:
							HandleWhoReply(parameters.Skip(3).ToArray());
							break;
						case 315:
							if (mPendingHostLookup != null)
								mPendingHostLookup.Trigger();
							break;
					}
				} else {
					switch (parameters[1].ToUpper())
					{
						case "PRIVMSG":
							HandleMessagePacket(line, parameters);
							break;
						case "KICK":
							HandleKickPacket(parameters);
							break;
						case "JOIN":
							HandleUserJoinPacket(parameters);
							break;
						case "PART":
							HandlePartPacket(line, parameters);
							break;
						case "MODE":
							HandleModeChangePacket(line, parameters);
							break;
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
		private void HandleWhoReply(string[] line)
		{
			//{channelName} {username} {hostname} {server} {nickname} {flags} :{hopcount} {realname}
			string channelName = line[0];

			if (mPendingHostLookup != null)
			{
				IrcUserMask mask = new IrcUserMask(this);
				mask.Hostname = line[2];
				mask.Nickname = line[4];
				mask.Username = line[1];
				mPendingHostLookup.AppendResult(mask);
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
		private void HandleModeChangePacket(string line, string[] parameters)
		{
			string channelName = parameters[2];

			if (channelName == mNickname)
				return;

			string modes = parameters[3];

			IRCChannel channel = mChannels.First(chan => chan.Name == channelName);
			
			bool isApply = false;

			string[] people = parameters.Skip(4).ToArray();
			List<IrcUserModeChange> modeChanges = new List<IrcUserModeChange>();
			int i = 0;

			foreach (char mode in modes)
			{
				if (mode == '-')
					isApply = false;
				else if (mode == '+')
					isApply = true;
				else {
					if (i > people.Length - 1)
					{
					} else {
						IrcUserModeChange change = new IrcUserModeChange();
						change.UserMask = new IrcUserMask(this, people[i]);
						change.Mode = CharToUserMode(mode);
						change.IsAdd = isApply;
						modeChanges.Add(change);
						i++;
					}
				}
			}

			channel.TriggerModeChange(modeChanges);
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
				try	{
					triggerOnMessageReceive(new IMMessageEventArgs(new IrcUserMask(this, parameters[0]), line.Substring(messageStartIndex + 1)));
				} catch (Exception) { } // Do not allow any bug in the client code to kill us
			}
		}

		// Socket Support Methods
		private void readDataAsync(IAsyncResult args)
		{
			int bytesRead;
			try	{
				bytesRead = mTextStream.EndRead(args);
			} catch (SocketException e)	{
				Dispose();
				triggerOnDisconnect(this, new IMDisconnectEventArgs(DisconnectReason.NetworkProblem) { Exception = e });
				return;
			}

			if (bytesRead == 0) // If we don't read anything, there's a problem
			{
				Dispose();
				return;
			}
			
			string streamBuf = mTextEncoder.GetString(mDataQueue, 0, bytesRead);
			IEnumerable<string> lines = streamBuf.Split(new char[] { '\r', '\n' } ,StringSplitOptions.RemoveEmptyEntries); // newline means a new command
			int lineCount = lines.Count();

			// Check to see if there was a command that was cutoff at the end of the previous buffer read.
			IEnumerable<string> readableLines = lines;
			if (mBufferCutoffMessage != null)
			{
				mBufferCutoffMessage.Append(lines.First());

				if (lineCount >= 2) // Check to see if there are any other commands after that one
				{
					string[] last = new string[] { mBufferCutoffMessage.ToString() };
					mBufferCutoffMessage = null;

					readableLines = last.Skip(1).Union(readableLines); // Skip the partial message at the beginning and append the other lines to the list
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
		private HostMaskFindResult mPendingHostLookup;
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