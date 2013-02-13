using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	class JingleRtpDescription : IJingleDescriptionType
	{
		public JingleRtpDescription()
		{
			PayloadTypes = new List<PayloadType>();
		}

		public void WriteBody(XmlWriter writer)
		{
			foreach (var payload in PayloadTypes)
			{
				payload.Write(writer);
			}
		}

		public string SubNamespace
		{
			get {
				return "rtp:1";
			}
		}

		public ICollection<PayloadType> PayloadTypes
		{
			get;
			private set;
		}

		public class PayloadType
		{
			public void Write(XmlWriter writer)
			{
				writer.WriteStartElement("payload-type");
				writer.WriteStartAttribute("id");
				writer.WriteString(XmlConvert.ToString(Id));
				writer.WriteEndAttribute();

				writer.WriteStartElement("name");
				writer.WriteString(Name);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			public int Id
			{
				get;
				set;
			}
			public string Name
			{
				get;
				set;
			}
		}

		public string MediaType
		{
			get {
				return null;
			}
		}
	}
}
