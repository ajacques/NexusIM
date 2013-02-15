using System;
using System.Xml;
using System.Text;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class IqMessage : XmppMessage
	{
		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("iq", XmppNamespaces.JabberClient);
			WriteAttribute(writer, "type", Type.ToString());
			WriteAttribute(writer, "id", Id);

			if (Source != null)
			{
				WriteAttribute(writer, "from", Source.ToString());
			}

			if (To != null)
			{
				WriteAttribute(writer, "to", To.ToString());
			}

			WriteBody(writer);
			writer.WriteEndElement();
		}

		protected abstract void WriteBody(XmlWriter writer);

		private class IqAckMessage : IqMessage
		{
			protected override void WriteBody(XmlWriter writer) {}
			public override string Namespace
			{
				get {
					throw new NotSupportedException();
				}
			}
			public override string LocalName
			{
				get {
					throw new NotSupportedException();
				}
			}
			public override IqType Type
			{
				get {
					return IqType.result;
				}
			}
		}

		public IqMessage Ack()
		{
			return new IqAckMessage()
			{
				Id = this.Id,
				To = this.Source
			};
		}

		public enum IqType
		{
			get,
			set,
			error,
			result
		}

		public abstract string Namespace
		{
			get;
		}
		public abstract string LocalName
		{
			get;
		}
		public Jid Source
		{
			get;
			set;
		}
		public abstract IqType Type
		{
			get;
		}
		public virtual Jid To
		{
			get;
			set;
		}
		public string Id
		{
			get;
			set;
		}
	}
}
