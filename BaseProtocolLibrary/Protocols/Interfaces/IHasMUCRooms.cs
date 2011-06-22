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
	public interface IHasMUCRooms
	{
		IChatRoom JoinChatRoom(string roomName);
		void JoinChatRoom(IChatRoom room);

		IEnumerable<IChatRoom> Channels
		{
			get;
		}
	}
}