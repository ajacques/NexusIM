using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using NAudio.Codecs;

namespace InstantMessage
{
	public class RtpEndpoint
	{
		public RtpEndpoint(IPEndPoint endpoint)
		{
			mClient = new UdpClient(endpoint);
			G722Codec codec = new G722Codec();
		}

		private void OnPacket(IAsyncResult e)
		{
			IPEndPoint source = null;
			byte[] packet = mClient.EndReceive(e, ref source);
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);

			packet[8] = 50;
			packet[9] = 20;
			packet[10] = 30;

			mClient.Send(packet, packet.Length, source);
		}
		
		public void ReadPacket()
		{
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);
		}

		private UdpClient mClient;
	}
}