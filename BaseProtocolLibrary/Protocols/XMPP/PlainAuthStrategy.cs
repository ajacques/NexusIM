using System;
using System.Text;
using System.Xml;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	internal class PlainAuthStrategy : IAuthStrategy
	{
		public void StartAuthentication(XmppProtocol protocol)
		{
			protocol.WriteMessage(new PlainAuthMessage(protocol.Username, protocol.Password));
		}

		public void PerformStep(SaslChallengeMessage message)
		{
			throw new NotSupportedException("PLAIN Authentication does support multiple steps.");
		}

		public void FinalizeStep(SaslAuthMessage.SuccessMessage message)
		{
			
		}

		// Nested Classes
		internal class PlainAuthMessage : SaslAuthMessage
		{
			public PlainAuthMessage(string username, string password)
			{
				Username = username;
				Password = password;
			}

			protected override void WriteAuthBody(XmlWriter writer)
			{
				byte[] busername = Encoding.GetBytes(Username);
				byte[] bpassword = Encoding.GetBytes(Password);
				byte[] data = new byte[busername.Length + bpassword.Length + 2];
				Buffer.BlockCopy(busername, 0, data, 1, busername.Length);
				Buffer.BlockCopy(bpassword, 0, data, busername.Length + 2, bpassword.Length);

				writer.WriteBase64(data, 0, data.Length);
			}

			// Properties
			protected string Username
			{
				get;
				private set;
			}
			protected string Password
			{
				get;
				private set;
			}

			protected override string Mechanism
			{
				get
				{
					return "PLAIN";
				}
			}
		}
	}
}
