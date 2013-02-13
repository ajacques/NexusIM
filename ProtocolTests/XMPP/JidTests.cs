using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.XMPP;

namespace ProtocolTests
{
	[TestClass]
	public class JidTests
	{
		[TestMethod]
		public void ParseTest()
		{
			Jid jid = Jid.Parse("Test@test.com");
			Assert.AreEqual("Test", jid.Username);
			Assert.AreEqual("test.com", jid.Server);
			Assert.IsTrue(String.IsNullOrEmpty(jid.Resource), "Resource was not empty");
		}

		[TestMethod]
		public void ParseResourceTest()
		{
			Jid jid = Jid.Parse("Test@test.com/test");
			Assert.AreEqual("Test", jid.Username);
			Assert.AreEqual("test.com", jid.Server);
			Assert.AreEqual("test", jid.Resource);
		}
	}
}