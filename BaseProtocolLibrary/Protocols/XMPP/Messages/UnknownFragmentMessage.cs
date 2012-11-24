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
			Stack<XmlNode> nodeStack = new Stack<XmlNode>();
			nodeStack.Push(msg.Document);

			do {
				XmlNode node = null;
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						node = msg.Document.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						break;
					case XmlNodeType.Text:
						node = msg.Document.CreateTextNode(reader.Value);
						break;
					case XmlNodeType.EndElement:
						nodeStack.Pop();
						break;
					case XmlNodeType.Attribute:
						node = msg.Document.CreateAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						break;
				}
				if (node != null)
				{
					nodeStack.Peek().AppendChild(node);
				}
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						nodeStack.Push(node);
						break;
				}
				reader.Read();
			} while (nodeStack.Count >= 2);

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
