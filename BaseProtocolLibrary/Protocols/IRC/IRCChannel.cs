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
			mParticipants = new SortedSet<string>();

			if (OnJoin != null)
				OnJoin(this, null);
		}

		// Methods
		public void SendMessage(string message)
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
		public void ApplyUserMode(string username, IRCUserModes modes)
		{
			mProtocol.ApplyIRCModeToUser(username, mChannelName, modes);
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
		public IEnumerable<IRCUserMask> EndFindByHostMask(IAsyncResult result)
		{
			return mProtocol.EndFindByHostMask(result);
		}

		internal void ReceiveMessage(string sender, string message)
		{
			try {
				if (OnMessageReceived != null)
					OnMessageReceived(this, new IMMessageEventArgs(new IRCUserMask(mProtocol, sender), message));
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void SetParticipants(IList<string> participants)
		{
			foreach (string participant in participants)
				mParticipants.Add(participant);

			if (OnUserListReceived != null)
				OnUserListReceived(this, null);
		}
		internal void TriggerOnKicked(string kicker, string reason)
		{
			mInChannel = false;
			try {
				if (OnKickedFromChannel != null)
					OnKickedFromChannel(this, new IMChatRoomGenericEventArgs() { Username = kicker, Message = reason });
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}
		internal void TriggerOnUserJoin(string username)
		{
			try {
				if (OnUserJoin != null)
					OnUserJoin(this, new IMChatRoomGenericEventArgs() { Username = username});
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
		internal void TriggerModeChange(IEnumerable<IRCUserModeChange> users)
		{
			if (OnModeChange != null)
				OnModeChange(this, new IRCModeChangeEventArgs() { UserModes = users });
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
		public IMProtocol Protocol
		{
			get {
				return mProtocol;
			}
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
		private SortedSet<string> mParticipants;
		private IRCProtocol mProtocol;
		private string mChannelName;
	}
}
