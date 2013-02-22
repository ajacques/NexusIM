using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.AudioVideo;
using System.IO;
using InstantMessage.Misc;
using System.Net.Sockets;

namespace ProtocolTests.STUN
{
	[TestClass]
	public class BindingRequestTests
	{
		[TestMethod]
		public void BindingRequestSerializeTest()
		{
			byte[] transId = new byte[] { 0xff, 0x7f, 0xe7, 0xef, 0x3c, 0x01, 0xaf, 0x64, 0x34, 0x21, 0x81, 0x7c };
			Random rand = new Random();
			//rand.NextBytes(transId);

			StunBindingRequest request = new StunBindingRequest();
			Buffer.BlockCopy(transId, 0, request.TransactionId, 0, 12);
			request.Attributes[0x8022] = Encoding.ASCII.GetBytes("nexus-im.com");

			byte[] buffer = new byte[44];

			using (BinaryWriter writer = new EndianAwareBinaryWriter(new MemoryStream(buffer), Endianness.LittleEndian))
			{
				request.WriteBody(writer, Endianness.BigEndian);
			}

			BinaryReader reader = new BinaryReader(new MemoryStream(buffer));

			Assert.AreEqual(1, reader.ReadUInt16(), "Message type was incorrectly serialized");
			Assert.AreEqual(24, reader.ReadUInt16(), "Message length was incorrectly serialized");
			Assert.AreEqual(0x2112a442, reader.ReadInt32(), "Incorrect message cookie");

			for (int i = 0; i < 12; i++)
			{
				Assert.AreEqual(transId[i], reader.ReadByte(), "Transaction ID byte range did not match expected transaction id");
			}

			Assert.AreEqual(0x8022, reader.ReadUInt16(), "Expected Fingerprint attribute id");
			Assert.AreEqual(12, reader.ReadInt16(), "Expected Fingerprint attribute length");
			string s = new String(reader.ReadChars(12));
			Assert.AreEqual("nexus-im.com", s);

			Assert.AreEqual(0x8028, reader.ReadUInt16(), "Expected Fingerprint attribute id");
			Assert.AreEqual(4, reader.ReadInt16(), "Expected Fingerprint attribute length");
		}

		[TestMethod]
		public void BindingResponseSucessTest()
		{
			byte[] data = new byte[] { 0x01, 0x01, 0x00, 0x28, 0x21, 0x12, 0xa4, 0x42, 0x16, 0xe8, 0x2d, 0xfb, 0x3c, 0x01, 0xa0, 0xec, 0x62, 0x9f, 0x10, 0x7f, 0x00, 0x20, 0x00, 0x08, 0x00, 0x01, 0x32, 0x9f, 0x39, 0x07, 0xdf, 0x9b, 0x80, 0x22, 0x00, 0x0e, 0x54, 0x75, 0x72, 0x6e, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x20, 0x30, 0x2e, 0x36, 0x00, 0x00, 0x80, 0x28, 0x00, 0x04, 0xfd, 0xe1, 0x86, 0x02 };

			BinaryReader reader = new EndianAwareBinaryReader(new MemoryStream(data), Endianness.BigEndian);
			StunPacket packet = StunPacket.ParseBody(reader);

			Assert.IsInstanceOfType(packet, typeof(StunBindingResponse));
		}
	}
}
