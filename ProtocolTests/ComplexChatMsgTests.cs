using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage;

namespace ProtocolTests
{
	[TestClass]
	public class ComplexChatMsgTests
	{
		[TestMethod]
		public void LineBreakTest()
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			msg.Inlines.Add(new IMLineBreak());

			Assert.AreEqual(Environment.NewLine, msg.ToString());
		}

		[TestMethod]
		public void RunTest()
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			msg.Inlines.Add(new IMRun("Test"));

			Assert.AreEqual("Test", msg.ToString());
		}

		[TestMethod]
		public void RunNewLineTest()
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			msg.Inlines.Add(new IMRun("Test"));
			msg.Inlines.Add(new IMLineBreak());

			Assert.AreEqual("Test" + Environment.NewLine, msg.ToString());
		}

		[TestMethod]
		public void MultiRunTest()
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			msg.Inlines.Add(new IMRun("Test "));
			msg.Inlines.Add(new IMRun("message"));

			Assert.AreEqual("Test message", msg.ToString());
		}

		[TestMethod]
		public void HyperlinkTest()
		{
			ComplexChatMessage msg = new ComplexChatMessage();
			msg.Inlines.Add(new HyperlinkInline(new Uri("http://nexus-im.com"), "NexusIM"));

			Assert.AreEqual("NexusIM (http://nexus-im.com/)", msg.ToString());
		}
	}
}