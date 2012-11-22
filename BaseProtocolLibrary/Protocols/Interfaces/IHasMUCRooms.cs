using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	public interface IHasMUCRooms<T> : IHasMUCRooms
	{
		new T JoinChatRoom(string roomName);
		void JoinChatRoom(T room);
		new IEnumerable<T> Channels
		{
			get;
		}
	}
	public delegate void RoomListCompleted(IEnumerable<string> roomNames);
	/// <summary>
	/// Describes a protocol that supports instances of multi-user group chats
	/// </summary>
	public interface IHasMUCRooms : IProtocol
	{
		IChatRoom JoinChatRoom(string roomName);
		void JoinChatRoom(IChatRoom room);
		void FindRoomByName(string query, RoomListCompleted onComplete);

		IEnumerable<IChatRoom> Channels
		{
			get;
		}
	}
}