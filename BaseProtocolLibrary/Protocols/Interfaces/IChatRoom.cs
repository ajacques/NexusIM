using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Events;

namespace InstantMessage.Protocols
{
	/// <summary>
	/// Represents a system that allows multiple people to chat.
	/// </summary>
	public interface IChatRoom : IMessagable
	{
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
		/// <summary>
		/// Sends a message to the chat room. The message will be visible to all users in the chat room.
		/// </summary>
		void SendMessage(string message);
		void Leave(string reason);

		event EventHandler<IMMessageEventArgs> OnMessageReceived;
		event EventHandler<IMChatRoomGenericEventArgs> OnUserJoin;
		event EventHandler OnUserListReceived;
		event EventHandler OnJoin;
	}
}