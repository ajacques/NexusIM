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

		public static IqFragmentMessage FromWire(string msgid, Jid from, XmlDocument doc, IqType msgtype)
		{
			return new IqFragmentMessage(doc)
			{
				Id = msgid,
				Source = from,
				type = msgtype
			};
		}

		public static IqFragmentMessage CreateNew(XmlDocument root)
		{
			return new IqFragmentMessage(root)
			{
				type = IqType.get
			};
		}

		public static IqFragmentMessage CreateInReplyTo(XmlDocument root, IqMessage original)
		{
			return new IqFragmentMessage(root)
			{
				type = IqType.result,
				Id = original.Id,
				to = original.Source
			};
		}

		protected override void WriteBody(XmlWriter writer)
		{
			Document.DocumentElement.WriteTo(writer);
		}

		public override IqMessage.IqType Type
		{
			get {
				return type;
			}
		}

		public override Jid To
		{
			get {
				return to;
			}
		}

		public XmlDocument Document
		{
			get;
			private set;
		}

		public override string Namespace
		{
			get {
				return Document.DocumentElement.NamespaceURI;
			}
		}

		public override string LocalName
		{
			get {
				return Document.DocumentElement.LocalName;
			}
		}

		private IqType type;
		private Jid to;
	}
}
