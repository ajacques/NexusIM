using System.Collections.Generic;
using System.Xml;
using InstantMessage.Protocols.XMPP.Messages;

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
			IqMessage message;
			if (messageFactories.TryGetValue(lookup, out factory))
			{
				message = factory(reader) as IqMessage;
				reader.Read();
			} else {
				UnknownFragmentMessage msg = (UnknownFragmentMessage)UnknownFragmentMessage.GetMessageFactory()(reader);

				message = new IqFragmentMessage(msg.Document);
			}

			message.Id = msgid;
			return message;
		}

		private static IDictionary<string, MessageFactory> messageFactories;
	}
}