using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace InstantMessage
{
	public class RtcpEndpoint
	{
		public RtcpEndpoint(IPEndPoint endpoint)
		{
			mClient = new UdpClient(endpoint);
		}

		private void OnPacket(IAsyncResult e)
		{
			IPEndPoint source = null;
			byte[] packet = mClient.EndReceive(e, ref source);
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);

			switch (packet[1]) // Check the Packet Type
			{
				case 0xca:
					Trace.WriteLine("RTCP Source Description");

					packet[4] = 50;
					packet[5] = 20;
					packet[6] = 30;

					packet[12] = 65;
					packet[13] = 65;

					mClient.Send(packet, packet.Length, source);

					break;
			}
		}

		public void ReadPacket()
		{
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);
		}

		private UdpClient mClient;
	}
}