//-----------------------------------------------------------------------
// <copyright file="CRC32Managed.cs" company="http://gregbeech.com">
// Copyright © Greg Beech.  All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Computes a CRC32 hash for the input data using the managed library.
// </summary>
//-----------------------------------------------------------------------

namespace System.Security.Cryptography
{
    /// <summary>
    /// Computes a <strong>CRC32</strong> hash for the input data using the managed library.
    /// </summary>
    /// <remarks>
    /// The default settings for this algorithm are shown below. These are the settings used in the
    /// Ethernet protocol and popular ZIP applications such as WinZip so should be suitable for most
    /// applications.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Setting</term>
    ///     <description>Default Value</description>
    ///   </listheader>
    ///   <item>
    ///     <term><see cref="Polynomial"/></term>
    ///     <description>The <see cref="CRC32.EthernetPolynomial"/>, 0x04C11DB7.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="InitialHash"/></term>
    ///     <description>0xFFFFFFFF</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="XorOut"/></term>
    ///     <description>0xFFFFFFFF</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="ReflectIn"/></term>
    ///     <description><strong>true</strong></description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="ReflectOut"/></term>
    ///     <description><strong>true</strong></description>
    ///   </item>
    /// </list>
    /// </remarks>
    sealed class CRC32Managed : CRC32
    {
        #region Constants

        /// <summary>
        /// The size of a byte in bits, 8.
        /// </summary>
        private const int BitsInByte = 8;

        /// <summary>
        /// The number of bytes in the hash.
        /// </summary>
        private const int BytesInHash = BitsInHash / BitsInByte;

        /// <summary>
        /// The size of the has in bits.
        /// </summary>
        private const int BitsInHash = 32;

        /// <summary>
        /// The mask to get only the values in the lowest order byte, 0xFF.
        /// </summary>
        private const uint ByteMask = 0xff;

        #endregion

        #region Fields

        /// <summary>
        /// The initial value of the hash.
        /// </summary>
        private readonly uint initialHash;

        /// <summary>
        /// The value to XOR the output with.
        /// </summary>
        private readonly uint xorOut;

        /// <summary>
        /// The polynomial to use for the calculations.
        /// </summary>
        private readonly uint polynomial;

        /// <summary>
        /// Whether the input is reflected.
        /// </summary>
        private readonly bool reflectIn;

        /// <summary>
        /// Whether the output is reflected.
        /// </summary>
        private readonly bool reflectOut;

        /// <summary>
        /// Holds the hash value during the computation.
        /// </summary>
        private uint hash;

        /// <summary>
        /// The lookup table containing precomputed values of a byte divided by the polynomial using
        /// crc arithmetic (i.e. no carry).
        /// </summary>
        private uint[] table;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new CRC32 class with default settings.
        /// </summary>
        public CRC32Managed()
            : this(CRC32.EthernetPolynomial)
        {
        }

        /// <summary>
        /// Creates a new CRC32 algorithm with specified polynomial.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial to use for the calculations.
        /// </param>
        public CRC32Managed(int polynomial)
            : this(polynomial, unchecked((int)uint.MaxValue))
        {
        }

        /// <summary>
        /// Creates a new CRC32 algorithm with specified polynomial and initial hash.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial to use for the calculations.
        /// </param>
        /// <param name="initialHash">
        /// The initial value of the hash.
        /// </param>
        public CRC32Managed(int polynomial, int initialHash)
            : this(polynomial, initialHash, initialHash)
        {
        }

        /// <summary>
        /// Creates a new CRC32 algorithm with specified polynomial, initial hash, and XOR output.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial to use for the calculations.
        /// </param>
        /// <param name="initialHash">
        /// The initial value of the hash.
        /// </param>
        /// <param name="xorOut">
        /// The value to XOR the output with.
        /// </param>
        public CRC32Managed(int polynomial, int initialHash, int xorOut)
             : this(polynomial, initialHash, xorOut, true, true)      
        {
        }

