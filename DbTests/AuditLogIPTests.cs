using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbTests
{
	[TestClass]
	public class AuditLogIPTests
	{
		[TestMethod]
		public void IPSerializeTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			int result = db.IPAddressToInt("127.0.0.1").GetValueOrDefault();

			Assert.AreEqual(2130706433, result);
		}

		[TestMethod]
		public void IPDeserializeTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			string result = db.IntToIPAddress(2130706433);

			Assert.AreEqual("127.0.0.1", result.Trim());
		}

		[TestMethod]
		public void IPOutOfRangeTest()
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			int result = db.IPAddressToInt("256.256.256.0").GetValueOrDefault();

			Assert.AreEqual(-1, result);
		}
	}
}