using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.AudioVideo
{
	public abstract class SdpTransportCandidates
	{
		public SdpTransportCandidates(ProtocolType protocolType, IPAddress address, int port, int priority)
		{
			ProtocolType = protocolType;
			Address = address;
			Port = port;
			Priority = priority;
		}

		public ProtocolType ProtocolType
		{
			get;
			private set;
		}
		public IPAddress Address
		{
			get;
			private set;
		}
		public int Port
		{
			get;
			private set;
		}
		public int Priority
		{
			get;
			private set;
		}
	}
}
