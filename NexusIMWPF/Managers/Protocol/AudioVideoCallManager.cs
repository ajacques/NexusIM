using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using InstantMessage;
using InstantMessage.Events;
using InstantMessage.Misc;
using InstantMessage.Protocols;
using InstantMessage.Protocols.AudioVideo;

namespace NexusIM.Managers
{
	internal static class AudioVideoCallManager
	{
		public static void Init()
		{
			AccountManager.Accounts.CollectionChanged += Accounts_CollectionChanged;

			calls = new Dictionary<string, CallContext>();
		}

		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (IMProtocolWrapper protocol in e.NewItems)
			{
				if (protocol.Protocol is IAudioVideoCapableProtocol)
				{
					IAudioVideoCapableProtocol avcp = (IAudioVideoCapableProtocol)protocol.Protocol;

					avcp.IncomingCall += avcp_IncomingCall;
				}
			}
		}
		private static void avcp_IncomingCall(object sender, IncomingCallEventArgs e)
		{
			e.Accepted = true;
			e.AcceptedPayloadTypes.Add(e.IncomingAudioPayloadTypes.First());

			var ips = from netface in NetworkInterface.GetAllNetworkInterfaces().SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
					  let addr = netface.Address
					  let addrbytes = addr.GetAddressBytes()
					  where !(addr.IsLoopback() || addr.IsLinkLocal()) && addr.AddressFamily == AddressFamily.InterNetwork
					  select netface.Address;

			CallContext context = new CallContext(e);

			e.BeginIceSelection = context.BeginIceSelection;
			calls.Add(e.Id, context);

			byte[] ufrag = new byte[6];
			Random rand = new Random();
			rand.NextBytes(ufrag);
			e.IceUfrag = Convert.ToBase64String(ufrag);
			byte[] pwd = new byte[12];
			rand.NextBytes(pwd);
			e.IcePassword = Convert.ToBase64String(pwd);

			int cursor = 0;
			foreach (var ip in ips)
			{
				e.AddCandidate(++cursor, 1, new IPEndPoint(ip, context.RtpPort));
				e.AddCandidate(++cursor, 2, new IPEndPoint(ip, context.RtcpPort));
			}

			context.BeginIceSelection(null, null);
		}

		private class CallContext
		{
			public CallContext(IncomingCallEventArgs args)
			{
				buffer = new byte[512];
				this.args = args;

				InitSocket();
			}

			private void InitSocket()
			{
				RtpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				RtcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				Random rand = new Random();
				RtpSocket.Bind(new IPEndPoint(IPAddress.Any, rand.Next(1025, 65535)));
				RtcpSocket.Bind(new IPEndPoint(IPAddress.Any, rand.Next(1025, 65535)));
				RtpSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Socket_Receive, null);
				RtcpSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RtcpSocket_Receive, null);
			}

			public void BeginIceSelection(object sender, EventArgs e)
			{
				Thread thread = new Thread(DoIceCandidacy);
				thread.Start();
			}

			private void DoIceCandidacy()
			{
				var remoteCandidates = from c in args.IncomingTransportCandidates
									   where c.ProtocolType == ProtocolType.Udp && c.EndPoint.AddressFamily == AddressFamily.InterNetwork
									   select c;

				var rtpCandidates = from c in remoteCandidates
									where c.Component == 1
									select c;

				var rtcpCandidates = from c in remoteCandidates
									 where c.Component == 2
									 select c;

				foreach (var candidate in rtpCandidates)
				{
					AttemptStunDiscovery(candidate, RtpSocket);
				}
			}

			private void AttemptStunDiscovery(SdpTransportCandidate candidate, Socket socket)
			{
				MemoryStream ms = new MemoryStream();
				BinaryWriter writer = new EndianAwareBinaryWriter(ms, Endianness.BigEndian);

				Random rand = new Random();
				StunPacket packet = new StunBindingRequest();
				rand.NextBytes(packet.TransactionId);
				packet.WriteBody(writer, Endianness.BigEndian);

				socket.SendTo(ms.GetBuffer(), 0, (int)ms.Length, SocketFlags.None, candidate.EndPoint);
			}

			private void Socket_Receive(IAsyncResult a)
			{
				int bytesRead = RtpSocket.EndReceive(a);

				MemoryStream ms = new MemoryStream(buffer, 0, bytesRead);

				StunPacket packet = StunPacket.ParseBody(new EndianAwareBinaryReader(ms, Endianness.BigEndian));
				if (packet is StunBindingRequest)
				{
					StunBindingRequest bindRequest = packet as StunBindingRequest;

				}
			}

			private void RtcpSocket_Receive(IAsyncResult a)
			{
				int bytesRead = RtcpSocket.EndReceive(a);
			}

			public Socket RtpSocket
			{
				get;
				private set;
			}
			public Socket RtcpSocket
			{
				get;
				private set;
			}
			public int RtpPort
			{
				get {
					return ((IPEndPoint)RtpSocket.LocalEndPoint).Port;
				}
			}
			public int RtcpPort
			{
				get {
					return ((IPEndPoint)RtcpSocket.LocalEndPoint).Port;
				}
			}

			private byte[] buffer;
			private IncomingCallEventArgs args;
		}

		private static IDictionary<string, CallContext> calls;
	}
}
