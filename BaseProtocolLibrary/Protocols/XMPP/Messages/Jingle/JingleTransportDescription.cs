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
			if (reader.MoveToAttribute("ufrag"))
				transport.Ufrag = reader.Value;
			if (reader.MoveToAttribute("pwd"))
				transport.Password = reader.Value;

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
			writer.WriteStartAttribute("ufrag");
			writer.WriteString(Ufrag);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("pwd");
			writer.WriteString(Password);
			writer.WriteEndAttribute();

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
		/// <summary>
		/// Gets or sets the ICE-UDP username fragment. Required for correct SDP operation.
		/// </summary>
		public string Ufrag
		{
			get;
			set;
		}
		public string Password
		{
			get;
			set;
		}
		public ICollection<XmppSdpCandidate> Candidates
		{
			get;
			private set;
		}
	}
}
