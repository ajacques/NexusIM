using System;
using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class GetRosterMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement(LocalName, Namespace);
			writer.WriteEndElement();
		}

		[IqMessageBody(XmppNamespaces.Iq + "roster", "query")]
		public sealed class RosterListResponse : IqMessage
		{
			[XmppMessageFactoryEntry]
			private static XmppMessage CreateInstance(XmlReader reader)
			{
				RosterListResponse msg = new RosterListResponse();
				ICollection<RosterItem> rosteritems = new List<RosterItem>();
				msg.RosterItems = rosteritems;

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "query")
					{
						break;
					}

					reader.MoveToAttribute("jid");
					RosterItem item = new RosterItem(Jid.Parse(reader.Value));
					rosteritems.Add(item);

					if (reader.MoveToAttribute("name"))
					{
						item.Name = reader.Value;
					}

					while (reader.Read())
					{
						if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "item")
						{
							break;
						}
					}
				}

				return msg;
			}

			protected override void WriteBody(XmlWriter writer)
			{
				throw new NotImplementedException();
			}

			public override IqType Type
			{
				get {
					return IqType.result;
				}
			}

			public IEnumerable<RosterItem> RosterItems
			{
				get;
				private set;
			}

			public override string Namespace
			{
				get {
					return XmppNamespaces.Iq + "roster";
				}
			}

			public override string LocalName
			{
				get {
					return "query";
				}
			}
		}

		public override IqMessage.IqType Type
		{
			get {
				return IqType.get;
			}
		}

		public override string Namespace
		{
			get {
				return "jabber:iq:roster";
			}
		}

		public override string LocalName
		{
			get {
				return "query";
			}
		}
	}
}