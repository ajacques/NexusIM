using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Message
{
	internal class ChatMessageMessage : MessageMessage
	{
		public ChatMessageMessage(Jid target, string body) : base(target)
		{
			Body = body;
		}

		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("body", RootNamespace);
			writer.WriteValue(Body);
			writer.WriteEndElement();
		}

		public string Body
		{
			get;
			private set;
		}
	}
}
