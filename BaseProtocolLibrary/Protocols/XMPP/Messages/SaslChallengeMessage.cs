using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	sealed class SaslChallengeMessage : XmppMessage
	{
		private SaslChallengeMessage()
		{
		}

		public static MessageFactory GetMessageFactory()
		{
			return new MessageFactory(GetMessage);
		}

		private static XmppMessage GetMessage(XmlReader reader)
		{
			reader.Read();
			SaslChallengeMessage message = new SaslChallengeMessage();

			string base64 = Encoding.ASCII.GetString(Convert.FromBase64String(reader.Value));
			string[] parts = base64.Split(',');
			message.Parameters = parts.ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);

			return message;
		}

		public override void WriteMessage(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public IDictionary<string, string> Parameters
		{
			get;
			private set;
		}
	}
}
