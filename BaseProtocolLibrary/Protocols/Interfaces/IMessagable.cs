using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	/// <summary>
	/// Represents an object that can be messages ie a contact or a chat room
	/// </summary>
	public interface IMessagable
	{
		void SendMessage(string message);
	}
}