        /// <summary>
        /// Creates a new CRC32 algorithm with specified polynomial, initial hash, XOR output and reflection.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial to use for the calculations.
        /// </param>
        /// <param name="initialHash">
        /// The initial value of the hash.
        /// </param>
        /// <param name="xorOut">
        /// The value to XOR the output with.
        /// </param>
        /// <param name="reflectIn">
        /// <strong>true</strong> to reflect the input bytes, often known as the reflected or hardware-mode algorithm, 
        /// or <strong>false</strong> to use the input bytes as is, often known as the normal or software-mode algorithm.
        /// </param>
        /// <param name="reflectOut">
        /// <strong>true</strong> to reflect the output bytes, or <strong>false</strong> to use them as is. This should normally
        /// be the same as <see cref="reflectIn"/>.
        /// </param>
        public CRC32Managed(int polynomial, int initialHash, int xorOut, bool reflectIn, bool reflectOut)
        {
            unchecked
            {
                this.polynomial = (uint)polynomial;
                this.initialHash = (uint)initialHash;
                this.xorOut = (uint)xorOut;
            }
            this.reflectIn = reflectIn;
            this.reflectOut = reflectOut;
            this.HashSizeValue = BitsInHash;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the initial value of the hash before any computations.
        /// </summary>
        /// <value>
        /// The initial value of the hash. The default is 0xFFFFFFFF.
        /// </value>
        public int InitialHash
        {
            get
            {
                return unchecked((int)this.initialHash);
            }
        }

        /// <summary>
        /// Gets the polynomial to use for the calculations.
        /// </summary>
        /// <value>
        /// The initial value of the hash. The default is <see cref="CRC32.EthernetPolynomial"/>.
        /// </value>
        public int Polynomial
        {
            get
            {
                return unchecked((int)this.polynomial);
            }
        }

        /// <summary>
        /// Gets whether the input bytes are reflected.
        /// </summary>
        /// <value>
        /// <strong>true</strong> if the bytes are reflected, or <strong>false</strong> otherwise. The default is <strong>true</strong>.
        /// </value>
        public bool ReflectIn
        {
            get
            {
                return this.reflectIn;
            }
        }

        /// <summary>
        /// Gets whether the output bytes are reflected.
        /// </summary>
        /// <value>
        /// <strong>true</strong> if the bytes are reflected, or <strong>false</strong> otherwise. The default is <strong>true</strong>.
        /// </value>
        public bool ReflectOut
        {
            get
            {
                return this.reflectOut;
            }
        }

        /// <summary>
        /// Gets the value that the output will be XOR'd with. Use zero for no XOR effect.
        /// </summary>
        /// <value>
        /// In integer that will be XOR'd with the output. The default is 0xFFFFFFFF.
        /// </value>
        public int XorOut
        {
            get
            {
                return unchecked((int)this.xorOut);
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes the algorithm.
        /// </summary>
        public override void Initialize()
        {
            // if we're reflecting the input then reflect the initial hash - this probably has no
            // effect as it's usually all 1 or all 0 but still worth doing just in case
            this.hash = (this.reflectIn) ? ReflectBits(this.initialHash, BitsInHash) : this.initialHash;

            // build the lookup table
            if (this.table == null)
            {
                this.BuildLookupTable();
            }
        }

        /// <summary>
        /// Incorporates the data in the buffer into the hash.
        /// </summary>
        /// <param name="array">
        /// The buffer of data.
        /// </param>
        /// <param name="ibStart">
        /// The offset into <paramref name="array"/> at which to begin.
        /// </param>
        /// <param name="cbSize">
        /// The number of bytes from <paramref name="array"/> to include.
        /// </param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            // ensure we are initialized
            if (this.table == null)
            {
                this.Initialize();
            }

            // compute the hash
            if (this.reflectIn)
            {
                for (int i = ibStart; i < cbSize; i++)
                {
                    // get the lookup table index by xoring the bottom byte of the hash with the message byte
                    int index = (int)((this.hash ^ array[i]) & ByteMask);

                    // right shift the register by one byte and xor with the table entry
                    this.hash = (this.hash >> BitsInByte) ^ this.table[index];
                }
            }
            else
            {
                for (int i = ibStart; i < cbSize; i++)
                {
                    // get the lookup table index by xoring the top byte of the hash with the message byte
                    int index = (int)(((this.hash >> (BitsInHash - BitsInByte)) ^ array[i]) & ByteMask);

                    // left shift the register by one byte and xor with the table entry
                    this.hash = (this.hash << BitsInByte) ^ this.table[index];
                }
            }
        }

        /// <summary>
        /// Finalizes the hash computation after all data has been processed.
        /// </summary>
        /// <returns>
        /// The hash value.
        /// </returns>
        protected override byte[] HashFinal()
        {
            // reflect the output if only one of reflect in/out was specified as if both
            // were we have already catered for it when calculating the lookup table
            if (this.reflectIn ^ this.reflectOut)
            {
                this.hash = ReflectBits(this.hash, BitsInHash);
            }

            // XOR the final value
            this.hash = this.hash ^ this.xorOut;

            // get the output bytes
            byte[] finalHash = new byte[BytesInHash];    
            for (int i = 0, shift = BitsInHash - BitsInByte; i < finalHash.Length; i++, shift -= BitsInByte)
            {
                finalHash[i] = (byte)((this.hash >> shift) & ByteMask);
            }
            return finalHash;
        }

        /// <summary>
        /// Reflects a number of the least significant bits.
        /// </summary>
        /// <param name="value">
        /// The value whose bits should be reflected.
        /// </param>
        /// <param name="count">
        /// The number of bits to reflect.
        /// </param>
        /// <returns>
        /// The number with the bits reflected.
        /// </returns>
        /// <remarks>
        /// This function swaps a number of the least significant bits in the value, for example if 
        /// count is 8 then it swaps 0 with 7, 1 with 6 etc. Any bits in a more significant position
        /// than count are not touched.
        /// </remarks>
        private static uint ReflectBits(uint value, int count)
        {
            uint reflected = value;
            for (int i = 1; i <= count; i++)
            {
                if ((value & (uint)1) == 1)
                {
                    reflected |= (uint)1 << (count - i);
                }
                else
                {
                    reflected &= ~((uint)1 << (count - i));
                }
                value >>= 1;
            }
            return reflected;
        }

        /// <summary>
        /// Builds a lookup table.
        /// </summary>
        private void BuildLookupTable()
        {
            // the mask to get the top bit of a byte
            const uint TopBitMask = (uint)1 << (BitsInHash - 1);

            // loop through each of the possible 256 values of a byte
            this.table = new uint[0x100];
            for (int i = 0; i < this.table.Length; i++)
            {
                // get the initial value for this byte - if reflecting input we reflect the lowest byte
                // so the most significant bit is first before shifting it into the top byte
                uint val = (this.reflectIn) ? ReflectBits((uint)i, BitsInByte) : (uint)i;
                val <<= (BitsInHash - BitsInByte);

                // loop through each bit in the top byte
                for (int j = 0; j < BitsInByte; j++)
                {
                    if ((val & TopBitMask) != 0)
                    {
                        // most significant bit is 1 so xor with the polynomial
                        val = ((val << 1) ^ this.polynomial);
                    }
                    else
                    {
                        // most significant bit is 0 so do not xor
                        val <<= 1;
                    }
                }

                // reflect all bits if we are reflecting the input and store in the lookup table
                this.table[i] = (this.reflectIn) ? ReflectBits(val, BitsInHash) : val;
            }
        }

        #endregion
    }
}
