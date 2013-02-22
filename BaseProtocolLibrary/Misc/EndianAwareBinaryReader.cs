using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Misc
{
	class EndianAwareBinaryReader : BinaryReader
	{
		public EndianAwareBinaryReader(Stream inputStream, Endianness targetEndian) : base(inputStream)
		{
			SourceEndian = targetEndian;
		}

		public override short ReadInt16()
		{
			return Endian.SwapInt16(base.ReadInt16());
		}

		public override ushort ReadUInt16()
		{
			return Endian.SwapUInt16(base.ReadUInt16());
		}

		public Endianness SourceEndian
		{
			get;
			private set;
		}
		private Endianness MachineEndian
		{
			get {
				return BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
			}
		}
	}
}
