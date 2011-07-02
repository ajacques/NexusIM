using System;
using System.Collections.Generic;
using InstantMessage.Events;

namespace InstantMessage.Protocols
{
	/// <summary>
	/// Represents a system that allows multiple people to chat.
	/// </summary>
	public interface IChatRoom : IMessagable
	{
		void InviteUser(string username, string message);
		void Leave(string reason);

		/// <summary>
		/// Gets the name of chat room
		/// </summary>
		string Name
		{
			get;
		}
		bool Joined
		{
			get;
		}
		IEnumerable<IContact> Participants
		{
			get;
		}
		IMProtocol Protocol
		{
			get;
		}

		event EventHandler<IMMessageEventArgs> OnMessageReceived;
		event EventHandler<IMChatRoomGenericEventArgs> OnUserJoin;
		event EventHandler OnUserListReceived;
		event EventHandler OnJoin;
	}
}