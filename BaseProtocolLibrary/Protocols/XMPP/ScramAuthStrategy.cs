using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace InstantMessage.Protocols.XMPP
{
	internal class ScramAuthStrategy : IAuthStrategy
	{
		public ScramAuthStrategy()
		{
			hashAlgo = new SHA1Managed();
		}

		public void StartAuthentication(XmppProtocol protocol)
		{
			IDictionary<string, string> param = new Dictionary<string, string>();
			param.Add("n", String.Format("\"{0}\"", protocol.Username));
			param.Add("r", GenerateRandomNonce());
			
			ScramAuthMessage mesg = new ScramAuthMessage(param);

			protocol.WriteMessage(mesg);
		}

		public void PerformStep(XmppMessage message)
		{
			throw new NotImplementedException();
		}

		private static string GenerateRandomNonce()
		{
			Random rand = new Random();
			TextWriter writer = new StringWriter();

			for (int i = 0; i < 24; ++i)
			{
				int digit = rand.Next(48, 110);

				if (digit > 57)
					digit += 8;
				if (digit > 90)
					digit += 7;
				
				writer.Write(Char.ConvertFromUtf32(digit));
			}

			return writer.ToString();
		}

		// Nested Classes
		private class ScramAuthMessage : SaslAuthMessage
		{
			public ScramAuthMessage(IDictionary<string, string> parameters)
			{
				this.parameters = parameters;
			}

			protected override void WriteAuthBody(XmlWriter writer)
			{
				MemoryStream ms = new MemoryStream();
				TextWriter mem = new StreamWriter(ms);

				mem.Write("n,,");
				foreach (var kvp in parameters)
				{
					mem.Write("{0}={1},", kvp.Key, kvp.Value);
				}

				mem.Flush();
				ms.SetLength(ms.Length - 1);

				writer.WriteBase64(ms.ToArray(), 0, (int)ms.Length);
			}

			protected override string Mechanism
			{
				get {
					return "SCRAM-SHA-1";
				}
			}

			private IDictionary<string, string> parameters;
		}

		private HashAlgorithm hashAlgo;
	}
}
