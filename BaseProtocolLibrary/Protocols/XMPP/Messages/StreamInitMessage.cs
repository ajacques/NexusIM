using System;
using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages
{
	[ReadableXmppMessage(XmppNamespaces.Streams, "stream")]
	internal class StreamInitMessage : XmppMessage
	{
		public StreamInitMessage(string targetDomain)
		{
			Server = targetDomain;
		}

		private StreamInitMessage(XmlReader reader)
		{
			reader.MoveToAttribute("from");

			reader.MoveToContent();
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						ParseFeatureList(reader);
						goto exit;
					case XmlNodeType.EndElement:
						if (reader.LocalName == "features")
							goto exit;
						break;
				}
			}
			exit:
			return;
		}

		public override void WriteMessage(XmlWriter writer)
		{
			writer.WriteStartElement("stream", "stream", XmppNamespaces.Streams);
			WriteAttribute(writer, "xmlns", XmppNamespaces.JabberClient);
			WriteAttribute(writer, "version", "1.0");
			WriteAttribute(writer, "to", Server);
			writer.WriteString(String.Empty);
		}

		private void ParseFeatureList(XmlReader reader)
		{
			ICollection<StreamFeature> features = new List<StreamFeature>();
			Features = features;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "features")
					return;
				switch (reader.NamespaceURI)
				{
					case XmppNamespaces.Sasl:
						features.Add(AuthMechanismFeature.Parse(reader));
						break;
					case XmppNamespaces.Tls:
						features.Add(new TlsCapableFeature());
						break;
					case XmppNamespaces.Bind:
						features.Add(BindFeature.Parse(reader));
						break;
				}
			}
		}

		public abstract class StreamFeature
		{
			public string Namespace
			{
				get;
				protected set;
			}
		}
		public sealed class TlsCapableFeature : StreamFeature
		{
			public TlsCapableFeature()
			{
				Namespace = XmppNamespaces.Tls;
			}

			public bool Required
			{
				get;
				private set;
			}
		}
		public sealed class AuthMechanismFeature : StreamFeature
		{
			public AuthMechanismFeature()
			{
				Namespace = XmppNamespaces.Sasl;
				MechanismList = new List<string>();
			}

			public static AuthMechanismFeature Parse(XmlReader reader)
			{
				AuthMechanismFeature msg = new AuthMechanismFeature();

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.EndElement)
						break;

					if (reader.LocalName == "mechanism")
					{
						reader.Read(); // Read the body
						msg.MechanismList.Add(reader.Value);
						reader.Read(); // Read the end element
					}
				}

				return msg;
			}

			public IEnumerable<string> Mechanisms
			{
				get {
					return MechanismList;
				}
			}
			private IList<string> MechanismList
			{
				get;
				set;
			}
		}
		public sealed class BindFeature : StreamFeature
		{
			public BindFeature()
			{
				Namespace = XmppNamespaces.Bind;

			}

			public static BindFeature Parse(XmlReader reader)
			{
				BindFeature msg = new BindFeature();

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "bind")
						break;

					if (reader.LocalName == "required")
						msg.Required = true;
				}

				return msg;
			}

			public bool Required
			{
				get;
				private set;
			}
		}

		public string Server
		{
			get;
			private set;
		}
		public IEnumerable<StreamFeature> Features
		{
			get;
			private set;
		}
	}
}
