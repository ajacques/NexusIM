using System;
using System.Collections.Generic;
using System.Diagnostics;
using InstantMessage.Events;

namespace InstantMessage.Protocols.Irc
{
	public class IRCChannel : IChatRoom
	{
		internal IRCChannel(string channelName, IRCProtocol protocol)
		{
			mChannelName = channelName;
			mProtocol = protocol;
			mInChannel = true;
			mParticipants = new AdvancedSet<IRCUserMask>();

			if (OnJoin != null)
				OnJoin(this, null);
		}

		// Methods
		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
			string final;

			if (flags == MessageFlags.UserAction)
				final = String.Format("PRIVMSG {0} :\x0001ACTION {1}\x0001", mChannelName, message);
			else
				final = String.Format("PRIVMSG {0} :{1}", mChannelName, message);

			mProtocol.SendRawMessage(final);	
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
		public void ApplyUserMode(string username, IRCUserModes modes)
		{
			mProtocol.ApplyIRCModeToUser(username, mChannelName, modes);
		}
		public void ApplyUserMode(string modes)
		{
			mProtocol.ApplyIRCMode(mChannelName, modes);
		}
		public void RemoveUserMode(string username, IRCUserModes modes)
		{
			mProtocol.RemoveIRCModeFromUser(username, mChannelName, modes);
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
		public void BeginFindByHostMask(AsyncCallback callback, object userState)
		{
			mProtocol.BeginFindByHostMask(mChannelName, callback, userState);
		}
		public void InviteUser(string username, string message)
		{
			if (String.IsNullOrEmpty(message))
				mProtocol.SendRawMessage(String.Format("INVITE {0} {1}", username, Name));
			else
				mProtocol.SendRawMessage(String.Format("INVITE {0} {1} :{2}", username, Name, message));
		}
		public IEnumerable<IRCUserMask> EndFindByHostMask(IAsyncResult result)
		{
			return mProtocol.EndFindByHostMask(result);
		}

		internal void ReceiveMessage(IMMessageEventArgs args)
		{
			try {
				if (OnMessageReceived != null)
					OnMessageReceived(this, args);
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void SetParticipants(IList<string> participants)
		{
			foreach (string participant in participants)
				mParticipants.Add(IRCUserMask.FromNickname(mProtocol, participant));

			if (OnUserListReceived != null)
				OnUserListReceived(this, null);
		}
		internal void TriggerOnKicked(string kicker, string reason)
		{
			mInChannel = false;
			try {
				if (OnKickedFromChannel != null)
					OnKickedFromChannel(this, new IMChatRoomGenericEventArgs() { Username = new IRCUserMask(mProtocol, kicker), Message = reason });
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void TriggerOnUserJoin(string username)
		{
			try {
				if (OnUserJoin != null)
					OnUserJoin(this, new IMChatRoomGenericEventArgs() { Username = new IRCUserMask(mProtocol, username) });
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void TriggerOnPart(string reason)
		{
			mInChannel = false;

			try {
				if (OnLeave != null)
					OnLeave(this, new IMChatRoomGenericEventArgs() { Message = reason, UserRequested = UserRequestedPart });
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void TriggerOnPart(IRCUserMask mask, string reason)
		{
			try {
				if (OnUserPart != null)
					OnUserPart(this, new IMChatRoomGenericEventArgs() { Message = reason, Username = mask });
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void TriggerModeChange(IEnumerable<IRCUserModeChange> users)
		{
			if (OnModeChange != null)
				OnModeChange(this, new IRCModeChangeEventArgs() { UserModes = users });
		}
		internal void TriggerTopicChange(string topic, string username)
		{
			if (TopicChanged != null)
				TopicChanged(this, new IMChatRoomGenericEventArgs() { Username = new IRCUserMask(mProtocol, username), Message = topic });
		}

		// Properties
		public string Name
		{
			get {
				return mChannelName;
			}
		}
		IEnumerable<IContact> IChatRoom.Participants
		{
			get {
				return mParticipants;
			}
		}
		public IEnumerable<IRCUserMask> Participants
		{
			get	{
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
		public IMProtocol Protocol
		{
			get {
				return mProtocol;
			}
		}
		public string Topic
		{
			get	{
				return mTopic;
			}
			internal set {
				mTopic = value;
			}
		}

		// Events
		public event EventHandler<IMMessageEventArgs> OnMessageReceived;
		public event EventHandler OnUserListReceived;
		public event EventHandler<IMChatRoomGenericEventArgs> OnKickedFromChannel;
		public event EventHandler OnJoin;
		public event EventHandler<IMChatRoomGenericEventArgs> OnUserJoin;
		public event EventHandler<IMChatRoomGenericEventArgs> OnUserPart;
		public event EventHandler<IRCModeChangeEventArgs> OnModeChange;
		public event EventHandler<IMChatRoomGenericEventArgs> OnLeave;
		public event EventHandler<IMChatRoomGenericEventArgs> TopicChanged;

		//Variables
		private bool mInChannel;
		private AdvancedSet<IRCUserMask> mParticipants;
		private IRCProtocol mProtocol;
		private string mChannelName;
		private string mTopic;
	}
}