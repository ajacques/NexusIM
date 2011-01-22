using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	public interface IHasMUCRooms<T>
	{
		IChatRoom JoinChatRoom(string roomName);
		IEnumerable<T> Channels
		{
			get;
		}
	}
	public interface IHasMUCRooms : IHasMUCRooms<IChatRoom> {}
}