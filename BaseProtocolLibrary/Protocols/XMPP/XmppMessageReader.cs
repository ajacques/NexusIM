using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	internal delegate XmppMessage MessageFactory(XmlReader reader);

	internal class XmppMessageReader
	{
		public XmppMessageReader()
		{
			mMessageFactories = new SortedDictionary<string, MessageFactory>();
			mMessageConstructers = new SortedDictionary<string, ConstructorInfo>();
			mMessageFactories.Add(XmppNamespaces.JabberClient + "iq", IqResponseFactory.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "success", SaslAuthMessage.SuccessMessage.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "failure", SaslAuthMessage.FailureMessage.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "challenge", SaslChallengeMessage.GetMessageFactory());

			var msgtypes = from type in Assembly.GetAssembly(typeof(XmppMessage)).GetTypes()
						   let attrib = type.GetCustomAttribute<ReadableXmppMessageAttribute>()
						   let factory = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(XmlReader) }, null)
						   where type.IsSubclassOf(typeof(XmppMessage)) && attrib != null
						   select new KeyValuePair<string, ConstructorInfo>(attrib.Namespace + attrib.LocalName, factory);

			foreach (var msgtype in msgtypes)
			{
				mMessageConstructers.Add(msgtype);
			}
		}

		public void NewReaderState()
		{
			mNameTable = new NameTable();
			mNSManager = new XmlNamespaceManager(mNameTable);
			mContext = new XmlParserContext(mNameTable, mNSManager, "en-US", XmlSpace.None);
			mReaderSettings = new XmlReaderSettings();
			mReaderSettings.CheckCharacters = false;
			mReaderSettings.Async = true;

			if (mStream != null)
			{
				mReader = XmlReader.Create(mStream, mReaderSettings, mContext);
			}
		}

		public Stream UnderlyingStream
		{
			get	{
				return mStream;
			}
			set	{
				if (mStream != value)
				{
					mStream = value;
					
					mReader = XmlReader.Create(mStream, mReaderSettings, mContext);
				}
			}
		}

		private XmppMessage ReadRootElement()
		{
			XmlReader reader = mReader.ReadSubtree();
			reader.Read();

			string key = reader.NamespaceURI + reader.LocalName;

			Trace.WriteLine(String.Format("Received new stanza of type {0} named {1}", mReader.NamespaceURI, mReader.LocalName));

			MessageFactory factory;
			ConstructorInfo constructer;

			if (mMessageFactories.TryGetValue(key, out factory))
			{
				XmppMessage msg = factory(reader);

				reader.Read();

				int i = 0;
				while (reader.Read() && reader.NodeType != XmlNodeType.None)
				{
					i++;
				}

				Debug.WriteLineIf(i >= 1, String.Format("Message Factory for type {0} did not balance the stack correctly. Had to manually collapse {1} steps", key, i));

				return msg;
			} else if (mMessageConstructers.TryGetValue(key, out constructer)) {
				return (XmppMessage)constructer.Invoke(new object[] { reader });
			}

			throw new NotSupportedException(String.Format("Stanza of type {0}:{1} not supported for deserialization.", mReader.NamespaceURI, mReader.LocalName));
		}

		public XmppMessage ReadMessage()
		{
			while (mReader.Read())
			{
				switch (mReader.NodeType)
				{
					case XmlNodeType.Element:
						return ReadRootElement();
				}
			}

			return null;
		}

		private IDictionary<string, MessageFactory> mMessageFactories;
		private IDictionary<string, ConstructorInfo> mMessageConstructers;

		// XML State Variables
		private XmlParserContext mContext;
		private XmlNameTable mNameTable;
		private XmlNamespaceManager mNSManager;
		private XmlReaderSettings mReaderSettings;

		// I/O Variables
		private XmlReader mReader;
		private Stream mStream;
	}
}
