using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class ScramAuthStrategy : IAuthStrategy
	{
		public ScramAuthStrategy()
		{
			currentStatus = Status.Begin;
			hash = new SHA1Managed();
			hmac = new HMACSHA1();
		}

		public void StartAuthentication(XmppProtocol protocol)
		{
			ScramAuthMessage mesg = new ScramAuthMessage();
			mesg.Parameters.Add("n", protocol.Username);
			mesg.Parameters.Add("r", GenerateRandomNonce());
			clientFirstMessage = mesg.BuildParameterList();
			mesg.initMessage = true;

			protocol.WriteMessage(mesg);
			currentStatus = Status.ClientFirstMessage;
			this.protocol = protocol;
		}

		public void PerformStep(SaslChallengeMessage message)
		{
			if (currentStatus == Status.ClientFirstMessage)
			{
				currentStatus = Status.ServerFirstMessage;

				string nonce = message.Parameters["r"];
				string salt = Encoding.ASCII.GetString(Convert.FromBase64String(message.Parameters["s"]));
				int iterations = Int32.Parse(message.Parameters["i"]);

				ScramAuthMessage mesg = new ScramAuthMessage();
				mesg.Parameters.Add("c", "biws");
				mesg.Parameters.Add("r", nonce);

				string serverFirstMessage = ImplodeDictionary(message.Parameters);

				byte[] saltedPassword = GenerateSaltedPassword(salt, iterations);
				byte[] clientKey = GenerateClientKey(saltedPassword);
				byte[] storedKey = hash.ComputeHash(clientKey);
				byte[] clientSignature = GenerateClientSignature(storedKey, clientFirstMessage, serverFirstMessage, mesg.BuildParameterList());
				byte[] clientProof = XorArray(clientKey, clientSignature);

				mesg.Parameters.Add("p", Convert.ToBase64String(clientProof));

				protocol.WriteMessage(mesg);
				currentStatus = Status.ClientFinalMessage;
			}
		}

		private static string ImplodeDictionary(IDictionary<string, string> input)
		{
			return input.Aggregate(String.Empty, (acculm, next) => String.Concat(acculm, acculm != String.Empty ? "," : "", next.Key, '=', next.Value));
		}

		public static string GenerateRandomNonce()
		{
			Random rand = new Random();
			TextWriter writer = new StringWriter();

			for (int i = 0; i < 24; ++i)
			{
				int digit = rand.Next(48, 110);

				if (digit > 57)
					digit += 7;
				if (digit > 90)
					digit += 6;
				
				writer.Write(Char.ConvertFromUtf32(digit));
			}

			return writer.ToString();
		}

		private byte[] GenerateSaltedPassword(string salt, int iterations)
		{
			// Normalize password
			byte[] str = new byte[salt.Length + 4];
			byte[] nonceBytes = Encoding.ASCII.GetBytes(salt);
			Buffer.BlockCopy(nonceBytes, 0, str, 0, nonceBytes.Length);
			str[str.Length - 1] = 1;

			hmac.Key = Encoding.UTF8.GetBytes(protocol.Password);

			// U1   := HMAC(str, salt + INT(1))
			// U2   := HMAC(str, U1)
			// ...
			// Ui-1 := HMAC(str, Ui-2)
			// Ui   := HMAC(str, Ui-1)
			// Hi   := U1 XOR U2 XOR ... XOR Ui

			byte[] ust = hmac.ComputeHash(str);
			byte[] result = ust;
			for (int i = 1; i < iterations; i++)
			{
				ust = hmac.ComputeHash(ust);
				result = XorArray(result, ust);
			}

			return result;
		}
		private byte[] GenerateClientKey(byte[] saltedPassword)
		{
			hmac.Key = saltedPassword;

			byte[] body = Encoding.ASCII.GetBytes("Client Key");

			return hmac.ComputeHash(body);
		}
		private byte[] GenerateClientSignature(byte[] storedKey, string clientFirstMessage, string serverFirstMessage, string clientFinalMessage)
		{
			string authMessage = String.Format("{0},{1},{2}", clientFirstMessage, serverFirstMessage, clientFinalMessage);

			byte[] authBytes = Encoding.ASCII.GetBytes(authMessage);

			hmac.Key = storedKey;
			return hmac.ComputeHash(authBytes);
		}
		private byte[] XorArray(byte[] left, byte[] right)
		{
			if (left.Length != right.Length)
				throw new ArgumentOutOfRangeException("Both arrays must be of the same length.");

			byte[] result = new byte[left.Length];

			for (int i = 0; i < left.Length; i++)
			{
				result[i] = (byte)(left[i] ^ right[i]);
			}

			return result;
		}

		// Nested Classes
		private class ScramAuthMessage : SaslAuthMessage
		{
			public ScramAuthMessage()
			{
				Parameters = new Dictionary<string, string>();
				mechanism = "SCRAM-SHA-1";
			}

			protected override void WriteAuthBody(XmlWriter writer)
			{
				MemoryStream ms = new MemoryStream();
				TextWriter mem = new StreamWriter(ms);

				mem.Write(BuildParameterList());

				mem.Flush();

				writer.WriteBase64(ms.ToArray(), 0, (int)ms.Length);
			}

			public string BuildParameterList()
			{
				TextWriter mem = new StringWriter();
				if (initMessage)
				{
					mem.Write("n,,");
				}
				foreach (var kvp in Parameters)
				{
					mem.Write("{0}={1},", kvp.Key, kvp.Value);
				}

				return mem.ToString().TrimEnd(',');
			}

			protected override string Mechanism
			{
				get {
					return mechanism;
				}
			}
			protected override string ElementName
			{
				get {
					return initMessage ? "auth" : "response";
				}
			}
			public IDictionary<string, string> Parameters
			{
				get;
				private set;
			}

			private string mechanism;
			public bool initMessage;
		}

		private enum Status
		{
			Begin,
			ClientFirstMessage,
			ServerFirstMessage,
			ClientFinalMessage,
			ServerFinalMessage
		}

		private HMAC hmac;
		private HashAlgorithm hash;
		private XmppProtocol protocol;
		private Status currentStatus;
		private string clientFirstMessage;
	}
}
