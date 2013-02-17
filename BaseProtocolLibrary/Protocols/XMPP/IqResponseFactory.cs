using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Linq;
using InstantMessage.Protocols.XMPP.Messages;
using System.Diagnostics;

namespace InstantMessage.Protocols.XMPP
{
	internal static class IqResponseFactory
	{
		static IqResponseFactory()
		{
			messageFactories = new SortedDictionary<string, MessageFactory>();

			var msgtypes = from type in Assembly.GetAssembly(typeof(IqMessage)).GetTypes()
						   let attrib = type.GetCustomAttribute<IqMessageBodyAttribute>()
						   let entry = (from method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
										where method.GetCustomAttribute<XmppMessageFactoryEntryAttribute>() != null
										select MessageFactory.CreateDelegate(typeof(MessageFactory), method)).FirstOrDefault() as MessageFactory
						   where type.IsSubclassOf(typeof(IqMessage)) && attrib != null && entry != null
						   select new KeyValuePair<string, MessageFactory>(attrib.Namespace + attrib.LocalName, entry);

			foreach (var msgtype in msgtypes)
			{
				messageFactories.Add(msgtype);
			}
		}

		public static MessageFactory GetMessageFactory()
		{
			return ReadMessage;
		}

		private static XmppMessage ReadMessage(XmlReader reader)
		{
			reader.MoveToAttribute("id");
			string msgid = reader.Value;

			reader.MoveToAttribute("type");
			IqMessage.IqType type = (IqMessage.IqType)Enum.Parse(typeof(IqMessage.IqType), reader.Value);

			Jid jid = null;
			if (reader.MoveToAttribute("from"))
			{
				jid = Jid.Parse(reader.Value);
			}

			IqMessage message;
			if (reader.Read())
			{
				XmlReader subtree = reader.ReadSubtree();
				subtree.Read();

				string lookup = subtree.NamespaceURI + subtree.LocalName;

				MessageFactory factory;
				if (messageFactories.TryGetValue(lookup, out factory))
				{
					message = factory(subtree) as IqMessage;
					subtree.Read();

					int i = 0;
					while (subtree.Read() && subtree.NodeType != XmlNodeType.None)
					{
						i++;
					}

					Debug.WriteLineIf(i >= 1, String.Format("IQ Message Factory for type {0} did not balance the stack correctly. Had to manually collapse {1} steps", lookup, i));
				} else {
					UnknownFragmentMessage msg = (UnknownFragmentMessage)UnknownFragmentMessage.GetMessageFactory()(subtree);

					message = IqFragmentMessage.FromWire(msgid, jid, msg.Document, type);
				}
			} else {
				message = IqFragmentMessage.FromWire(msgid, jid, new XmlDocument(), type);
			}

			if (message != null)
			{
				message.Id = msgid;
				message.Source = jid;
			}
			return message;
		}

		private static IDictionary<string, MessageFactory> messageFactories;
	}
}