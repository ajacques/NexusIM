using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM;
using ProtocolTests.Properties;
using InstantMessage;
using InstantMessage.Protocols.Yahoo;

namespace ProtocolTests
{
	[TestClass]
	[DeploymentItem("UserProfile.sdf")]
	public class SQLCEProtocolSettingRootListTests
	{
		[TestMethod]
		public void ContainsProtocolTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";

			Assert.IsTrue(setting.ProtocolSettings.ContainsKey(protocol));
			protocol.Username = "TestUsername2";
			Assert.IsFalse(setting.ProtocolSettings.ContainsKey(protocol));
		}

		[TestMethod]
		public void TryGetTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername";

			IDictionary<string, string> output;
			Assert.IsTrue(setting.ProtocolSettings.TryGetValue(protocol, out output));
			Assert.IsNotNull(output);
		}

		[TestMethod]
		public void NonExistantLoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername2";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void KeysTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.ProtocolSettings.Keys);
			Assert.AreNotEqual(0, setting.ProtocolSettings.Keys.Count);
		}

		[TestMethod]
		public void CountTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.ProtocolSettings.Count);
		}

		[TestMethod]
		public void ValuesTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.ProtocolSettings.Values);
			Assert.AreNotEqual(0, setting.ProtocolSettings.Values.Count);
		}

		[TestMethod]
		public void ReadOnlyTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsTrue(setting.ProtocolSettings.IsReadOnly);
		}

		[TestMethod]
		public void EnumerateSettings()
		{
			SQLCESettings settings = new SQLCESettings(Settings.Default.UserProfileTest);
			foreach (var setting in settings.ProtocolSettings)
			{
				Assert.IsNotNull(setting.Key);
				Assert.IsNotNull(setting.Value);
			}
		}
	}
}