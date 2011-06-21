using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols;

namespace InstantMessage
{
	class ChatRoomCollection<T> : AdvancedDictionary<string, T> where T : IChatRoom
	{
		public void Add(T room)
		{
			base.Add(room.Name, room);
		}
	}
}
