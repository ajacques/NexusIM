using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using NAudio.Codecs;
using NAudio.Wave;

namespace InstantMessage
{
	public class RtpEndpoint
	{
		public RtpEndpoint(IPEndPoint endpoint)
		{
			mClient = new UdpClient(endpoint);
			codec = new G722Codec();
			state = new G722CodecState(48000, G722Flags.SampleRate8000);
			waveOut = new WaveOut();
			provider = new BufferedWaveProvider(new WaveFormat(48000, 1));
			waveOut.Init(provider);
			waveOut.Play();
		}

		private void OnPacket(IAsyncResult e)
		{
			IPEndPoint source = null;
			byte[] packet = mClient.EndReceive(e, ref source);
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);

			packet[8] = 50;
			packet[9] = 20;
			packet[10] = 30;

			WaveBuffer buffer = new WaveBuffer(160);

			byte[] input = new byte[160];
			Buffer.BlockCopy(packet, 12, input, 0, 160);

			codec.Decode(state, buffer.ShortBuffer, input, 100);

			provider.AddSamples(buffer.ByteBuffer, 0, buffer.ByteBufferCount);
		}
		
		public void ReadPacket()
		{
			mClient.BeginReceive(new AsyncCallback(OnPacket), null);
		}
		
		private BufferedWaveProvider provider;
		private WaveOut waveOut;
		private UdpClient mClient;
		private G722Codec codec;
		private G722CodecState state;
	}
}