using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	internal abstract class JingleInviteMessage : JingleBaseMessage
	{
		protected JingleInviteMessage()
		{
			DescriptionNodes = new List<IJingleDescriptionType>();
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

			writer.WriteStartElement("transport", ComputeNamespace("transports:stub:0"));
			writer.WriteEndElement();

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
	}
}
