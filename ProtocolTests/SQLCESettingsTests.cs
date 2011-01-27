using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM;
using ProtocolTests.Properties;
using InstantMessage;

namespace ProtocolTests
{
	[TestClass]
	[DeploymentItem("UserProfile.sdf")]
	public class SQLCESettingsTests
	{
		[TestMethod]
		public void GenericSettingLoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.AreEqual("TestValue1", setting.Settings["TestKey1"]);
			Assert.IsTrue(setting.Settings.ContainsKey("TestKey1"));
		}

		[TestMethod]
		public void GenericBadSettingLoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.AreEqual(null, setting.Settings["TestKey3"]);
			Assert.IsFalse(setting.Settings.ContainsKey("TestKey3"));
		}

		[TestMethod]
		public void GenericSettingSaveTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			setting.Settings.Add("TestKeyAdd", "TestAddValue");

			UserProfile profile = UserProfile.Create(Settings.Default.UserProfileTest);
			Setting dbsetting = profile.Settings.First(s => s.Key == "TestKeyAdd");

			Assert.AreEqual("TestAddValue", dbsetting.Value);
			Assert.IsTrue(setting.Settings.ContainsKey("TestKeyAdd"));
			profile.Settings.DeleteOnSubmit(dbsetting);
			profile.SubmitChanges();
		}

		[TestMethod]
		public void ProtocolSettingLoadTest()
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
		public void ProtocolSettingNonExistantLoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocol protocol = new IMYahooProtocol();
			protocol.Username = "TestUsername2";
			IDictionary<string, string> settings = setting.ProtocolSettings[protocol];
		}
	}
}