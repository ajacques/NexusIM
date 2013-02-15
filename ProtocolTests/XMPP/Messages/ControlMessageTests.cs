using System.Xml;
using InstantMessage.Protocols.XMPP;
using InstantMessage.Protocols.XMPP.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests.XMPP.Messages
{
	[TestClass]
	public class ControlMessageTests : BaseMessageTest
	{
		[TestMethod]
		public void StreamInitMessageTest()
		{
			StreamInitMessage msg = new StreamInitMessage(Domain);

			XmlDocument doc = SerializeObject((writer) =>
			{
				msg.WriteMessage(writer);
				writer.WriteEndElement(); // Have to close the attribute
			});
			XmlElement stream = doc.DocumentElement;

			Assert.AreEqual(XmppNamespaces.Streams, stream.NamespaceURI);
			Assert.AreEqual("stream", stream.LocalName);
			Assert.IsTrue(stream.HasAttribute("version"), "Stream did not have version attribute");
			Assert.IsTrue(stream.HasAttribute("to"), "Stream did not have to attribute");
			Assert.AreEqual("1.0", stream.GetAttribute("version"));
			Assert.AreEqual(Domain, stream.GetAttribute("to"));
		}

		[TestMethod]
		public void PingMessageTest()
		{
			PingMessage msg = new PingMessage();
			msg.Source = Romeo;
			msg.Id = MessageId;

			XmlDocument doc = SerializeMessage(msg);
			XmlElement iq = doc.DocumentElement;

			VerifyIqAttributes(iq, Romeo, null, MessageId, IqMessage.IqType.get);
			Assert.AreEqual(XmppNamespaces.JabberClient, iq.NamespaceURI);

			XmlElement ping = iq["ping"];
			Assert.IsNotNull(ping, "Ping element did not exist");

			Assert.AreEqual(XmppNamespaces.Ping, ping.NamespaceURI);
		}

		[TestMethod]
		public void StartTlsMessageTest()
		{
			StartTlsMessage msg = new StartTlsMessage();

			XmlDocument xml = SerializeMessage(msg);
			XmlElement stls = xml.DocumentElement;

			Assert.AreEqual("starttls", stls.LocalName);
			Assert.AreEqual(XmppNamespaces.Tls, stls.NamespaceURI);
		}
	}
}
