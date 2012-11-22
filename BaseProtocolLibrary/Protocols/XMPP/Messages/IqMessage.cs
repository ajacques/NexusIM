using System;
using System.Xml;
using System.Text;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class IqMessage : XmppMessage
	{
		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("iq");
			//WriteAttribute(writer, "from", Source.ToString());
			WriteAttribute(writer, "type", Type.ToString());
			WriteAttribute(writer, "id", Id);

			WriteBody(writer);
			writer.WriteEndElement();
		}

		protected abstract void WriteBody(XmlWriter writer);

		protected enum IqType
		{
			get,
			set,
			error,
			result
		}

		public Jid Source
		{
			get;
			set;
		}
		protected abstract IqType Type
		{
			get;
		}
		public string Id
		{
			get;
			set;
		}
	}
}
