using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class XmppStream
	{
		public XmppStream(Stream targetWriteStream)
		{
			protocolLayerStream = transportLayerStream = targetWriteStream;

			writerSettings = new XmlWriterSettings();
			writerSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
			writerSettings.Encoding = Encoding.UTF8;
			ResetWriterState();

			mMsgReader = new XmppMessageReader();
			mMsgReader.NewReaderState();
		}

		public void WriteMessage(XmppMessage message)
		{
			message.WriteMessage(xmlWriter);
			xmlWriter.Flush();
		}

		public void InitReader()
		{
			mMsgReader.UnderlyingStream = protocolLayerStream;
			mMsgReader.NewReaderState();
		}

		public void ResetWriterState()
		{
			xmlWriter = XmlWriter.Create(transportLayerStream, writerSettings);
		}

		public XmppMessage ReadMessage()
		{
			return mMsgReader.ReadMessage();
		}

		public void ActivateTLS(string targetHost)
		{
			Trace.WriteLine("XMPP: Beginning TLS Negotiaton");
			Stopwatch sw = new Stopwatch();
			sw.Start();
			protocolLayerStream = sslStream = new SslStream(protocolLayerStream, true, new RemoteCertificateValidationCallback(OnRemoteCertVerify));
			sslStream.AuthenticateAsClient(targetHost);
			TimeSpan protocol = sw.Elapsed;
			xmlWriter = XmlWriter.Create(protocolLayerStream, writerSettings);
			InitReader();

			sw.Stop();
			Trace.WriteLine(String.Format("XMPP: TLS Negotiation complete (TotalTime: {0}; Protocol Cost: {1}; Client-side: {2})", sw.Elapsed, protocol, sw.Elapsed - protocol));
		}

		private bool OnRemoteCertVerify(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
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

		private SslStream sslStream;
		private Stream protocolLayerStream; // Highest level stream - No transforms, clear-text
		private Stream transportLayerStream; // Lowest level stream
	}
}
