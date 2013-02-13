using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class UnknownFragmentMessage : XmppMessage
	{
		public UnknownFragmentMessage()
		{
			Document = new XmlDocument();
			Fragment = Document.CreateDocumentFragment();
		}

		public override void WriteMessage(XmlWriter writer)
		{
			Fragment.WriteContentTo(writer);
		}

		public static MessageFactory GetMessageFactory()
		{
			return ParseMessage;
		}

		private static XmppMessage ParseMessage(XmlReader reader)
		{
			UnknownFragmentMessage msg = new UnknownFragmentMessage();

			msg.Document.Load(reader);

			return msg;
		}

		public XmlDocumentFragment Fragment
		{
			get;
			private set;
		}

		public XmlDocument Document
		{
			get;
			private set;
		}
	}
}
