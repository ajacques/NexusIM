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
	public class SQLCEGenericSettingsTests
	{
		[TestMethod]
		public void LoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.AreEqual("TestValue1", setting.Settings["TestKey1"]);
			Assert.IsTrue(setting.Settings.ContainsKey("TestKey1"));
		}

		[TestMethod]
		public void TryGetTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			string output;
			Assert.IsTrue(setting.Settings.TryGetValue("TestKey1", out output));
			Assert.AreEqual("TestValue1", output);
		}

		[TestMethod]
		public void KeysTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.Settings.Keys);
			Assert.AreNotEqual(0, setting.Settings.Keys.Count);
		}

		[TestMethod]
		public void CountTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.Settings.Count);
			Assert.AreNotEqual(0, setting.Settings.Count);
		}

		[TestMethod]
		public void ValuesTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsNotNull(setting.Settings.Values);
			Assert.AreNotEqual(0, setting.Settings.Values.Count);
		}

		[TestMethod]
		public void BadSettingLoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.AreEqual(null, setting.Settings["TestKey3"]);
			Assert.IsFalse(setting.Settings.ContainsKey("TestKey3"));
		}

		[TestMethod]
		public void DeleteTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsTrue(setting.Settings.Remove("DeleteKey"));

			UserProfile profile = UserProfile.Create(Settings.Default.UserProfileTest);
			bool found = profile.Settings.Any(s => s.Key == "DeleteKey");

			Assert.IsFalse(found);
		}

		[TestMethod]
		public void ReadOnlyTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsFalse(setting.Settings.IsReadOnly);
		}

		[TestMethod]
		public void EnumerateSettings()
		{
			SQLCESettings settings = new SQLCESettings(Settings.Default.UserProfileTest);
			foreach (var setting in settings.Settings)
			{
				Assert.IsNotNull(setting.Key);
				Assert.IsNotNull(setting.Value);
			}
		}

		[TestMethod]
		public void DeleteBadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			Assert.IsFalse(setting.Settings.Remove("TestKey5"));
		}

		[TestMethod]
		public void SaveTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			setting.Settings.Add("TestKeyAdd", "TestAddValue");

			UserProfile profile = UserProfile.Create(Settings.Default.UserProfileTest);
			Setting dbsetting = profile.Settings.First(s => s.Key == "TestKeyAdd");

			Assert.AreEqual("TestAddValue", dbsetting.Value);
			Assert.IsTrue(setting.Settings.ContainsKey("TestKeyAdd"));
		}
	}
}