using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Events;

namespace InstantMessage.Protocols
{
	public interface IChatRoom : IMessagable
	{
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
		void SendMessage(string message);
		void Leave(string reason);

		event EventHandler<IMMessageEventArgs> OnMessageReceived;
		event EventHandler OnUserListReceived;
		event EventHandler OnJoin;
	}
}