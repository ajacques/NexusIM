using System;
using System.Net;
using System.Net.Sockets;
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
		public void BasicStubInviteTest()
		{
			JingleInviteMessage.AttemptMessage msg = new JingleInviteMessage.AttemptMessage();
			msg.Source = Romeo;
			msg.To = Juliet;
			msg.Id = MessageId;
			msg.SessionId = "sessionid";
			msg.DescriptionNodes.Add("audio", StubDescription);
			msg.TransportNodes.Add(StubDescription);

			XmlDocument xml = SerializeMessage(msg);
			XmlElement iq = xml.DocumentElement;

			VerifyIqAttributes(iq, Romeo, Juliet, MessageId, IqMessage.IqType.set);;
			VerifyJingleAttributes(iq, Romeo, "session-initiate", "1", "sessionid");

			XmlElement jingle = iq["jingle"];
			Assert.AreEqual(JingleNamespaceRoot, jingle.NamespaceURI);

			XmlElement content = jingle["content"];
			Assert.IsNotNull(content, "Jingle node did not have a subnode called content");

			Assert.IsTrue(content.HasAttribute("creator"), "Content node did not have a creator attribute");
			Assert.AreEqual("initiator", content.GetAttribute("creator"));

			XmlElement description = content["description"];
			Assert.IsNotNull(description, "Content node did not have a subnode called description");
			Assert.AreEqual("urn:xmpp:jingle:apps:stub:0", description.NamespaceURI);

			XmlElement transport = content["transport"];
			Assert.IsNotNull(transport, "Content node did not have a subnode called transport");
			Assert.AreEqual("urn:xmpp:jingle:transports:stub:0", transport.NamespaceURI);
		}

		[TestMethod]
		public void RtpSerializeTest()
		{
			JingleRtpDescription descriptor = BasicRtpDescription;
			descriptor.MediaType = "audio";
			XmlDocument xml = SerializeObject((writer) =>
			{
				writer.WriteStartElement("content");
				descriptor.WriteBody(writer);
				writer.WriteEndElement();
			});

			XmlElement content = xml.DocumentElement;

			foreach (XmlElement element in content)
			{
				VerifyRequiredAttributes(element, "id", "name");

				if (element.HasAttribute("clockrate"))
				{
					uint clockrate;
					Assert.IsTrue(UInt32.TryParse(element.GetAttribute("clockrate"), out clockrate), "Attribute clockrate did not have integer value: {0}", element.GetAttribute("clockrate"));
				}

				byte id;
				Assert.IsTrue(Byte.TryParse(element.GetAttribute("id"), out id), "Attribute id did not have integer value: {0}", element.GetAttribute("id"));
			}
		}

		[TestMethod]
		public void TransportSerializeTest()
		{
			JingleTransportDescription descriptor = new JingleTransportDescription();
			descriptor.Candidates.Add(NewCandidate(1, new IPEndPoint(IPAddress.Loopback, 500)));
			descriptor.Candidates.Add(NewCandidate(2, new IPEndPoint(IPAddress.Loopback, 500)));

			XmlDocument xml = SerializeObject((writer) => {
				writer.WriteStartElement("transport");
				descriptor.WriteBody(writer);
				writer.WriteEndElement();
			});
			XmlElement content = xml.DocumentElement;

			VerifyTransportElement(content);
		}

		private void VerifyTransportElement(XmlElement content)
		{
			Assert.IsNotNull(content["candidate"], "Transport node must have one or more elements labelled 'candidate'");

			foreach (XmlElement element in content)
			{
				VerifyRequiredAttributes(element, "component", "foundation", "generation", "id", "ip", "network", "port", "priority", "protocol", "type");
				Assert.AreEqual(IPAddress.Loopback, IPAddress.Parse(element.GetAttribute("ip")));
				Assert.AreEqual("host", element.GetAttribute("type"));

				VerifyByte(element.GetAttributeNode("component"), 1);
				VerifyByte(element.GetAttributeNode("component"), 1);
				VerifyInt32(element.GetAttributeNode("port"), 500);
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
		private JingleTransportDescription BasicTransportDescription
		{
			get {
				JingleTransportDescription descriptor = new JingleTransportDescription();
				descriptor.Candidates.Add(NewCandidate(1, new IPEndPoint(IPAddress.Loopback, 500)));

				return descriptor;
			}
		}

		private JingleRtpDescription.JinglePayloadType NewPayloadType(int id, string codecName, int? clockrate = null, int channels = 1)
		{
			return new JingleRtpDescription.JinglePayloadType(id, codecName)
			{
				ClockRate = clockrate,
				Channels = channels
			};
		}
		private XmppSdpCandidate NewCandidate(int priority, IPEndPoint ep)
		{
			return new XmppSdpCandidate(ProtocolType.Udp, ep.Address, ep.Port, priority, priority, JingleCandidateType.host);
		}

		private const string JingleNamespace = "urn:xmpp:jingle:";
		private const string JingleNamespaceRoot = JingleNamespace + "1";
	}
}
