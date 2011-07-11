using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace InstantMessage.Protocols.SIP
{
	public class SIPClient : IMProtocol//, IVOIPCapable
	{
		public SIPClient()
		{
			mClient = new UdpClient();

			UserAgent = "NexusIM SIP Client";
			mSequences = new SortedList<int, SIPPacket>();
			mCallsInProgress = new SortedList<string, SIPCall>();
		}

		public override void BeginLogin()
		{
			base.BeginLogin();

			RegisterPacket packet = new RegisterPacket(this);
			SendPacket(packet);
			mClient.BeginReceive(new AsyncCallback(ReceivePacket), null);
		}
		public SIPCall BeginCall(string username)
		{
			InvitePacket packet = new InvitePacket(username, this);
			SendPacket(packet);

			string callId = packet.Headers["Call-Id"];

			SIPCall call = new SIPCall(this, callId);
			mCallsInProgress.Add(callId, call);

			return call;
		}

		// Packet Classes
		private abstract class SIPPacket
		{
			protected SIPPacket(SIPClient client)
			{
				Headers = new SortedDictionary<string, string>();
				Body = new StringWriter();

				SequenceId = ++client.sequenceId;
			}

			public int SequenceId
			{
				get;
				private set;
			}
			public string Method
			{
				get;
				protected set;
			}
			public string Target
			{
				get;
				set;
			}
			public IDictionary<string, string> Headers
			{
				get;
				private set;
			}
			protected StringWriter Body
			{
				get;
				private set;
			}

			public string GetBody(SIPClient client)
			{
				StringWriter writer = new StringWriter();

				Headers["User-Agent"] = client.UserAgent;
				Headers["CSeq"] = String.Format("{0} {1}", SequenceId, Method);
				

				// Write the first line
				writer.WriteLine("{0} sip:{1} SIP/2.0", Method, Target);

				foreach (KeyValuePair<string, string> pair in Headers)
					writer.WriteLine("{0}: {1}", pair.Key, pair.Value);
				writer.Write("Content-Length: ");
				writer.WriteLine(Body.ToString().Length);

				writer.WriteLine();

				// Write Body
				writer.Write(Body.ToString());

				return writer.ToString();
			}

			protected string GetHeader(string key)
			{
				string value;
				if (Headers.TryGetValue(key, out value))
					return value;

				return null;
			}
		}
		private class RegisterPacket : SIPPacket
		{
			public RegisterPacket(SIPClient client) : base(client)
			{
				Method = "REGISTER";
				Target = client.Server;

				Headers.Add("To", String.Format("\"{0}\" <sip:{0}@{1}>", client.Username, client.Server));
				Headers.Add("From", String.Format("\"{0}\" <sip:{0}@{1}>", client.Username, client.Server));
				Headers.Add("Call-Id", client.GenerateCallId());
			}
		}
		private class InvitePacket : SIPPacket
		{
			public InvitePacket(string username, SIPClient client) : base(client)
			{
				Method = "INVITE";
				Target = username;

				Headers.Add("Content-Type", "application/sdp");
				Headers.Add("From", String.Format("\"{0}\" <sip:{0}@{1}>", client.Username, client.Server));
				Headers.Add("To", String.Format("\"{0}\" <sip:{0}>", username));
				Headers.Add("Call-Id", client.GenerateCallId());

				BuildSDP();
			}

			private void BuildSDP()
			{
				Body.WriteLine("v=0");
				Body.WriteLine("o=- {0} {0} IN IP4 {1}", Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 12, 1)).TotalSeconds).ToString(), "192.168.2.35");
				Body.WriteLine("s=NexusIM VoIP Call");
				Body.WriteLine("t=0 0");
				Body.WriteLine("m=audio 50000 RTP/AVP 9");
				Body.WriteLine("a=rtpmap:9 G722/8000");
				Body.WriteLine("a=rtpmap:8 speex/48000");
				Body.WriteLine("a=sendrecv");

				BuildConnInfo();
			}

			private void BuildConnInfo()
			{
				IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

				foreach (IPAddress ip in localIPs)
				{
					switch (ip.AddressFamily)
					{
						case AddressFamily.InterNetwork:
							if (ip.GetAddressBytes()[0] == 169)
								continue;
							Body.Write("c=IN IP4 ");
							break;
						case AddressFamily.InterNetworkV6:
							if (ip.GetAddressBytes()[0] == 0xfe && ip.GetAddressBytes()[1] == 0x80)
								continue;
							Body.Write("c=IN IP6 ");
							break;
						default:
							continue;
					}

					Body.WriteLine(ip.ToString());
				}
			}
		}

		private string ComputeMD5Hash(string input)
		{
			if (mMD5Hash == null)
				mMD5Hash = new MD5Cng();

			byte[] output = mMD5Hash.ComputeHash(Encoding.Default.GetBytes(input));

			StringBuilder builder = new StringBuilder(32);
			for (int i = 0; i < output.Length; i++)
				builder.Append(output[i].ToString("x2"));

			return builder.ToString();
		}
		private string ProcessDigestAuth(SIPPacket packet, string crypto, string realm, string nonce)
		{
			if (mA1Hash == null)
			{
				string input = Username + ":" + realm + ":" + Password;
				mA1Hash = ComputeMD5Hash(input);
			}
			
			string a2 = ComputeMD5Hash(packet.Method + ":sip:" + packet.Target);

			string final = ComputeMD5Hash(mA1Hash + ":" + nonce + ":" + a2);

			return final;
		}
		private void ProcessUnauthorized(TextReader reader)
		{
			SIPPacket packet = null;
			string authline = null;
			string realm = null;
			string nonce = null;

			while (reader.Peek() != -1)
			{
				string line = reader.ReadLine();
				if (String.IsNullOrEmpty(line))
					break;

				int headerPoint = line.IndexOf(':') + 1;
				string header = line.Substring(0, headerPoint - 1);
				string value = line.Substring(headerPoint + 1);

				switch (header)
				{
					case "CSeq":
						int id = Int32.Parse(value.Substring(0, value.IndexOf(' ')));
						mSequences.TryGetValue(id, out packet);
						break;
					case "WWW-Authenticate":
						string scheme = value.Substring(0, value.IndexOf(' '));
						string[] parts = value.Substring(value.IndexOf(' ')).Split(',');
						switch (scheme)
						{
							case "Digest":
								string algo = parts[0].Substring(11);
								realm = parts[1].Substring(8, parts[1].Length - 9);
								nonce = parts[2].Substring(8, parts[2].Length - 9);

								authline = ProcessDigestAuth(packet, algo, realm, nonce);
								break;
							default:
								triggerOnError(new Events.IMErrorEventArgs(Events.IMProtocolErrorReason.Unknown, "Unsupported authentication scheme: " + parts[0]));
								break;
						}
						break;
				}
			}

			if (packet != null && authline != null)
			{
				// Digest username=\"adam\", realm=\"asterisk\", nonce=\"4774c314\", uri=\"sip:192.168.2.217\", response=\"52f9ebebadfea2dde23e0308b95e9e7d\", algorithm=MD5
				StringBuilder builder = new StringBuilder();
				builder.Append("Digest ");
				builder.AppendFormat("username=\"{0}\"", Username);
				builder.AppendFormat(", realm=\"{0}\"", realm);
				builder.AppendFormat(", nonce=\"{0}\"", nonce);
				builder.AppendFormat(", uri=\"sip:{0}\"", Server);
				builder.AppendFormat(", response=\"{0}\"", authline);
				builder.AppendFormat(", algorithm=\"{0}\"", "MD5");

				if (packet.Headers.ContainsKey("Authorization"))
				{
					triggerOnError(null);
					return;
				}
				packet.Headers.Add("Authorization", builder.ToString());

				ResendPacket(packet);
			}
		}
		private void ReceivePacket(IAsyncResult e)
		{
			IPEndPoint source = null;
			byte[] pktdata = mClient.EndReceive(e, ref source);
			mClient.BeginReceive(new AsyncCallback(ReceivePacket), null);
			
			TextReader reader = new StringReader(Encoding.UTF8.GetString(pktdata));
			string status = reader.ReadLine();

			int statusCode = Int32.Parse(status.Substring(8, 3));

			switch (statusCode)
			{
				case 401: // Unauthorized
					ProcessUnauthorized(reader);
					break;
				case 200: // OK
					mLoginWaitHandle.Set();
					mProtocolStatus = IMProtocolStatus.Online;
					break;
			}
		}

		protected string GenerateCallId()
		{
			Random rand = new Random();
			StringBuilder builder = new StringBuilder(32);

			for (int i = 0; i < builder.Capacity; i++)
				builder.Append((char)rand.Next(97, 122));

			return builder.ToString();
		}
		private void ResendPacket(SIPPacket packet)
		{
			string pktdata = packet.GetBody(this);
			byte[] pkt = Encoding.UTF8.GetBytes(pktdata);
			mClient.Send(pkt, pkt.Length, new IPEndPoint(IPAddress.Parse(Server), 5060));
		}
		private void SendPacket(SIPPacket packet)
		{
			string pktdata = packet.GetBody(this);
			byte[] pkt = Encoding.UTF8.GetBytes(pktdata);
			mSequences.Add(packet.SequenceId, packet);

			mClient.Send(pkt, pkt.Length, new IPEndPoint(IPAddress.Parse(Server), 5060));
		}

		public string UserAgent
		{
			get;
			set;
		}

		private MD5 mMD5Hash;
		private string mA1Hash;
		private UdpClient mClient;
		private SortedList<int, SIPPacket> mSequences;
		private SortedList<string, SIPCall> mCallsInProgress;
		protected int sequenceId;
	}
}