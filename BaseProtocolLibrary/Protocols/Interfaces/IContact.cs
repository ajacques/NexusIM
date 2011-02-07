using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IContact : IMessagable, IEquatable<IContact>
	{
		IMProtocol Protocol
		{
			get;
		}
		string Username
		{
			get;
		}
		IMBuddyStatus Status
		{
			get;
		}
		string Nickname
		{ 
			get; 
			set;
		}
		string Group
		{
			get;
		}
	}
}