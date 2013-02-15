using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class StartTlsMessage : XmppMessage
	{
		[ReadableXmppMessage(XmppNamespaces.Tls, "proceed")]
		public class ProceedMessage : XmppMessage
		{
			private ProceedMessage(XmlReader reader)
			{
			}

			public override void WriteMessage(XmlWriter writer)
			{
				throw new NotImplementedException();
			}
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("starttls", XmppNamespaces.Tls);
			writer.WriteEndElement();
		}
	}
}