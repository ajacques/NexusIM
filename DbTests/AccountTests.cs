using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusCore.Databases;
using System.Security.Cryptography;

namespace DbTests
{
	[TestClass]
	public class AccountTests
	{
		[TestMethod]
		public void AccountDecryptionTest()
		{
			Account acc = new Account();
			acc.password = new byte[] { 219, 31, 76, 81, 44, 251, 160, 215, 199, 235, 49, 63, 104, 207, 153, 96, 114, 40, 180, 122, 19, 44, 77, 241, 196, 222, 168, 244, 59, 97, 114, 247, 231, 15, 15, 155, 100, 31, 33, 17, 69, 210, 198, 103, 151, 240, 234, 221 };
			acc.id = 3;

			HashAlgorithm algr = new SHA256Managed();
			byte[] pwdhash = algr.ComputeHash(Encoding.UTF8.GetBytes("test"));

			string decryptPass = acc.DecryptPassword(pwdhash);

			Assert.AreEqual("testpassword", decryptPass);
		}

		[TestMethod]
		public void AccountEncryptionTest()
		{
			Account acc = new Account();
			acc.id = 2;

			HashAlgorithm algr = new SHA256Managed();
			byte[] pwdhash = algr.ComputeHash(Encoding.UTF8.GetBytes("test"));

			acc.ChangePassword(pwdhash, "testpassword");

			Assert.AreEqual("testpassword", acc.DecryptPassword(pwdhash));
		}
	}
}