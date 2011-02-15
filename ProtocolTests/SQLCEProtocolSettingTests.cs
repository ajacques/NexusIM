using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM;
using ProtocolTests.Properties;
using InstantMessage;
using InstantMessage.Protocols;
using InstantMessage.Protocols.Yahoo;

namespace ProtocolTests
{
	[TestClass]
	[DeploymentItem("UserProfile.sdf")]
	public class SQLCEProtocolSettingTests
	{
		[TestMethod]
		public void LoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			Assert.IsNotNull(settings);
			Assert.IsTrue(settings.ContainsKey("TestAccKey1"));
			Assert.AreEqual("TestAccValue1", settings["TestAccKey1"]);
		}

		[TestMethod]
		public void TryGetTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			string output;
			Assert.IsFalse(settings.TryGetValue("NonExistant", out output));
			Assert.IsTrue(settings.TryGetValue("TestAccKey1", out output));
		}

		[TestMethod]
		public void KeysTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			Assert.IsNotNull(settings.Keys);
			Assert.AreNotEqual(0, settings.Count);
		}

		[TestMethod]
		public void CountTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			Assert.IsNotNull(settings.Count);
		}

		[TestMethod]
		public void ReadOnlyTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			Assert.IsFalse(settings.IsReadOnly);
		}

		[TestMethod]
		public void ValuesTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];

			Assert.IsNotNull(settings.Values);
			Assert.IsTrue(settings.Values.Any());
		}

		[TestMethod]
		public void NewAccountTest()
		{
			SQLCESettings sqlce = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "NewUsername";
			IDictionary<string, string> settings = sqlce.ProtocolSettings[protocol];

			Assert.IsNotNull(settings);
		}
	}
}
