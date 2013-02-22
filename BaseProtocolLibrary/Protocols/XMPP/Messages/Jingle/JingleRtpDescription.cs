using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InstantMessage.Protocols.AudioVideo;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	class JingleRtpDescription : IJingleDescriptionType
	{
		public JingleRtpDescription()
		{
			PayloadTypes = new List<JinglePayloadType>();
		}

		public void WriteBody(XmlWriter writer)
		{
			foreach (var payload in PayloadTypes)
			{
				payload.Write(writer);
			}
		}

		public static JingleRtpDescription ParseRoot(XmlReader reader)
		{
			JingleRtpDescription rtp = new JingleRtpDescription();
			reader.Read();

			rtp.MediaType = reader.GetAttribute("media");

			while (reader.Read())
			{
				if (reader.LocalName == "payload-type")
				{
					rtp.PayloadTypes.Add(JinglePayloadType.Parse(reader));
				}
			}

			return rtp;
		}

		public string SubNamespace
		{
			get {
				return "rtp:1";
			}
		}

		public ICollection<JinglePayloadType> PayloadTypes
		{
			get;
			private set;
		}

		public class JinglePayloadType : SdpPayloadType
		{
			public JinglePayloadType(int id, string name)
				: base(id, name)
			{
			}

			public void Write(XmlWriter writer)
			{
				writer.WriteStartElement("payload-type");
				writer.WriteStartAttribute("id");
				writer.WriteString(XmlConvert.ToString(Id));
				writer.WriteEndAttribute();

				writer.WriteStartAttribute("name");
				writer.WriteString(Name);
				writer.WriteEndAttribute();

				if (ClockRate.HasValue)
				{
					writer.WriteStartAttribute("clockrate");
					writer.WriteString(ClockRate.Value.ToString());
					writer.WriteEndAttribute();
				}

				if (Channels >= 2)
				{
					writer.WriteStartAttribute("channels");
					writer.WriteString(Channels.ToString());
					writer.WriteEndAttribute();
				}

				writer.WriteEndElement();
			}

			public static JinglePayloadType Parse(XmlReader reader)
			{
				int id = Int32.Parse(reader.GetAttribute("id"));
				string name = null;

				if (reader.MoveToAttribute("name"))
					name = reader.Value;

				JinglePayloadType payload = new JinglePayloadType(id, name);

				if (reader.MoveToAttribute("channels"))
					payload.Channels = Int32.Parse(reader.Value);
				if (reader.MoveToAttribute("clockrate"))
					payload.ClockRate = Int32.Parse(reader.Value);

				reader.MoveToElement();
				reader.Skip();

				return payload;
			}
		}

		public string MediaType
		{
			get;
			set;
		}
	}
}
