using System;
using System.Collections.Generic;
using System.Linq;

namespace InstantMessage.Protocols.Irc
{
	public class IRCModeChangeEventArgs : EventArgs
	{
		public IEnumerable<IrcUserModeChange> UserModes
		{
			get;
			internal set;
		}
		public IGrouping<string, IrcUserModes> ChannelModes
		{
			get;
			internal set;
		}
	}
	public class IrcUserMask : IContact
	{
		public IrcUserMask(IRCProtocol protocol)
		{
			Protocol = protocol;
		}
		public IrcUserMask(IRCProtocol protocol, string input)
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
		public void SendMessage(string message)
		{
		}

		public override string ToString()
		{
			return String.Format("{0}!{1}@{2}", Nickname, Username, Hostname);
		}
		public string Nickname
		{
			get;
			internal set;
		}
		public string Username
		{
			get;
			internal set;
		}
		public string Hostname
		{
			get;
			internal set;
		}
		public IRCProtocol Protocol
		{
			get;
			private set;
		}
	}
	public struct IrcUserModeChange
	{
		public IrcUserMask UserMask;
		public IrcUserModes Mode;
		public bool IsAdd;
	}
}
