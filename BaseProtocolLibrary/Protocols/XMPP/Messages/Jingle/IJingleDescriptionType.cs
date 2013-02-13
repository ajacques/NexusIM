using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	interface IJingleDescriptionType
	{
		void WriteBody(XmlWriter writer);

		string SubNamespace
		{
			get;
		}

		string MediaType
		{
			get;
		}
	}
}
