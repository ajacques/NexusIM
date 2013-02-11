using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class BindToResourceMessage : IqMessage
	{
		public BindToResourceMessage(string resource)
		{
			Resource = resource;
		}

		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("bind", XmppNamespaces.Bind);
			writer.WriteStartElement("resource", XmppNamespaces.Bind);
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

		[IqMessageBody(XmppNamespaces.Bind, "bind")]
		public class ResponseMessage : IqResponseMessage
		{
			[XmppMessageFactoryEntry]
			private static XmppMessage ParseMessage(XmlReader reader)
			{
				while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "bind"))
					reader.Read();
				return new ResponseMessage();
			}
		}

		public string Resource
		{
			get;
			private set;
		}
	}
}
