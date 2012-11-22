using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
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
