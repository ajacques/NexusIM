using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
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