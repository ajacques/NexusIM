using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IContact : IMessagable, IEquatable<IContact>, IHasPresence
	{
		IMProtocol Protocol
		{
			get;
		}
		string Username
		{
			get;
		}
		string Nickname
		{ 
			get; 
			set;
		}
		string DisplayName
		{
			get;
		}
		string Group
		{
			get;
		}
	}
}