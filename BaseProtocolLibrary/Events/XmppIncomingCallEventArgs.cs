using System.Net;
using System.Net.Sockets;
using InstantMessage.Protocols.XMPP;

namespace InstantMessage.Events
{
	public class XmppIncomingCallEventArgs : IncomingCallEventArgs
	{
		public XmppIncomingCallEventArgs(XmppContact contact) :base(contact)
		{
		}

		public override void AddCandidate(int priority, IPEndPoint ep)
		{
			OurTransportCandidates.Add(new XmppSdpCandidate(ProtocolType.Udp, ep.Address, ep.Port, priority, OurTransportCandidates.Count, JingleCandidateType.host));
		}
	}
}
