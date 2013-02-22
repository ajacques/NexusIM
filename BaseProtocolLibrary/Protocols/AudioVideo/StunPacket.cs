using System;
using System.Collections.Generic;
using System.IO;
using InstantMessage.Misc;

namespace InstantMessage.Protocols.AudioVideo
{
	internal abstract class StunPacket
	{
		public StunPacket()
		{
			TransactionId = new byte[12];
			Attributes = new SortedDictionary<ushort, byte[]>();

			Attributes[FingerprintAttributeId] = new byte[4];
		}

		public static StunPacket ParseBody(BinaryReader reader)
		{
			ushort messageType = reader.ReadUInt16();
			byte messageClass = (byte)((messageType & 0x100) >> 7 | (messageType & 0x10) >> 4);
			ushort messageMethod = (ushort)((messageType & 0xfe00) >> 1 | (messageType & 0xf0) >> 1 | (messageType & 0x0f));
			ushort length = reader.ReadUInt16();
			reader.ReadUInt32(); // Message Cookie

			StunPacket packet = null;

			if (messageMethod == 1 && messageClass == 2) // Binding Success Response
			{
				packet = new StunBindingResponse();
			} else {
				throw new NotSupportedException();
			}

			packet.TransactionId = reader.ReadBytes(12);

			if (length >= 1)
			{
				while (reader.PeekChar() != -1)
				{
					ushort attributeType = reader.ReadUInt16();
					ushort attributeLength = reader.ReadUInt16();
					byte[] attributes = reader.ReadBytes(attributeLength);

					packet.Attributes[attributeType] = attributes;
				}
			}

			return packet;
		}

		// Methods
		public void WriteBody(BinaryWriter writer, Endianness targetEndian)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter innerWriter = new EndianAwareBinaryWriter(ms, targetEndian);
			DoWriteBody(innerWriter);
			innerWriter.Flush();

			ms.Seek(0, SeekOrigin.Begin);
			Crc32 crc = new Crc32(FingerprintCrcPolynomial, Crc32.DefaultSeed);
			byte[] crcbytes = crc.ComputeHash(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
			uint computedcrc = BitConverter.ToUInt32(crcbytes, 0);
			computedcrc = Endian.SwapUInt32(computedcrc) ^ FingerprintCrcMask;

			Attributes[FingerprintAttributeId] = BitConverter.GetBytes(computedcrc);

			DoWriteBody(writer);
		}
		private void DoWriteBody(BinaryWriter writer)
		{
			int newMessageType = ((MessageMethod & 0xff00) << 1) | ((MessageMethod & 0xf0) << 1) | (MessageMethod & 0x0f);
			newMessageType = newMessageType | ((MessageClass & 2) << 7) | ((MessageClass & 1) << 4);

			writer.Write((ushort)newMessageType);
			writer.Write(ComputeAttributeLength());
			writer.Write(0x2112a442); // Magic Cookie
			writer.Write(TransactionId);

			foreach (var pair in Attributes)
			{
				writer.Write(pair.Key);
				int length = ((int)Math.Ceiling(pair.Value.Length / 4.0) * 4);
				writer.Write((ushort)pair.Value.Length);
				writer.Write(pair.Value);

				// Padding
				for (int i = 0; i < length - pair.Value.Length; i++)
					writer.Write(false);
			}
		}
		private ushort ComputeAttributeLength()
		{
			int size = 0;
			foreach (var pair in Attributes)
			{
				size += 4 + ((int)Math.Ceiling(pair.Value.Length / 4.0) * 4);
			}

			return (ushort)size;
		}

		// Properties
		protected abstract ushort MessageMethod
		{
			get;
		}
		protected abstract byte MessageClass
		{
			get;
		}
		public byte[] TransactionId
		{
			get;
			private set;
		}
		public IDictionary<ushort, byte[]> Attributes
		{
			get;
			private set;
		}

		// Constants
		private const ushort FingerprintAttributeId = 0x8028;
		private const uint FingerprintCrcPolynomial = 0x04c11db7;
		private const int FingerprintCrcMask = 0x5354554e;
	}
}