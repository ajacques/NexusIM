using System;
using System.Security.Cryptography;

namespace InstantMessage.Misc
{
	internal class Crc32 : HashAlgorithm
	{
		public Crc32() : this(DefaultPolynomial, DefaultSeed)
		{
		}

		public Crc32(uint polynomial, uint seed)
		{
			table = InitializeTable(polynomial);
			this.seed = seed;
			Initialize();
		}

		public override void Initialize()
		{
			hash = seed;
		}

		protected override void HashCore(byte[] buffer, int start, int length)
		{
			{
				for (int i = start; i < start + length; i++)
				{
					hash = (hash >> 8) ^ table[(hash ^ buffer[i]) & 0xff];
				}
			}
		}

		protected override byte[] HashFinal()
		{
			HashValue = BitConverter.GetBytes(~hash);
			return HashValue;
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == DefaultPolynomial && defaultTable != null)
				return defaultTable;

			uint[] createTable = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint entry = (uint)i;
				for (int j = 0; j < 8; j++)
					if ((entry & 1) == 1)
						entry = (entry >> 1) ^ polynomial;
					else
						entry = entry >> 1;
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
				defaultTable = createTable;

			return createTable;
		}

		public override int HashSize
		{
			get {
				return 32;
			}
		}

		public const uint DefaultPolynomial = 0xedb88320;
		public const uint DefaultSeed = 0xffffffff;

		private uint hash;
		private uint seed;
		private uint[] table;
		private static uint[] defaultTable;
	}
}
