using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IHasPresence
	{
		IMBuddyStatus Status
		{
			get;
		}
		string StatusMessage
		{
			get;
		}
	}
}
