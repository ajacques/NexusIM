using System;

namespace InstantMessage.Protocols.Irc
{
	[Flags]
	public enum IrcUserModes
	{
		None = 0,
		Invisible = 1,
		Protected = 2, // a
		Operator = 4, // o
		Voice = 8, // v
		Banned = 16, // b
		Unknown = 32
	}
	[Flags]
	public enum IrcChannelModes
	{
		None,
		Moderated,
		InviteOnly,
	}
}
