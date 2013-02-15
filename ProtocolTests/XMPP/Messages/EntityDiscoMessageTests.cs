using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.XMPP.Messages;
using System.Xml;
using InstantMessage.Protocols.XMPP;

namespace ProtocolTests.XMPP.Messages
{
	[TestClass]
	public class EntityDiscoMessageTests : BaseMessageTest
	{
		private const string Namespace = "urn:unit-test:namespace";
		private const string Node = "01234567890=";

		[TestMethod]
		public void RequestSerializeTest()
		{
			EntityDiscoveryMessage.Request msg = new EntityDiscoveryMessage.Request(Namespace, Node);
			msg.Source = Romeo;
			msg.To = Juliet;
			msg.Id = MessageId;

			XmlDocument doc = SerializeMessage(msg);
			XmlElement iq = doc.DocumentElement;

			VerifyIqAttributes(iq, Romeo, Juliet, MessageId, IqMessage.IqType.get);
			VerifyQueryNode(iq);
		}

		[TestMethod]
		public void ResponseSerializeTest()
		{
			EntityDiscoveryMessage.Response msg = new EntityDiscoveryMessage.Response(Namespace, Node);
			msg.Source = Romeo;
			msg.To = Juliet;
			msg.Id = MessageId;
			msg.Features.Add(new EntityDiscoveryMessage.XmppFeature(Namespace));

			XmlDocument doc = SerializeMessage(msg);
			XmlElement iq = doc.DocumentElement;

			VerifyIqAttributes(iq, Romeo, Juliet, MessageId, IqMessage.IqType.result);
			VerifyQueryNode(iq);

			XmlElement query = iq["query"];

			Assert.IsNotNull(query["feature"]);

			foreach (XmlElement caps in query)
			{
				if (caps.NamespaceURI == XmppNamespaces.DiscoInfo)
				{
					switch (caps.Name)
					{
						case "identity":
							Assert.IsTrue(caps.HasAttribute("category"), "Identity feature element did not have a category attribute");
							Assert.IsTrue(caps.HasAttribute("name"), "Identity feature element did not have a name attribute");
							Assert.IsTrue(caps.HasAttribute("type"), "Identity feature element did not have a type attribute");
							Assert.AreNotEqual(caps.GetAttribute("category").Length >= 1, "Category attribute must be non-empty");
							Assert.AreNotEqual(caps.GetAttribute("type").Length >= 1, "Type attribute must be non-empty");
							break;
						case "feature":
							VerifyDiscoFeature(caps);
							break;
					}
				}
			}
		}

		[TestMethod]
		public void SerializeXmppFeatureTest()
		{
			EntityDiscoveryMessage.XmppFeature feat = new EntityDiscoveryMessage.XmppFeature(Namespace);

			XmlDocument doc = SerializeObject((writer) => feat.WriteXml(writer));
			XmlElement feature = doc.DocumentElement;

			VerifyDiscoFeature(feature);
		}

		private void VerifyDiscoFeature(XmlElement feature)
		{
			Assert.AreEqual(XmppNamespaces.DiscoInfo, feature.NamespaceURI);
			Assert.IsTrue(feature.HasAttribute("var"), "Feature element did not have a var attribute");
		}

		private void VerifyQueryNode(XmlElement iq)
		{
			XmlElement query = iq["query"];
			Assert.IsNotNull(query, "Query subnode does not exist. OuterXml: {0}", iq.OuterXml);
			Assert.AreEqual(XmppNamespaces.DiscoInfo, query.NamespaceURI);

			Assert.IsTrue(query.HasAttribute("node"), "Query element must have an attribute 'node'");
			Assert.AreEqual(String.Format("{0}#{1}", Namespace, Node), query.GetAttribute("node"));
		}
	}
}
