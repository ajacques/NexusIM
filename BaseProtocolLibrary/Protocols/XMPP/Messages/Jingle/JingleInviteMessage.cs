using System;
using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	[IqMessageBody(XmppNamespaces.Jingle, "jingle")]
	internal abstract class JingleInviteMessage : JingleBaseMessage
	{
		protected JingleInviteMessage()
		{
			DescriptionNodes = new Dictionary<string, IJingleDescriptionType>();
			TransportNodes = new List<IJingleDescriptionType>();
		}

		protected override void WritePayload(XmlWriter writer)
		{
			writer.WriteStartElement("content", Namespace);
			WriteAttribute(writer, "creator", "initiator");
			WriteAttribute(writer, "name", "audio");

			foreach (var node in DescriptionNodes)
			{
				writer.WriteStartElement("description", ComputeNamespace("apps:" + node.Value.SubNamespace));
				WriteAttribute(writer, "media", node.Key);

				node.Value.WriteBody(writer);
				writer.WriteEndElement();
			}

			foreach (var node in TransportNodes)
			{
				writer.WriteStartElement("transport", ComputeNamespace("transports:" + node.SubNamespace));
				node.WriteBody(writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement(); // </content>
		}

		[XmppMessageFactoryEntry]
		public static XmppMessage ParseMessage(XmlReader reader)
		{
			string action = reader.GetAttribute("action");
			string sid = reader.GetAttribute("sid");
			
			JingleInviteMessage msg;
			if (action == "session-initiate")
			{
				msg = AttemptMessage.ParseMessage(reader);
			} else {
				throw new NotImplementedException();
			}

			msg.SessionId = sid;

			return msg;
		}

		public class AttemptMessage : JingleInviteMessage
		{
			public new static AttemptMessage ParseMessage(XmlReader reader)
			{
				AttemptMessage msg = new AttemptMessage();

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "description")
					{
						XmlReader description = reader.ReadSubtree();

						JingleRtpDescription rtp = JingleRtpDescription.ParseRoot(description);

						msg.DescriptionNodes.Add(rtp.MediaType, rtp);
					} else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "transport") {
						XmlReader subtree = reader.ReadSubtree();

						msg.TransportNodes.Add(JingleTransportDescription.Parse(subtree));
					}
				}

				return msg;
			}

			protected override string Action
			{
				get	{
					return "session-initiate";
				}
			}
			protected override bool IsInitiating
			{
				get {
					return true;
				}
			}
		}
		public class AcceptMessage : JingleInviteMessage
		{
			protected override string Action
			{
				get {
					return "transport-info";
				}
			}

			protected override bool IsInitiating
			{
				get {
					return false;
				}
			}
		}

		protected override string SubNamespace
		{
			get	{
				return "1";
			}
		}
		public IDictionary<string, IJingleDescriptionType> DescriptionNodes
		{
			get;
			private set;
		}
		public IList<IJingleDescriptionType> TransportNodes
		{
			get;
			private set;
		}
	}
}
