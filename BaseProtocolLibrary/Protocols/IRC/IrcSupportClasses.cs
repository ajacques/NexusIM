using System;
using System.Collections.Generic;
using System.Linq;
using InstantMessage.Protocols.Irc;
using System.Globalization;

namespace InstantMessage.Events
{
	public class IRCModeChangeEventArgs : EventArgs
	{
		public IEnumerable<IRCUserModeChange> UserModes
		{
			get;
			internal set;
		}
		public IEnumerable<IRCChannelModeChange> ChannelModes
		{
			get;
			internal set;
		}
		public IRCUserMask Username
		{
			get;
			internal set;
		}
		public string RawMode
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
	public class IRCUserMask : IContact, IComparable, IEquatable<IContact>
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
			int exclaim = input.IndexOf("!", StringComparison.Ordinal);

			if (!input.Contains('@'))
			{
				Username = input.Substring(exclaim + 1);
				return;
			}
			int at = input.IndexOf("@", StringComparison.Ordinal);

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

		public int CompareTo(object obj)
		{
			if (obj.GetType() != this.GetType())
				throw new ArgumentException("You can only compare this object with another IRCUserMask object.", "other");

			IContact other = (IContact)obj;

			if (Protocol != other.Protocol)
				return Protocol.CompareTo(other.Protocol);

			return String.Compare(Nickname, other.Nickname, StringComparison.Ordinal);
		}

		public bool Equals(IContact other)
		{
			if (other == null)
				throw new ArgumentNullException("right");

			if (!(other is IRCUserMask))
				throw new ArgumentException("right must be of type IRCUserMask");

			return Object.ReferenceEquals(Protocol, other.Protocol) && Username.Equals(other.Username);
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{0}!{1}@{2}", Nickname, Username, Hostname);
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
		public string DisplayName
		{
			get	{
				return Username;
			}
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
		public string StatusMessage
		{
			get {
				return null;
			}
		}

		#region IContact Members

		public string Group
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
	public struct IRCChannelModeChange
	{
		public IRCChannelModes Mode;
		public bool IsAdd;
	}
}
