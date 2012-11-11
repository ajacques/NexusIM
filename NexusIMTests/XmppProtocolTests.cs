using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Protocols.XMPP;
using System.Xml;
using System.Diagnostics;

namespace NexusIMTests
{
	[TestClass]
	public class XmppProtocolTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			StreamInitMessage msg = new StreamInitMessage("example.com");
			StringBuilder builder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder);
			msg.WriteMessage(writer);
			Trace.WriteLine(builder.ToString());
		}
	}
}
