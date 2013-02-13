using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	class FetchVCardMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement(LocalName, Namespace);
			writer.WriteEndElement();
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
				return "vcard-temp";
			}
		}

		public override string LocalName
		{
			get {
				return "vCard";
			}
		}
	}
}
