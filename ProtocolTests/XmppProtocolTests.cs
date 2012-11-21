using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.XMPP;

namespace ProtocolTests
{
	[TestClass]
	public class XmppProtocolTests
	{
		[TestMethod]
		public void JidParseTest()
		{
			Jid jid = Jid.Parse("Test@test.com");
			Assert.AreEqual("Test", jid.Username);
			Assert.AreEqual("test.com", jid.Server);
			Assert.IsNull(jid.Resource);
		}
	}
}