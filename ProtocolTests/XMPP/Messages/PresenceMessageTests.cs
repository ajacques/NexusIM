using System.Xml;
using InstantMessage.Protocols.XMPP;
using InstantMessage.Protocols.XMPP.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests.XMPP.Messages
{
	[TestClass]
	public class PresenceMessageTests : BaseMessageTest
	{
		[TestMethod]
		public void AvailableTest()
		{
			PresenceMessage msg = new PresenceMessage();
			msg.Priority = 1;
			XmlDocument xml = SerializeMessage(msg);

			XmlElement presence = xml.DocumentElement;
			XmlElement priority = presence["priority"];
			Assert.IsNotNull(priority, "Presence message did not have a priority");
			VerifyInt32(priority);
		}

		[TestMethod]
		public void AwayTest()
		{
			PresenceMessage msg = new PresenceMessage();
			msg.Priority = 1;
			msg.Show = "away";
			msg.Status = "STATUS";
			XmlDocument xml = SerializeMessage(msg);

			XmlElement presence = xml.DocumentElement;
			Assert.AreEqual(XmppNamespaces.JabberClient, presence.NamespaceURI);

			XmlElement priority = presence["priority"];
			Assert.IsNotNull(priority, "Presence message did not have a priority");
			VerifyInt32(priority);
			Assert.AreEqual(XmppNamespaces.JabberClient, priority.NamespaceURI);

			XmlElement show = presence["show"];
			Assert.IsNotNull(show, "Presence node did not have a show subnode");
			Assert.AreEqual("away", show.InnerText);
			Assert.AreEqual(XmppNamespaces.JabberClient, show.NamespaceURI);

			XmlElement status = presence["status"];
			Assert.IsNotNull(status, "Presence node did not have a status subnode");
			Assert.AreEqual("STATUS", status.InnerText);
			Assert.AreEqual(XmppNamespaces.JabberClient, status.NamespaceURI);
		}

		[TestMethod]
		public void AwayNullStatusTest()
		{
			PresenceMessage msg = new PresenceMessage();
			msg.Priority = 1;
			msg.Show = "away";
			XmlDocument xml = SerializeMessage(msg);

			XmlElement presence = xml.DocumentElement;
			Assert.AreEqual(XmppNamespaces.JabberClient, presence.NamespaceURI);

			XmlElement priority = presence["priority"];
			Assert.IsNotNull(priority, "Presence message did not have a priority");
			VerifyInt32(priority);
			Assert.AreEqual(XmppNamespaces.JabberClient, priority.NamespaceURI);

			XmlElement show = presence["show"];
			Assert.IsNotNull(show, "Presence node did not have a show subnode");
			Assert.AreEqual("away", show.InnerText);
			Assert.AreEqual(XmppNamespaces.JabberClient, show.NamespaceURI);

			XmlElement status = presence["status"];
			Assert.IsNull(status, "Presence node did have a status subnode");
		}
	}
}
