using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	public interface IHasMUCRooms<T> : IHasMUCRooms
	{
		T JoinChatRoom(string roomName);
		void JoinChatRoom(T room);
		IEnumerable<T> Channels
		{
			get;
		}
	}
	public delegate void FindRoomCompleted(IEnumerable<string> roomNames);
	/// <summary>
	/// Describes a protocol that supports instances of multi-user group chats
	/// </summary>
	public interface IHasMUCRooms : IProtocol
	{
		IChatRoom JoinChatRoom(string roomName);
		void JoinChatRoom(IChatRoom room);
		void FindRoomByName(string query, FindRoomCompleted onComplete);

		IEnumerable<IChatRoom> Channels
		{
			get;
		}
	}
}