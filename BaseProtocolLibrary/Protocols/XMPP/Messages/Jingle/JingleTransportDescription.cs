using System.Collections.Generic;
using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	internal class JingleTransportDescription : IJingleDescriptionType
	{
		public JingleTransportDescription()
		{
			Candidates = new AdvancedSet<XmppSdpCandidate>();
		}

		public static JingleTransportDescription Parse(XmlReader reader)
		{
			JingleTransportDescription transport = new JingleTransportDescription();
			reader.Read();

			while (reader.Read())
			{
				if (reader.LocalName == "candidate")
				{
					transport.Candidates.Add(XmppSdpCandidate.Parse(reader));
				}
			}

			return transport;
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

		public ICollection<XmppSdpCandidate> Candidates
		{
			get;
			private set;
		}
	}
}
