using System;
using System.Collections.Generic;
using System.IO;
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
			descriptor.Ufrag = "test";
			descriptor.Password = "test";

			XmlDocument xml = SerializeObject((writer) => {
				writer.WriteStartElement("transport");
				descriptor.WriteBody(writer);
				writer.WriteEndElement();
			});
			XmlElement content = xml.DocumentElement;

			VerifyTransportElement(content);
		}

		[TestMethod]
		public void SdpCandidateIPv6DeserializeTest()
		{
			string xml = GenerateIceCandidateXml(6, 1, "udp", 2113937151, 0, 1, new IPEndPoint(IPAddress.Parse("fe80:0:0:0:e954:4b2c:368a:6fba"), 5002), "host", 0);

			XmlReader reader = XmlReader.Create(new StringReader(xml));
			reader.Read();

			XmppSdpCandidate sdp = XmppSdpCandidate.Parse(reader);
			Assert.AreEqual(1, sdp.Component, "Incorrect component id");
			Assert.AreEqual(ProtocolType.Udp, sdp.ProtocolType, "Incorrect protocol type");
			Assert.AreEqual(2113937151, sdp.Priority, "Incorrect priority");
			Assert.AreEqual(1, sdp.Id, "Incorrect Id");
			Assert.AreEqual(IPAddress.Parse("fe80::e954:4b2c:368a:6fba"), sdp.EndPoint.Address, "Incorrect IP Address");
			Assert.AreEqual(5002, sdp.EndPoint.Port, "Incorrect port");
			Assert.AreEqual(JingleCandidateType.host, sdp.Type, "Incorrect candidate type");
		}

		[TestMethod]
		public void TransportDeserializeTest()
		{
			string ufrag = "5noj0";
			string pwd = "6sotfp1lj611k61pfndkljb79u";

			StringWriter writer = new StringWriter();
			writer.Write("<transport xmlns='urn:xmpp:jingle:transports:ice-udp:1' ufrag='{0}' pwd='{1}'>", ufrag, pwd);
			writer.Write(GenerateIceCandidateXml(6, 1, "udp", 2113937151, 0, 1, new IPEndPoint(IPAddress.Parse("fe80:0:0:0:e954:4b2c:368a:6fba"), 5002), "host", 0));
			writer.Write(GenerateIceCandidateXml(1, 1, "udp", 2113932031, 0, 3, new IPEndPoint(IPAddress.Parse("10.0.0.3"), 5002), "host", 0));
			writer.Write(GenerateIceCandidateXml(7, 2, "udp", 1677724415, 0, 2, new IPEndPoint(IPAddress.Parse("1.2.3.4"), 5002), "srflx", 0));
			writer.Write("</transport>");

			XmlReader reader = XmlReader.Create(new StringReader(writer.ToString()));
			JingleTransportDescription transport = JingleTransportDescription.Parse(reader);

			Assert.AreEqual(3, transport.Candidates.Count, "Expected '3' decoded candidates");
			Assert.AreEqual("6sotfp1lj611k61pfndkljb79u", transport.Password);
			Assert.AreEqual("5noj0", transport.Ufrag);
		}

		private string GenerateIceCandidateXml(int foundation, int component, string protocol, int priority, int generation, int id, IPEndPoint ep, string type, int network)
		{
			return String.Format("<candidate foundation='{0}' component='{1}' protocol='{2}' priority='{3}' generation='{4}' id='{5}' ip='{6}' port='{7}' type='{8}' network='{9}'/>", foundation, component, protocol, priority, generation, id, ep.Address, ep.Port, type, network);
		}

		private void VerifyTransportElement(XmlElement content)
		{
			Assert.IsNotNull(content["candidate"], "Transport node must have one or more elements labelled 'candidate'");
			VerifyRequiredAttributes(content, "ufrag", "pwd");

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
			return new XmppSdpCandidate(ProtocolType.Udp, ep, priority, priority, 1, JingleCandidateType.host);
		}

		private const string JingleNamespace = "urn:xmpp:jingle:";
		private const string JingleNamespaceRoot = JingleNamespace + "1";
	}
}
