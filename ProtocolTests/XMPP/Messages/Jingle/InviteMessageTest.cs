using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InstantMessage.Protocols.XMPP;
using InstantMessage.Protocols.XMPP.Messages;
using InstantMessage.Protocols.XMPP.Messages.Jingle;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests.XMPP.Messages.Jingle
{
	[TestClass]
	public class InviteMessageTest : BaseMessageTest
	{
		private void VerifyJingleAttributes(XmlElement iqNode, Jid source, string action, string subns, string sessionid)
		{
			Assert.IsNotNull(iqNode["jingle"]);
			XmlElement jingle = iqNode["jingle"];

			Assert.AreEqual("urn:xmpp:jingle:" + subns, jingle.NamespaceURI);

			Assert.IsTrue(jingle.HasAttribute("action"), "Message did not have an action attribute set");
			Assert.IsTrue(jingle.HasAttribute("initiator"), "Message did not have an initiator attribute set");
			Assert.IsTrue(jingle.HasAttribute("sid"), "Message did not have a session id attribute set");

			Assert.AreEqual(action, jingle.GetAttribute("action"));
			Assert.AreEqual(source.ToString(), jingle.GetAttribute("initiator"));
			Assert.AreEqual(sessionid, jingle.GetAttribute("sid"));
		}

		[TestCategory("XMPP")]
		[TestCategory("Messages")]
		[TestMethod]
		public void BasicInviteTest()
		{
			JingleInviteMessage.AttemptMessage msg = new JingleInviteMessage.AttemptMessage();
			msg.Source = Romeo;
			msg.To = Juliet;
			msg.Id = MessageId;
			msg.SessionId = "sessionid";
			msg.DescriptionNodes.Add(StubDescription);

			XmlDocument xml = SerializeMessage(msg);
			VerifyIqAttributes(xml, Romeo, Juliet, MessageId, IqMessage.IqType.set);

			XmlElement iq = xml.DocumentElement;
			VerifyJingleAttributes(iq, Romeo, "session-initiate", "1", "sessionid");

			XmlElement jingle = iq["jingle"];

			XmlElement content = jingle["content"];
			Assert.IsNotNull(content, "Jingle node did not have a subnode called content");

			Assert.IsTrue(content.HasAttribute("creator"), "Content node did not have a creator attribute");
			Assert.AreEqual("initiator", content.GetAttribute("creator"));

			XmlElement description = content["description"];
			Assert.IsNotNull(description, "Content node did not have a subnode called description");

			XmlElement transport = content["transport"];
			Assert.IsNotNull(transport, "Content node did not have a subnode called transport");
		}

		[TestMethod]
		public void RtpSerializeTest()
		{
			JingleRtpDescription descriptor = BasicRtpDescription;
			XmlDocument xml = SerializeObject((writer) => {
				writer.WriteStartElement("content");
				descriptor.WriteBody(writer);
				writer.WriteEndElement();
			});

			XmlElement content = xml.DocumentElement;

			foreach (XmlElement element in content)
			{
				Assert.IsTrue(element.HasAttribute("id"), "Payload type did not have attribute name 'id'");
				Assert.IsTrue(element.HasAttribute("name"), "Payload type node did not have attribute 'name'");

				int id;
				Assert.IsTrue(Int32.TryParse(element.GetAttribute("id"), out id), "Attribute id did not have integer value: {0}", element.GetAttribute("id"));
			}
		}

		private IJingleDescriptionType StubDescription
		{
			get {
				return new JingleStubDescription();
			}
		}
		
		private JingleRtpDescription BasicRtpDescription
		{
			get {
				JingleRtpDescription descriptor = new JingleRtpDescription();
				descriptor.PayloadTypes.Add(NewPayloadType(96, "speex"));

				return descriptor;
			}
		}

		private JingleRtpDescription.PayloadType NewPayloadType(int id, string codecName)
		{
			return new JingleRtpDescription.PayloadType()
			{
				Id = id,
				Name = codecName
			};
		}
	}
}
