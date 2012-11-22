using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal class IqResponseMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			throw new NotSupportedException();
		}

		protected override IqMessage.IqType Type
		{
			get {
				return IqMessage.IqType.result;
			}
		}
	}
}
