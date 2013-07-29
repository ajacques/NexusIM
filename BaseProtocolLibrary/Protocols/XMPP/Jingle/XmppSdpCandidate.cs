using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InstantMessage.Protocols.AudioVideo;
using System.Diagnostics;

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

	[DebuggerDisplay("Candidate: {ProtocolType}:[{EndPoint.Address}:{EndPoint.Port}] - Priority: {Priority}")]
	public class XmppSdpCandidate : SdpTransportCandidate, IComparable<XmppSdpCandidate>
	{
		public XmppSdpCandidate(ProtocolType type, IPEndPoint ep, int priority, int id, int component, JingleCandidateType ctype) : base(type, ep, priority, component)
		{
			Type = ctype;
			Id = id;
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("candidate");
			WriteAttribute(writer, "ip", EndPoint.Address);
			WriteAttribute(writer, "protocol", ProtocolType.ToString().ToLower());
			WriteAttribute(writer, "priority", Priority);
			WriteAttribute(writer, "type", Type);
			WriteAttribute(writer, "component", Component);
			WriteAttribute(writer, "foundation", 1);
			WriteAttribute(writer, "generation", 0);
			WriteAttribute(writer, "network", 0);
			WriteAttribute(writer, "port", EndPoint.Port);
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
			int component = Int32.Parse(reader.GetAttribute("component"));

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

			XmppSdpCandidate cand = new XmppSdpCandidate(ptype, new IPEndPoint(address, port), priority, id, component, ctype);

			cand.Type = (JingleCandidateType)Enum.Parse(typeof(JingleCandidateType), reader.GetAttribute("type"));

			return cand;
		}

		public int CompareTo(XmppSdpCandidate other)
		{
			return Priority.CompareTo(other.Priority) + (Priority == other.Priority ? Id.CompareTo(other.Id) : 0);
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
