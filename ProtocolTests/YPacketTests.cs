using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.Yahoo;

namespace ProtocolTests
{
	[TestClass]
	public class YPacketTests
	{
		[TestMethod]
		public void ByteTest()
		{
			// Sample YMSG Auth packet service type 87
			byte[] testPacket = { 0x59, 0x4d, 0x53, 0x47, 0x00, 0x11, 0x00, 0x00, 0x00, 0x12, 0x00, 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x31, 0xc0, 0x80, 0x61, 0x64, 0x72, 0x65, 0x6e, 0x73, 0x6f, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65, 0xc0, 0x80 };
			YPacket_Accessor packet = YPacket_Accessor.FromPacket(testPacket);

			Assert.AreEqual(YahooServices_Accessor.ymsg_authentication, packet.Service);
			Assert.AreEqual(0, packet.version[0]);
			Assert.AreEqual(17, packet.version[1]);
			Assert.AreEqual("\0\0\0\0", packet.Status);
			string value;
			Assert.IsTrue(packet.Parameters.TryGetValue(1, out value), "Parameter was not loaded properly");
			Assert.AreEqual("adrensoftware", value);
		}

		[TestMethod]
		public void BuildTest()
		{
			byte[] testPacket = { 0x59, 0x4d, 0x53, 0x47, 0x00, 0x11, 0x00, 0x00, 0x00, 0x12, 0x00, 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x31, 0xc0, 0x80, 0x61, 0x64, 0x72, 0x65, 0x6e, 0x73, 0x6f, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65, 0xc0, 0x80 };
			YPacket_Accessor packet = new YPacket_Accessor();
			packet.Service = YahooServices_Accessor.ymsg_authentication;
			packet.AddParameter(1, "adrensoftware");
			byte[] result = packet.ToBytes();

			Assert.AreEqual(testPacket.Length, result.Length);
			for (int i = 0; i < testPacket.Length; i++)
				Assert.AreEqual(testPacket[i], result[i]);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MalformedTest()
		{
			byte[] testPacket = { 0x59, 0x4d, 0x53, 0x47, 0x00, 0x11, 0x00, 0x00, 0x00 };
			YPacket_Accessor packet = YPacket_Accessor.FromPacket(testPacket);
		}
	}
}
