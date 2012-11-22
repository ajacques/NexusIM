using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;

namespace InstantMessage.Protocols.XMPP
{
	internal delegate XmppMessage MessageFactory(XmlReader reader);

	internal class XmppMessageReader
	{
		public XmppMessageReader()
		{
			mMessageFactories = new SortedDictionary<string, MessageFactory>();
			mMessageFactories.Add("http://etherx.jabber.org/streamsstream", StreamInitMessage.GetMessageFactory());
			mMessageFactories.Add("urn:ietf:params:xml:ns:xmpp-tlsproceed", StartTlsMessage.ProceedMessage.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "success", SaslAuthMessage.SuccessMessage.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "failure", SaslAuthMessage.FailureMessage.GetMessageFactory());
			mMessageFactories.Add(SaslAuthMessage.Namespace + "challenge", SaslChallengeMessage.GetMessageFactory());
		}

		public void NewReaderState()
		{
			mNameTable = new NameTable();
			mNSManager = new XmlNamespaceManager(mNameTable);
			mContext = new XmlParserContext(mNameTable, mNSManager, "en-US", XmlSpace.None);
			mReaderSettings = new XmlReaderSettings();
			mReaderSettings.CheckCharacters = false;

			PropertyInfo pinfo = mReaderSettings.GetType().GetProperty("Async");
			pinfo.SetValue(mReaderSettings, true, null);

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
			string key = mReader.NamespaceURI + mReader.LocalName;

			Trace.WriteLine(String.Format("Received new stanza of type {0} named {1}", mReader.NamespaceURI, mReader.LocalName));

			MessageFactory factory;
			if (mMessageFactories.TryGetValue(key, out factory))
			{
				XmppMessage msg = factory(mReader);
				return msg;
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
