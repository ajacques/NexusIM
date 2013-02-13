using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class IqResponseMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			throw new NotSupportedException();
		}

		public override IqMessage.IqType Type
		{
			get {
				return IqMessage.IqType.result;
			}
		}
	}
}
