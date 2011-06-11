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
		IEnumerable<string> Participants
		{
			get;
		}
		IMProtocol Protocol
		{
			get;
		}
		void SendMessage(string message);
		void Leave(string reason);

		event EventHandler<IMMessageEventArgs> OnMessageReceived;
		event EventHandler OnUserListReceived;
		event EventHandler OnJoin;
	}
}