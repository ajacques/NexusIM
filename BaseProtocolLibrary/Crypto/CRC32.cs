//-----------------------------------------------------------------------
// <copyright file="CRC32.cs" company="http://gregbeech.com">
// Copyright © Greg Beech.  All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Computes a CRC32 hash for the input data.
// </summary>
//-----------------------------------------------------------------------

namespace System.Security.Cryptography
{
    using System.Security.Cryptography;

    /// <summary>
    /// Computes a <strong>CRC32</strong> hash for the input data.
    /// </summary>   
    abstract class CRC32 : HashAlgorithm
    {
        #region Constants

        /// <summary>
        /// The polynomial used by the Ethernet specification.
        /// </summary>
        public const int EthernetPolynomial = 0x04c11db7;

        /// <summary>
        /// The polynomial used by the PNG image specification.
        /// </summary>
        public const int PngPolynomial = unchecked((int)0xedb88320);

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the algorithm.
        /// </summary>
        protected CRC32()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of the default implementation of <see cref="CRC32"/>.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="CRC32"/>.
        /// </returns>
        public static new CRC32 Create()
        {
            return Create("GregBeech.Security.Cryptography.CRC32");
        }

        /// <summary>
        /// Creates an instance of the specified implementation of <see cref="CRC32"/>.
        /// </summary>
        /// <param name="hashName">
        /// The name of the specific implementation of <see cref="CRC32"/> to be used.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="CRC32"/>.
        /// </returns>
        public static new CRC32 Create(string hashName)
        {
            CRC32 algorithm = (CRC32)CryptoConfig.CreateFromName(hashName);
            if (algorithm == null)
            {
                algorithm = new CRC32Managed();
            }
            return algorithm;
        }

        #endregion
    }
}
