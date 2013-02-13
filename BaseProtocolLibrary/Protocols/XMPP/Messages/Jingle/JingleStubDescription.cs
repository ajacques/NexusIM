using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	class JingleStubDescription : IJingleDescriptionType
	{
		public void WriteBody(XmlWriter writer)
		{
			
		}

		public string SubNamespace
		{
			get {
				return "stub:0";
			}
		}
	}
}
