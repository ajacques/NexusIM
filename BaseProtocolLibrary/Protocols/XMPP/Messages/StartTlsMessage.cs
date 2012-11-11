using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class StartTlsMessage : XmppMessage
	{
		public class ProceedMessage : XmppMessage
		{
			public override void WriteMessage(XmlWriter writer)
			{
				throw new NotImplementedException();
			}

			public static MessageFactory GetMessageFactory()
			{
				return new MessageFactory(ProcessMessage);
			}

			private static XmppMessage ProcessMessage(XmlReader reader)
			{
				return new ProceedMessage();
			}
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("starttls", "urn:ietf:params:xml:ns:xmpp-tls");
			writer.WriteEndElement();
		}
	}
}