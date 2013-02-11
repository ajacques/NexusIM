using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.XMPP
{
	internal class RosterItem
	{
		public RosterItem(Jid jid)
		{
			Jid = jid;
		}

		public Jid Jid
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			set;
		}
	}
}
