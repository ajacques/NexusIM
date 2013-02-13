using System;
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
			descriptor.MediaType = "audio";
			XmlDocument xml = SerializeObject((writer) => {
				writer.WriteStartElement("description");
				descriptor.WriteBody(writer);
				writer.WriteEndElement();
			});

			XmlElement content = xml.DocumentElement;
			Assert.IsTrue(content.HasAttribute("media"), "Content node did not have media attribute");
			Assert.AreEqual("audio", content.GetAttribute("media"));

			foreach (XmlElement element in content)
			{
				Assert.IsTrue(element.HasAttribute("id"), "Payload type did not have attribute name 'id'");
				Assert.IsTrue(element.HasAttribute("name"), "Payload type node did not have attribute 'name'");

				if (element.HasAttribute("clockrate"))
				{
					int clockrate;
					Assert.IsTrue(Int32.TryParse(element.GetAttribute("clockrate"), out clockrate), "Attribute clockrate did not have integer value: {0}", element.GetAttribute("clockrate"));
				}

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
				descriptor.MediaType = "audio";
				descriptor.PayloadTypes.Add(NewPayloadType(96, "speex",	16000));
				descriptor.PayloadTypes.Add(NewPayloadType(97, "speex", 8000));
				descriptor.PayloadTypes.Add(NewPayloadType(103, "L16", 16000, 2));

				return descriptor;
			}
		}

		private JingleRtpDescription.PayloadType NewPayloadType(int id, string codecName, int? clockrate = null, int channels = 1)
		{
			return new JingleRtpDescription.PayloadType()
			{
				Id = id,
				Name = codecName,
				ClockRate = clockrate,
				Channels = channels
			};
		}
	}
}
