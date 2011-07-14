using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace InstantMessage
{
	class RtcpEndpoint
	{
		public RtcpEndpoint(IPEndPoint endpoint)
		{
			mClient = new UdpClient(endpoint);
		}

		private UdpClient mClient;
	}
}