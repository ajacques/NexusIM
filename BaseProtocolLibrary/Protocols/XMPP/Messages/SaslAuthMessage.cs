﻿using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal abstract class SaslAuthMessage : XmppMessage
	{
		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("auth", Namespace);
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
				throw new System.NotImplementedException();
			}
		}

		public const string Namespace = "urn:ietf:params:xml:ns:xmpp-sasl";
	}
}
