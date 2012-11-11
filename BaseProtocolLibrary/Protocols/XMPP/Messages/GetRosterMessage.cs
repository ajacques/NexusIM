using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class GetRosterMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("query", "jabber:iq:roster");
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