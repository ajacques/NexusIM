using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	internal class JingleTransportDescription : IJingleDescriptionType
	{
		public JingleTransportDescription()
		{
			Candidates = new List<Candidate>();
		}

		public void WriteBody(XmlWriter writer)
		{
			foreach (var candidate in Candidates)
			{
				candidate.Write(writer);
			}
		}

		public string SubNamespace
		{
			get {
				return "ice-udp:1";
			}
		}

		public ICollection<Candidate> Candidates
		{
			get;
			private set;
		}

		public class Candidate
		{
			public void Write(XmlWriter writer)
			{
				writer.WriteStartElement("candidate");
				WriteAttribute(writer, "ip", Address);
				WriteAttribute(writer, "protocol", ProtocolType);
				WriteAttribute(writer, "priority", Priority);
				WriteAttribute(writer, "type", Type);

				writer.WriteEndElement();
			}

			private void WriteAttribute(XmlWriter writer, string name, object value)
			{
				writer.WriteStartAttribute(name);
				writer.WriteString(value.ToString());
				writer.WriteEndAttribute();
			}

			public int Priority
			{
				get;
				set;
			}
			public ProtocolType ProtocolType
			{
				get;
				set;
			}
			public int Port
			{
				get;
				set;
			}
			public IPAddress Address
			{
				get;
				set;
			}
			public CandidateType Type
			{
				get;
				set;
			}
		}
		public enum CandidateType
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
	}
}
