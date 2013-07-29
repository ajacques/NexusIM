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

		[TestMethod]
		public void ParseServerTest()
		{
			Jid jid = Jid.Parse("test.com");
			Assert.AreEqual("test.com", jid.Server);
		}

        [TestMethod]
        public void ParseEquivalentJids()
        {
            Jid jid = Jid.Parse("Test@test.com/resource1");
            Jid jid2 = new Jid("Test", "test.com", "resource1");
            Assert.AreEqual(jid, jid2);
			Assert.AreEqual(jid.GetHashCode(), jid2.GetHashCode());

            Jid jid3 = Jid.Parse("Test@test.com/resource2");
            Assert.AreNotEqual(jid, jid3);
			Assert.AreNotEqual(jid.GetHashCode(), jid3.GetHashCode(), "Expected 'Test@test.com/resource1' and 'Test@test.com/resource2' to have different hash codes");
            Assert.IsTrue(jid.Equals(jid3, true));
        }
	}
}