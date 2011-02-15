using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage;
using InstantMessage.Protocols;
using InstantMessage.Protocols.Yahoo;

namespace ProtocolTests
{
	[TestClass]
	public class IMProtocolTests
	{
		[TestMethod]
		public void FromStringTest()
		{
			Assert.IsInstanceOfType(IMProtocol.FromString("yahoo"), typeof(IMYahooProtocol));
			Assert.IsNull(IMProtocol.FromString("bad"));
		}
	}
}
