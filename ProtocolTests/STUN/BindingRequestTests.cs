using System;
using System.IO;
using System.Text;
using InstantMessage.Misc;
using InstantMessage.Protocols.AudioVideo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests.STUN
{
	[TestClass]
	public class BindingRequestTests
	{
		[TestMethod]
		public void BindingRequestSerializeTest()
		{
			byte[] transId = new byte[] { 0xc0, 0x53, 0x1e, 0x05, 0x3d, 0x01, 0xb3, 0x0a, 0x8c, 0xf0, 0x0b, 0x37 };
			Random rand = new Random();
			//rand.NextBytes(transId);

			StunBindingRequest request = new StunBindingRequest();
			Buffer.BlockCopy(transId, 0, request.TransactionId, 0, 12);
			request.Attributes[0x8022] = Encoding.ASCII.GetBytes("ice4j.org");

			byte[] buffer = new byte[44];

			using (BinaryWriter writer = new EndianAwareBinaryWriter(new MemoryStream(buffer), Endianness.BigEndian))
			{
				request.WriteBody(writer, Endianness.BigEndian);
			}

			byte[] expected = new byte[] { 0x00, 0x01, 0x00, 0x18, 0x21, 0x12, 0xa4, 0x42, 0xc0, 0x53, 0x1e, 0x05, 0x3d, 0x01, 0xb3, 0x0a, 0x8c, 0xf0, 0x0b, 0x37, 0x80, 0x22, 0x00, 0x09, 0x69, 0x63, 0x65, 0x34, 0x6a, 0x2e, 0x6f, 0x72, 0x67, 0x00, 0x00, 0x00, 0x80, 0x28, 0x00, 0x04, 0x45, 0x25, 0x68, 0x39};

			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual(expected[i], buffer[i], "Incorrect byte at offset {0}", i);
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
			Assert.AreEqual(9, reader.ReadInt16(), "Expected Fingerprint attribute length");
			string s = new String(reader.ReadChars(9));
			Assert.AreEqual("ice4j.org", s);
			Assert.AreEqual("\0\0\0", new String(reader.ReadChars(3)));

			Assert.AreEqual(0x8028, reader.ReadUInt16(), "Expected Fingerprint attribute id");
			Assert.AreEqual(4, reader.ReadInt16(), "Expected Fingerprint attribute length");
			Assert.AreEqual(0x45256839, reader.ReadUInt32());
		}

		[TestMethod]
		public void DeserializeBindingRequestTest()
		{
			byte[] input = new byte[] { 0x00, 0x01, 0x00, 0x58, 0x21, 0x12, 0xa4, 0x42, 0x99, 0xdb, 0x0a, 0x0b, 0x3d, 0x01, 0xad, 0xef, 0x05, 0x43, 0xa2, 0x32, 0x00, 0x24, 0x00, 0x04, 0x6e, 0x00, 0x0a, 0xff, 0x80, 0x2a, 0x00, 0x08, 0x29, 0xb5, 0x05, 0x14, 0xb1, 0x41, 0xde, 0xd0, 0x00, 0x06, 0x00, 0x0e, 0x42, 0x73, 0x4a, 0x58, 0x34, 0x55, 0x41, 0x48, 0x3a, 0x35, 0x6e, 0x6f, 0x6a, 0x30, 0x00, 0x00, 0x80, 0x22, 0x00, 0x09, 0x69, 0x63, 0x65, 0x34, 0x6a, 0x2e, 0x6f, 0x72, 0x67, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x14, 0xf3, 0xd7, 0xef, 0x06, 0x7e, 0x71, 0xd6, 0xd6, 0xa3, 0x59, 0x16, 0xf6, 0xa9, 0x81, 0xc9, 0xa4, 0x4b, 0x11, 0x33, 0xff, 0x80, 0x28, 0x00, 0x04, 0x09, 0x22, 0xf1, 0xa2 };

			BinaryReader reader = new EndianAwareBinaryReader(new MemoryStream(input), Endianness.BigEndian);
			StunPacket packet = StunPacket.ParseBody(reader);

			Assert.AreEqual(reader.BaseStream.Length, reader.BaseStream.Position, "BinaryReader should be at end of stream");

			Assert.IsInstanceOfType(packet, typeof(StunBindingRequest));
			Assert.AreEqual("ice4j.org", packet.ApplicationName);
			
		}

		[TestMethod]
		public void CrcTest()
		{
			byte[] test = new byte[] { 0, 1, 0, 0x18, 33, 18, 0xa4, 66, 92, 0xe6, 0xc3, 5, 61, 1, 0xd6, 0xd6, 4, 0xc8, 90, 56, 0x80, 34, 0, 9, 105, 99, 101, 52, 106, 46, 111, 114, 103, 0, 0, 0, 0};

			Crc32 crc = new Crc32(0x04c11db7, Crc32.DefaultSeed);

			byte[] crc1 = crc.ComputeHash(test, 0, 36);
			Assert.AreNotEqual(0xCF75AC7C, BitConverter.ToInt32(crc1, 0));
			Assert.AreNotEqual((uint)0xCF75AC7C, BitConverter.ToUInt32(crc1, 0));

			Assert.Fail("Did not find CRC");
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
