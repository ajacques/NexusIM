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

		internal void VerifyIqAttributes(XmlDocument document, Jid from, Jid to, string id, IqMessage.IqType type)
		{
			XmlElement element = document.DocumentElement;

			Assert.IsTrue(element.HasAttribute("from"), "Message did not have a from attribute set");
			Assert.IsTrue(element.HasAttribute("to"), "Message did not have a to attribute set");
			Assert.IsTrue(element.HasAttribute("id"), "Message did not have an id attribute set");
			Assert.IsTrue(element.HasAttribute("type"), "Message did not have a type attribute set");

			Assert.AreEqual(from.ToString(), element.GetAttribute("from"));
			Assert.AreEqual(to.ToString(), element.GetAttribute("to"));
			Assert.AreEqual(id, element.GetAttribute("id"));
			Assert.AreEqual(type.ToString(), element.GetAttribute("type"));
		}
	}
}
