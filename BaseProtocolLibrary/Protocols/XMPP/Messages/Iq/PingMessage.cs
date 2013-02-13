using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class PingMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement(LocalName, Namespace);
			writer.WriteEndElement();
		}

		public override IqType Type
		{
			get {
				return IqType.get;
			}
		}

		public override string Namespace
		{
			get {
				return XmppNamespaces.Ping;
			}
		}

		public override string LocalName
		{
			get {
				return "ping";
			}
		}
	}
}
