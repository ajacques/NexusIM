using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace InstantMessage.Protocols.SIP
{
	class SIPClient : IMProtocol
	{
		public SIPClient()
		{
			mClient = new UdpClient();
		}

		public override void BeginLogin()
		{
			base.BeginLogin();

			RegisterPacket packet = new RegisterPacket();

			string pktdata = packet.GetBody(this);
		}

		private class SIPPacket
		{
			protected SIPPacket()
			{
				Headers = new SortedDictionary<string, string>();
			}

			public string Method
			{
				get;
				protected set;
			}
			protected IDictionary<string, string> Headers
			{
				get;
				private set;
			}

			public string GetBody(SIPClient client)
			{
				StringWriter writer = new StringWriter();

				Headers["User-Agent"] = client.UserAgent;

				// Write the first line
				writer.WriteLine("{0} sip:{1} SIP/2.0", Method, client.Server);

				foreach (KeyValuePair<string, string> pair in Headers)
					writer.WriteLine("{0}: {1}", pair.Key, pair.Value);

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
			public RegisterPacket() : base()
			{
				Method = "REGISTER";
			}
		}

		public string UserAgent
		{
			get;
			set;
		}

		private UdpClient mClient;
	}
}