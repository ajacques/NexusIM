using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Misc
{
	enum Endianness
	{
		LittleEndian,
		BigEndian
	}
	internal class EndianAwareBinaryWriter : BinaryWriter
	{
		public EndianAwareBinaryWriter(Stream outputStream, Endianness targetEndian) : base(outputStream)
		{
			TargetEndian = targetEndian;
		}

		public override void Write(int value)
		{
			if (TargetEndian != MachineEndian)
				value = Endian.SwapInt32(value);

			base.Write(value);
		}

		public override void Write(uint value)
		{
			if (TargetEndian != MachineEndian)
				value = Endian.SwapUInt32(value);

			base.Write(value);
		}

		public override void Write(short value)
		{
			if (TargetEndian != MachineEndian)
				value = Endian.SwapInt16(value);

			base.Write(value);
		}

		public override void Write(ushort value)
		{
			if (TargetEndian != MachineEndian)
				value = Endian.SwapUInt16(value);

			base.Write(value);
		}

		public Endianness TargetEndian
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