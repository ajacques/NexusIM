using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class BindToResourceMessage : IqMessage
	{
		public BindToResourceMessage(string resource)
		{
			Resource = resource;
		}

		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("bind", Namespace);
			writer.WriteStartElement("resource", Namespace);
			writer.WriteString(Resource);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		protected override IqMessage.IqType Type
		{
			get {
				return IqType.set;
			}
		}

		public const string Namespace = "urn:ietf:params:xml:ns:xmpp-bind";
		public string Resource
		{
			get;
			private set;
		}
	}
}
