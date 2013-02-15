using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	internal abstract class JingleInviteMessage : JingleBaseMessage
	{
		protected JingleInviteMessage()
		{
			DescriptionNodes = new List<IJingleDescriptionType>();
			TransportNodes = new List<IJingleDescriptionType>();
		}

		protected override void WritePayload(XmlWriter writer)
		{
			writer.WriteStartElement("content", Namespace);
			WriteAttribute(writer, "creator", "initiator");
			WriteAttribute(writer, "name", "this-is-a-stub");

			foreach (var node in DescriptionNodes)
			{
				writer.WriteStartElement("description", ComputeNamespace("apps:" + node.SubNamespace));
				node.WriteBody(writer);
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

		public class AttemptMessage : JingleInviteMessage
		{
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
					return "session-initiate";
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
		public ICollection<IJingleDescriptionType> DescriptionNodes
		{
			get;
			private set;
		}
		public ICollection<IJingleDescriptionType> TransportNodes
		{
			get;
			private set;
		}
	}
}
