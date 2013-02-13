using System;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	static class ClientTimeMessage
	{
		public class Request : IqMessage
		{
			protected override void WriteBody(XmlWriter writer)
			{
				writer.WriteStartElement(LocalName, Namespace);
				writer.WriteEndElement();
			}

			public override string Namespace
			{
				get {
					return ClientTimeMessage.Namespace;
				}
			}

			public override string LocalName
			{
				get {
					return "time";
				}
			}

			public override IqMessage.IqType Type
			{
				get {
					return IqType.get;
				}
			}
		}

		public class Response : IqMessage
		{
			public Response(DateTime stamp)
			{
				LocalTimestamp = stamp;
			}

			protected override void WriteBody(XmlWriter writer)
			{
				DateTime utc = LocalTimestamp.ToUniversalTime();

				writer.WriteStartElement(LocalName, Namespace);
				
				// Timezone
				writer.WriteStartElement("tzo", Namespace);
				writer.WriteString(LocalTimestamp.ToString("%K"));
				writer.WriteEndElement();

				// Timestamp
				writer.WriteStartElement("utc", Namespace);
				writer.WriteString(LocalTimestamp.ToString("o"));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}

			public override string Namespace
			{
				get {
					return ClientTimeMessage.Namespace;
				}
			}
			public override string LocalName
			{
				get {
					return "time";
				}
			}
			public override IqMessage.IqType Type
			{
				get {
					return IqType.result;
				}
			}

			public DateTime LocalTimestamp
			{
				get;
				set;
			}
		}

		public const string Namespace = "urn:xmpp:time";
	}
}
