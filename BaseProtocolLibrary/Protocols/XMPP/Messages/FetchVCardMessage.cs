using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	class FetchVCardMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("vCard", "vcard-temp");
			writer.WriteEndElement();
		}

		protected override IqMessage.IqType Type
		{
			get {
				return IqType.get;
			}
		}
	}
}
