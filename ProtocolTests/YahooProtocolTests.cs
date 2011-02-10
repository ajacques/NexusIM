using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage;

namespace ProtocolTests
{
	[TestClass]
	public class YahooProtocolTests
	{
		[TestMethod]
		public void ChatFontMessageParse()
		{
			string input = "<font face=\"Calibri\">Calibri";
			ComplexChatMessage output = IMYahooProtocol_Accessor.ParseMessage(input);
			
			Assert.AreEqual("Calibri", output.ToString());
			Assert.AreEqual("Calibri", output.Inlines[0].FontFamily);
		}

		[TestMethod]
		public void ChatSizeMessageParse()
		{
			string input = "<font size=\"5\">Calibri";
			ComplexChatMessage output = IMYahooProtocol_Accessor.ParseMessage(input);

			Assert.AreEqual("Calibri", output.ToString());
			Assert.AreEqual(5, output.Inlines[0].FontSize);
		}

		[TestMethod]
		public void CompoundMessageParse()
		{
			string input = "<font size=\"5\">part1<font size=\"12\">part2";
			ComplexChatMessage output = IMYahooProtocol_Accessor.ParseMessage(input);

			Assert.AreEqual("part1part2", output.ToString());
			Assert.AreEqual("part1", output.Inlines[0].ToString());
			Assert.AreEqual("part2", output.Inlines[1].ToString());
			Assert.AreEqual(5, output.Inlines[0].FontSize);
			Assert.AreEqual(12, output.Inlines[1].FontSize);
		}

		[TestMethod]
		public void MalformedMessageParse()
		{
			IMYahooProtocol_Accessor.ParseMessage("<font face=>part1");
			IMYahooProtocol_Accessor.ParseMessage("<font face");
			IMYahooProtocol_Accessor.ParseMessage("<font size=5");
			IMYahooProtocol_Accessor.ParseMessage("<font size=>");
			IMYahooProtocol_Accessor.ParseMessage("<font size");
			IMYahooProtocol_Accessor.ParseMessage("<fon");
		}

		[TestMethod]
		public void DualDefinitionMessageParse()
		{
			string input = "<font face=\"Calibri\" size=\"5\">part1";
			ComplexChatMessage output = IMYahooProtocol_Accessor.ParseMessage(input);

			Assert.AreEqual("part1", output.ToString());
			Assert.AreEqual("Calibri", output.Inlines[0].FontFamily);
			Assert.AreEqual(5, output.Inlines[0].FontSize);
		}

		[TestMethod]
		public void BasicMessageParse()
		{
			string input = "Test message";
			ComplexChatMessage output = IMYahooProtocol_Accessor.ParseMessage(input);

			Assert.AreEqual("Test message", output.ToString());
			Assert.IsNull(output.Inlines[0].FontFamily);
		}
	}
}
