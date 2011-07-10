using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;

namespace InstantMessage.Protocols.SIP
{
	public class SIPClient : IMProtocol
	{
		public SIPClient()
		{
			mClient = new UdpClient();

			UserAgent = "NexusIM SIP Client";
			mSequences = new SortedList<int, SIPPacket>();
		}

		public override void BeginLogin()
		{
			base.BeginLogin();
			//mClient.Connect(Server, 5060);

			RegisterPacket packet = new RegisterPacket(this);
			SendPacket(packet);
			mClient.BeginReceive(new AsyncCallback(ReceivePacket), null);
		}

		// Packet Classes
		private class SIPPacket
		{
			protected SIPPacket(SIPClient client)
			{
				Headers = new SortedDictionary<string, string>();

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

			public string GetBody(SIPClient client)
			{
				StringWriter writer = new StringWriter();

				Headers["User-Agent"] = client.UserAgent;
				Headers["CSeq"] = String.Format("{0} {1}", SequenceId, Method);

				// Write the first line
				writer.WriteLine("{0} sip:{1} SIP/2.0", Method, Target);

				foreach (KeyValuePair<string, string> pair in Headers)
					writer.WriteLine("{0}: {1}", pair.Key, pair.Value);
				writer.WriteLine("Content-Length: 0");

				writer.WriteLine();
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
		private string ProcessDigestAuth(string crypto, string realm, string nonce)
		{
			if (mA1Hash == null)
			{
				string input = Username + ":" + realm + ":" + Password;
				mA1Hash = ComputeMD5Hash(input);
			}
			
			string a2 = ComputeMD5Hash("REGISTER:sip:" + Server);

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

								authline = ProcessDigestAuth(algo, realm, nonce);
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
		protected int sequenceId;
	}
}