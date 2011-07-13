using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using InstantMessage;

namespace InstantMessage.Protocols.SIP
{
	[IMNetwork("sip")]
	public class SIPClient : IMProtocol, IProtocol, IRequiresUsername, IRequiresPassword //, IVOIPCapable
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
		public SIPCall NewCall()
		{
			string callId = GenerateCallId();

			SIPCall call = new SIPCall(this, callId);
			mCallsInProgress.Add(callId, call);

			return call;
		}

		internal void InviteUser(SIPCall call, string username, string mediaProfile)
		{
			InvitePacket packet = new InvitePacket(username, call, this);
			IPEndPoint ep = (IPEndPoint)mClient.Client.LocalEndPoint;
			packet.Body.Write(mediaProfile);

			SendPacket(packet);
		}

		// Packet Classes
		private abstract class SIPPacket
		{
			protected SIPPacket(int seqId)
			{
				Headers = new SortedDictionary<string, string>();
				Body = new StringWriter();

				SequenceId = seqId;
			}

			public int SequenceId
			{
				get;
				protected set;
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
			public StringWriter Body
			{
				get;
				private set;
			}

			public string GetBody(SIPClient client)
			{
				StringWriter writer = new StringWriter();

				Headers["User-Agent"] = client.UserAgent;
				Headers["CSeq"] = String.Format("{0} {1}", SequenceId, Method);
				Headers["Max-Forwards"] = "70";
				Headers["Content-Length"] = Body.ToString().Length.ToString();

				// Write the first line
				writer.WriteLine("{0} sip:{1} SIP/2.0", Method, Target);

				foreach (KeyValuePair<string, string> pair in Headers)
					writer.WriteLine("{0}: {1}", pair.Key, pair.Value);

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
			public RegisterPacket(SIPClient client) : base(++client.sequenceId)
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
			public InvitePacket(string username, SIPCall call, SIPClient client) : base(++client.sequenceId)
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
		private class ResponsePacket : SIPPacket
		{
			public ResponsePacket(TextReader source) : base(-1)
			{
				string status = source.ReadLine();
				StatusCode = Int32.Parse(status.Substring(8, 3));

				while (source.Peek() != -1)
				{
					string line = source.ReadLine();
					if (String.IsNullOrEmpty(line))
						break;

					int headerPoint = line.IndexOf(':') + 1;
					string header = line.Substring(0, headerPoint - 1);
					string value = line.Substring(headerPoint + 1);

					Headers.Add(header, value);
				}

				string cseq;
				if (Headers.TryGetValue("CSeq", out cseq))
					SequenceId = Int32.Parse(cseq.Substring(0, cseq.IndexOf(' ')));
			}

			public int StatusCode
			{
				get;
				private set;
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
		private void ProcessUnauthorized(ResponsePacket respPacket)
		{
			int id = respPacket.SequenceId;
			SIPPacket packet = mSequences[id];
			string realm = null;
			string nonce = null;
			string hash = null;

			string authval = respPacket.Headers["WWW-Authenticate"];
			string scheme = authval.Substring(0, authval.IndexOf(' '));
			string[] parts = authval.Substring(authval.IndexOf(' ')).Split(',');
			switch (scheme)
			{
				case "Digest":
					string algo = parts[0].Substring(11);
					realm = parts[1].Substring(8, parts[1].Length - 9);
					nonce = parts[2].Substring(8, parts[2].Length - 9);

					hash = ProcessDigestAuth(packet, algo, realm, nonce);
					break;
				default:
					triggerOnError(new Events.IMErrorEventArgs(Events.IMProtocolErrorReason.Unknown, "Unsupported authentication scheme: " + parts[0]));
					break;
			}

			if (packet != null && hash != null)
			{
				// Digest username=\"adam\", realm=\"asterisk\", nonce=\"4774c314\", uri=\"sip:192.168.2.217\", response=\"52f9ebebadfea2dde23e0308b95e9e7d\", algorithm=MD5
				StringBuilder builder = new StringBuilder();
				builder.Append("Digest ");
				builder.AppendFormat("username=\"{0}\"", Username);
				builder.AppendFormat(", realm=\"{0}\"", realm);
				builder.AppendFormat(", nonce=\"{0}\"", nonce);
				if (packet is RegisterPacket)
					builder.AppendFormat(", uri=\"sip:{0}\"", Server);
				else
					builder.AppendFormat(", uri=\"sip:{0}\"", packet.Target);
				builder.AppendFormat(", response=\"{0}\"", hash);
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
			
			ResponsePacket packet = new ResponsePacket(reader);
			
			switch (packet.StatusCode)
			{
				case 401: // Unauthorized
					ProcessUnauthorized(packet);
					break;
				case 200:
					SIPPacket tPacket = mSequences[packet.SequenceId];

					if (tPacket is RegisterPacket)
						LoginWaitHandle.Set();
					mSequences.Remove(packet.SequenceId);

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