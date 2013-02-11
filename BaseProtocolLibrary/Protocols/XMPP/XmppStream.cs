using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	internal class XmppStream
	{
		public XmppStream(Stream targetWriteStream, XmppProtocol protocol)
		{
			protocolLayerStream = transportLayerStream = targetWriteStream;
			this.protocol = protocol;

			writerSettings = new XmlWriterSettings();
			writerSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
			writerSettings.Encoding = Encoding.UTF8;
			ResetWriterState();

			mMsgReader = new XmppMessageReader();
			mMsgReader.NewReaderState();

			writeLock = new object();
			readLock = new object();
		}

		public void WriteMessage(XmppMessage message)
		{
			lock (writeLock)
			{
				message.WriteMessage(xmlWriter);
				xmlWriter.Flush();
			}
		}

		public void InitReader()
		{
			mMsgReader.UnderlyingStream = protocolLayerStream;
			mMsgReader.NewReaderState();
		}

		public void ResetWriterState()
		{
			xmlWriter = XmlWriter.Create(protocolLayerStream, writerSettings);
		}

		public XmppMessage ReadMessage()
		{
			lock (readLock)
			{
				return mMsgReader.ReadMessage();
			}
		}

		public void ActivateTLS(string targetHost)
		{
			Trace.WriteLine("XMPP: Beginning TLS Negotiaton");
			Stopwatch sw = new Stopwatch();
			sw.Start();
			lock (readLock)
			{
				lock (writeLock)
				{
					protocolLayerStream = sslStream = new SslStream(protocolLayerStream, true, new RemoteCertificateValidationCallback(OnRemoteCertVerify));
					sslStream.AuthenticateAsClient(targetHost);
				}
			}
			TimeSpan protocol = sw.Elapsed;
			xmlWriter = XmlWriter.Create(protocolLayerStream, writerSettings);
			InitReader();
			ResetWriterState();

			sw.Stop();
			Trace.WriteLine(String.Format("XMPP: TLS Negotiation complete (TotalTime: {0}; Protocol Cost: {1}; Client-side: {2})", sw.Elapsed, protocol, sw.Elapsed - protocol));
		}

		private bool OnRemoteCertVerify(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return protocol.TriggerTlsVerifyEvent(certificate, chain, sslPolicyErrors);
		}
		
		public bool IsEncrypted
		{
			get	{
				return sslStream != null && sslStream.IsEncrypted;
			}
		}

		private XmlWriter xmlWriter;
		private XmlWriterSettings writerSettings;
		private XmppMessageReader mMsgReader;

		// Thread Safety
		private object writeLock;
		private object readLock;

		private XmppProtocol protocol;
		private SslStream sslStream;
		private Stream protocolLayerStream; // Highest level stream - No transforms, clear-text
		private Stream transportLayerStream; // Lowest level stream
	}
}
