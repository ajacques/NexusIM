using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class IqFragmentMessage : IqMessage
	{
		public IqFragmentMessage(XmlDocument doc)
		{
			Document = doc;
		}

		protected override void WriteBody(System.Xml.XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		protected override IqMessage.IqType Type
		{
			get {
				return IqType.result;
			}
		}

		public XmlDocument Document
		{
			get;
			private set;
		}
	}
}
