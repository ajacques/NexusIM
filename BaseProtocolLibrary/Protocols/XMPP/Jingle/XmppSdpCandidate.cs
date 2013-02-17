using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InstantMessage.Protocols.AudioVideo;

namespace InstantMessage.Protocols.XMPP
{
	public enum JingleCandidateType
	{
		host,
		/// <summary>
		/// Peer reflexive
		/// </summary>
		prflx,
		relay,
		/// <summary>
		/// Server reflexive
		/// </summary>
		srflx
	}

	public class XmppSdpCandidate : SdpTransportCandidates, IComparable<XmppSdpCandidate>
	{
		public XmppSdpCandidate(ProtocolType type, IPAddress address, int port, int priority, int id, JingleCandidateType ctype)
			: base(type, address, port, priority)
		{
			Type = ctype;
			Id = id;
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("candidate");
			WriteAttribute(writer, "ip", Address);
			WriteAttribute(writer, "protocol", ProtocolType.ToString().ToLower());
			WriteAttribute(writer, "priority", Priority);
			WriteAttribute(writer, "type", Type);
			WriteAttribute(writer, "component", 1);
			WriteAttribute(writer, "foundation", 1);
			WriteAttribute(writer, "generation", 0);
			WriteAttribute(writer, "network", 0);
			WriteAttribute(writer, "port", Port);
			WriteAttribute(writer, "id", Id);

			writer.WriteEndElement();
		}

		private void WriteAttribute(XmlWriter writer, string name, object value)
		{
			writer.WriteStartAttribute(name);
			writer.WriteString(value.ToString());
			writer.WriteEndAttribute();
		}

		public static XmppSdpCandidate Parse(XmlReader reader)
		{
			int priority = Int32.Parse(reader.GetAttribute("priority"));
			int port = Int32.Parse(reader.GetAttribute("port"));
			IPAddress address = IPAddress.Parse(reader.GetAttribute("ip"));

			ProtocolType ptype = ProtocolType.Unspecified;
			int id = Int32.Parse(reader.GetAttribute("id"));

			JingleCandidateType ctype = (JingleCandidateType)Enum.Parse(typeof(JingleCandidateType), reader.GetAttribute("type"));

			switch (reader.GetAttribute("protocol"))
			{
				case "udp":
					ptype = ProtocolType.Udp;
					break;
				case "tcp":
					ptype = ProtocolType.Tcp;
					break;
			}

			XmppSdpCandidate cand = new XmppSdpCandidate(ptype, address, port, priority, id, ctype);

			cand.Type = (JingleCandidateType)Enum.Parse(typeof(JingleCandidateType), reader.GetAttribute("type"));

			return cand;
		}

		public int CompareTo(XmppSdpCandidate other)
		{
			return Priority.CompareTo(other.Priority) + Priority == other.Priority ? Id.CompareTo(other.Id) : 0;
		}

		public override string ToString()
		{
			return String.Format("Candidate: {0}:[{1}:{2}] - Priority: {3}", ProtocolType, Address, Port, Priority);
		}

		public JingleCandidateType Type
		{
			get;
			set;
		}
		public int Id
		{
			get;
			private set;
		}
	}
}
