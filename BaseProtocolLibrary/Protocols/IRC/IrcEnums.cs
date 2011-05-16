using System;

namespace InstantMessage.Protocols.Irc
{
	/// <summary>
	/// Contains modes that can applied to any user in a channel or a server.
	/// </summary>
	[Flags]
	public enum IRCUserModes
	{
		None = 0,
		/// <summary>
		/// User is invisible to all users unless they know the exact nickname or are on the same channel.
		/// </summary>
		Invisible = 1,
		Protected = 2, // a
		/// <summary>
		/// User is a channel operator. This means the user can kick and ban users from the specified channel.
		/// </summary>
		Operator = 4, // o
		/// <summary>
		/// User can speak in moderated channels (+m)
		/// </summary>
		Voice = 8, // v
		Banned = 16, // b
		HalfOperator = 32, // h
		Unknown = 64
	}
	[Flags]
	public enum IRCChannelModes
	{
		None,
		Moderated,
		InviteOnly,
	}

	public enum IRCJoinFailedReason
	{
		InviteOnly,
		Banned
	}
}
