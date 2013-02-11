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
		private IqFragmentMessage(XmlDocument doc)
		{
			Document = doc;
		}

		public static IqFragmentMessage FromWire(XmlDocument doc)
		{
			return new IqFragmentMessage(doc)
			{
				type = IqType.result
			};
		}

		public static IqFragmentMessage CreateNew(XmlDocument root)
		{
			return new IqFragmentMessage(root)
			{
				type = IqType.set
			};
		}

		protected override void WriteBody(XmlWriter writer)
		{
			Document.DocumentElement.WriteTo(writer);
		}

		protected override IqMessage.IqType Type
		{
			get {
				return type;
			}
		}

		public XmlDocument Document
		{
			get;
			private set;
		}

		private IqType type;
	}
}
