using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class SaslAuthMessage : XmppMessage
	{
		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement(ElementName, Namespace);
			writer.WriteStartAttribute("mechanism");
			writer.WriteString(Mechanism);
			writer.WriteEndAttribute();
			WriteAuthBody(writer);
			writer.WriteEndElement();
		}

		protected abstract void WriteAuthBody(XmlWriter writer);
		protected abstract string Mechanism
		{
			get;
		}
		protected virtual string ElementName
		{
			get {
				return "auth";
			}
		}

		// Nested Classes
		public sealed class SuccessMessage : XmppMessage
		{
			public static MessageFactory GetMessageFactory()
			{
				return new MessageFactory(GetMessage);
			}

			private static XmppMessage GetMessage(XmlReader reader)
			{
				reader.Read();
				return new SuccessMessage();
			}

			public override void WriteMessage(XmlWriter writer)
			{
				throw new NotImplementedException();
			}
		}
		public sealed class FailureMessage : XmppMessage
		{
			public static MessageFactory GetMessageFactory()
			{
				return new MessageFactory(GetMessage);
			}

			private static XmppMessage GetMessage(XmlReader reader)
			{
				reader.Read();
				return new FailureMessage();
			}

			public override void WriteMessage(XmlWriter writer)
			{
				throw new NotImplementedException();
			}
		}

		public const string Namespace = "urn:ietf:params:xml:ns:xmpp-sasl";
	}
}
