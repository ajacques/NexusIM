using System;
using System.Collections.Generic;
using System.Linq;
using InstantMessage.Protocols.Irc;

namespace InstantMessage.Events
{
	public class IRCModeChangeEventArgs : EventArgs
	{
		public IEnumerable<IRCUserModeChange> UserModes
		{
			get;
			internal set;
		}
		public IGrouping<string, IRCUserModes> ChannelModes
		{
			get;
			internal set;
		}
	}
	public class ChatRoomJoinFailedEventArgs : EventArgs
	{
		public IRCJoinFailedReason Reason
		{
			get;
			internal set;
		}
		public string Message
		{
			get;
			internal set;
		}
	}
}
namespace InstantMessage.Protocols.Irc
{
	public class IRCUserMask : IContact, IComparable
	{
		internal IRCUserMask(IRCProtocol protocol)
		{
			Protocol = protocol;
		}
		internal IRCUserMask(IRCProtocol protocol, string input)
		{
			Protocol = protocol;
			if (!input.Contains('!'))
			{
				Nickname = input.Substring(0);
				return;
			}
			int exclaim = input.IndexOf("!");

			if (!input.Contains('@'))
			{
				Username = input.Substring(exclaim + 1);
				return;
			}
			int at = input.IndexOf("@");

			Nickname = input.Substring(0, exclaim);
			Username = input.Substring(exclaim + 1, at - exclaim - 1);
			Hostname = input.Substring(at + 1);
		}
		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
		}

		internal static IRCUserMask FromNickname(IRCProtocol protocol, string nickname)
		{
			IRCUserMask mask = new IRCUserMask(protocol);

			switch (nickname[0])
			{
				case '~':
				case '@':
				case '%':
				case '+':
					mask.UserMode = nickname[0];
					nickname = nickname.Substring(1);
					break;
			}

			mask.Nickname = nickname;

			return mask;
		}

		public int CompareTo(object other)
		{
			if (other.GetType() != this.GetType())
				throw new ArgumentException();

			IContact othr = (IContact)other;

			if (Protocol != othr.Protocol)
				return Protocol.CompareTo(othr.Protocol);

			return Nickname.CompareTo(othr.Nickname);
		}

		public bool Equals(IContact right)
		{
			if (right == null)
				throw new ArgumentNullException("right");

			if (!(right is IRCUserMask))
				throw new ArgumentException("right must be of type IRCUserMask");

			return object.ReferenceEquals(Protocol, right.Protocol) && Username.Equals(right.Username);
		}

		public override string ToString()
		{
			return String.Format("{0}!{1}@{2}", Nickname, Username, Hostname);
		}
		public string Nickname
		{
			get;
			set;
		}
		public string Username
		{
			get;
			internal set;
		}
		public IMBuddyStatus Status
		{
			get	{
				return IMBuddyStatus.Unknown;
			}
		}
		public string Hostname
		{
			get;
			internal set;
		}
		public IMProtocol Protocol
		{
			get;
			private set;
		}
		public char UserMode
		{
			get;
			private set;
		}

		#region IContact Members

		public string Group
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IHasPresence Members

		public string StatusMessage
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
	public struct IRCUserModeChange
	{
		public IRCUserMask UserMask;
		public IRCUserModes Mode;
		public bool IsAdd;
	}
}
