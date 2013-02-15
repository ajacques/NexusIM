using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Message
{
	internal abstract class MessageMessage : XmppMessage
	{
		protected MessageMessage(Jid to)
		{
			To = to;
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("message", RootNamespace);
			WriteAttribute(writer, "type", Type);
			WriteAttribute(writer, "id", Id);
			WriteAttribute(writer, "to", To);

			WriteBody(writer);

			writer.WriteEndElement();
		}

		protected abstract void WriteBody(XmlWriter writer);

		protected virtual string Type
		{
			get {
				return "chat";
			}
		}
		public Jid To
		{
			get;
			private set;
		}
		public string Id
		{
			get;
			set;
		}
		public const string RootNamespace = XmppNamespaces.JabberClient;
	}
}
