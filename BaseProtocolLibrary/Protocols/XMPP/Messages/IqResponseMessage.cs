using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
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
