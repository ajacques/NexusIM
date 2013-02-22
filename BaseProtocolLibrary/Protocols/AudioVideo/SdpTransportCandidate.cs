using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.AudioVideo
{
	public abstract class SdpTransportCandidate
	{
		public SdpTransportCandidate(ProtocolType protocolType, IPEndPoint ep, int priority, int component)
		{
			ProtocolType = protocolType;
			EndPoint = ep;
			Priority = priority;
			Component = component;
		}

		public ProtocolType ProtocolType
		{
			get;
			private set;
		}
		public IPEndPoint EndPoint
		{
			get;
			private set;
		}
		public int Priority
		{
			get;
			private set;
		}
		public int Component
		{
			get;
			private set;
		}
	}
}
