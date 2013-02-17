using System;
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
			ParseMessage(reader, this);
		}

		[XmppMessageFactoryEntry]
		public static PresenceMessage ParseMessage(XmlReader reader)
		{
			PresenceMessage msg = new PresenceMessage();
			ParseMessage(reader, msg);

			return msg;
		}

		private static void ParseMessage(XmlReader reader, PresenceMessage message)
		{
			reader.MoveToAttribute("from");
			message.From = Jid.Parse(reader.Value);

			if (reader.MoveToAttribute("SHOW"))
			{
				reader.Read();
				message.Show = reader.Value;
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
								message.Priority = reader.ReadContentAsInt();
								break;
						}
						break;
				}
			}
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("presence", XmppNamespaces.JabberClient);
			
			writer.WriteStartElement("priority");
			writer.WriteValue(Priority);
			writer.WriteEndElement();

			writer.WriteStartElement("show");
			writer.WriteValue(Show);
			writer.WriteEndElement();

			if (!String.IsNullOrEmpty(Status))
			{
				writer.WriteStartElement("status");
				writer.WriteValue(Status);
				writer.WriteEndElement();
			}

			writer.WriteStartElement("c", "http://jabber.org/protocol/caps");
			WriteAttribute(writer, "node", "http://nexus-im.com");
			WriteAttribute(writer, "ver", "q4J68Jgr/wVkZIcA9TDqNZbgq7s=");
			WriteAttribute(writer, "hash", "sha-1");
			//WriteAttribute(writer, "ext", "voice-v1 video-v1 camera-v1");
			writer.WriteEndElement();

			writer.WriteEndElement();
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
		public string Status
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