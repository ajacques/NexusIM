using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	[ReadableXmppMessage(XmppNamespaces.JabberClient, "presence")]
	internal class PresenceMessage : XmppMessage
	{
		public PresenceMessage()
		{
		}
		public PresenceMessage(XmlReader reader)
		{
			reader.MoveToAttribute("from");
			From = Jid.Parse(reader.Value);

			if (reader.MoveToAttribute("SHOW"))
			{
				reader.Read();
				Show = reader.Value;
			}

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						switch (reader.LocalName)
						{
							case "priority":
								reader.Read(); // Content
								Priority = reader.ReadContentAsInt();
								break;
							case "c":

								break;
						}
						break;
				}
			}
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("presence");
			
			writer.WriteStartElement("priority");
			writer.WriteValue(Priority);
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		private static XmppMessage GetMessage(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public int Priority
		{
			get;
			set;
		}
		public string Show
		{
			get;
			set;
		}
		public Jid From
		{
			get;
			set;
		}
	}
}
