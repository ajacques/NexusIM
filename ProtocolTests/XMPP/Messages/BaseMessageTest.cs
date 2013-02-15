using System;
using System.IO;
using System.Xml;
using InstantMessage.Protocols.XMPP;
using InstantMessage.Protocols.XMPP.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests.XMPP.Messages
{
	public abstract class BaseMessageTest
	{
		protected readonly Jid Romeo = new Jid("romeo", "montague.lit", "orchard");
		protected readonly Jid Juliet = new Jid("juliet", "capulet.lit", "balcony");
		protected const string MessageId = "testid";
		protected const string Domain = "example.com";

		internal XmlDocument SerializeMessage(XmppMessage message)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				XmlWriter writer = XmlWriter.Create(ms);
				writer.WriteStartDocument();
				message.WriteMessage(writer);
				writer.Flush();
				ms.Seek(0, SeekOrigin.Begin);

				XmlDocument xml = new XmlDocument();
				xml.Load(ms);
				return xml;
			}
		}
		internal XmlDocument SerializeObject(Action<XmlWriter> target)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				XmlWriter writer = XmlWriter.Create(ms);
				writer.WriteStartDocument();
				target(writer);
				writer.Flush();
				ms.Seek(0, SeekOrigin.Begin);

				XmlDocument xml = new XmlDocument();
				xml.Load(ms);
				return xml;
			}
		}

		internal void VerifyIqAttributes(XmlElement root, Jid from, Jid to, string id, IqMessage.IqType type)
		{
			Assert.AreEqual(XmppNamespaces.JabberClient, root.NamespaceURI, "Iq element did not have correct XML namespace");

			Assert.IsTrue(root.HasAttribute("from"), "Message did not have a from attribute set");

			if (to != null)
			{
				Assert.IsTrue(root.HasAttribute("to"), "Message did not have a to attribute set");
				Assert.AreEqual(to.ToString(), root.GetAttribute("to"));
			}

			Assert.IsTrue(root.HasAttribute("id"), "Message did not have an id attribute set");
			Assert.IsTrue(root.HasAttribute("type"), "Message did not have a type attribute set");

			Assert.AreEqual(from.ToString(), root.GetAttribute("from"));
			Assert.AreEqual(id, root.GetAttribute("id"));
			Assert.AreEqual(type.ToString(), root.GetAttribute("type"));
		}
		protected void VerifyInt32(XmlNode attribute)
		{
			int value;
			Assert.IsTrue(Int32.TryParse(attribute.InnerText, out value), "Value for attribute name {0} was not an integer. Actual: {1}", attribute.Name, attribute.Value);
		}
	}
}
