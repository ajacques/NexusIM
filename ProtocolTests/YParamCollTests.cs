using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.Yahoo;

namespace ProtocolTests
{
	[TestClass]
	public class YParamCollTests
	{
		[TestMethod]
		public void AddTest()
		{
			YPacketParamCollection acc = new YPacketParamCollection();
			acc.Add(1, "value1");
			acc.Add(new KeyValuePair<int,string>(2, "value2"));

			Assert.IsTrue(acc.ContainsKey(1));
			Assert.IsTrue(acc.ContainsKey(2));
		}

		[TestMethod]
		public void RemoveTest()
		{
			YPacketParamCollection acc = new YPacketParamCollection();
			acc.Add(1, "value1");
			Assert.IsTrue(acc.ContainsKey(1));
			Assert.IsTrue(acc.Remove(1));
			Assert.IsFalse(acc.ContainsKey(1));
		}
	}
}