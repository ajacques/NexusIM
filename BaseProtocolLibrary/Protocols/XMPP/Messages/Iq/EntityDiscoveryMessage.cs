using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class EntityDiscoveryMessage : IqMessage
	{
		public EntityDiscoveryMessage(string clientNs, string version)
		{
			ClientNamespace = clientNs;
			Version = version;
		}

		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement(LocalName, Namespace);
			WriteAttribute(writer, "node", String.Format("{0}#{1}", ClientNamespace, Version));

			WritePayload(writer);

			writer.WriteEndElement();
		}

		protected abstract void WritePayload(XmlWriter writer);

		[IqMessageBody(XmppNamespaces.DiscoInfo, "query")]
		public sealed class Request : EntityDiscoveryMessage
		{
			public Request(string clientNs, string version) : base(clientNs, version)
			{
			}

			[XmppMessageFactoryEntry]
			public static XmppMessage ParseMessage(XmlReader reader)
			{
				if (reader.MoveToAttribute("node"))
				{
					string[] node = reader.Value.Split('#');

					return new Request(node[0], node[1]);
				}
				return new Request(null, null);
			}

			public Response Respond()
			{
				return new Response(ClientNamespace, Version)
				{
					To = this.Source,
					Id = this.Id
				};
			}

			protected override void WritePayload(XmlWriter writer)
			{
				
			}

			public override IqMessage.IqType Type
			{
				get {
					return IqType.get;
				}
			}
		}
		public sealed class Response : EntityDiscoveryMessage
		{
			public Response(string clientNs, string version) : base(clientNs, version)
			{
				Features = new List<XmppFeature>();
			}

			protected override void WritePayload(XmlWriter writer)
			{
				foreach (var feature in Features)
				{
					feature.WriteXml(writer);
				}
			}

			public override IqMessage.IqType Type
			{
				get {
					return IqType.result;
				}
			}
			public ICollection<XmppFeature> Features
			{
				get;
				private set;
			}
		}

		public class XmppFeature
		{
			public XmppFeature(string ns)
			{
				Namespace = ns;
			}

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteStartElement("feature", XmppNamespaces.DiscoInfo);
				writer.WriteStartAttribute("var");
				writer.WriteString(Namespace);

				WriteBody(writer);

				writer.WriteEndElement();
			}

			protected virtual void WriteBody(XmlWriter writer)
			{
			}

			public string Namespace
			{
				get;
				private set;
			}
		}

		public override string Namespace
		{
			get {
				return XmppNamespaces.DiscoInfo;
			}
		}
		public override string LocalName
		{
			get {
				return "query";
			}
		}
		public string ClientNamespace
		{
			get;
			private set;
		}
		public string Version
		{
			get;
			private set;
		}
	}
}
