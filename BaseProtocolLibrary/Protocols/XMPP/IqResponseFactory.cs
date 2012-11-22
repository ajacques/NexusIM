using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal static class IqResponseFactory
	{
		static IqResponseFactory()
		{
			messageFactories = new SortedDictionary<string, MessageFactory>();
			messageFactories.Add(XmppNamespaces.Bind + "bind", BindToResourceMessage.ResponseMessage.GetMessageFactory());
		}

		public static MessageFactory GetMessageFactory()
		{
			return ReadMessage;
		}

		private static XmppMessage ReadMessage(XmlReader reader)
		{
			reader.MoveToAttribute("id");
			string msgid = reader.Value;

			reader.Read();

			string lookup = reader.NamespaceURI + reader.LocalName;

			MessageFactory factory;
			if (messageFactories.TryGetValue(lookup, out factory))
			{
				IqMessage message = factory(reader) as IqMessage;
				reader.Read();
				message.Id = msgid;
				return message;
			}

			return null;
		}

		private static IDictionary<string, MessageFactory> messageFactories;
	}
}