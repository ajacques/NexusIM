﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM;
using InstantMessage;
using ProtocolTests.Properties;
using System.Collections;

namespace ProtocolTests
{
	[TestClass]
	[DeploymentItem("UserProfile.sdf")]
	public class SQLCEProtocolListTests
	{
		[TestMethod]
		public void LoadTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			ICollection<IMProtocolWrapper> protocols = setting.Accounts;
			Assert.IsNotNull(protocols);
		}

		[TestMethod]
		public void IndexTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			//Assert.IsNotNull(setting.Accounts[0]);
		}

		[TestMethod]
		public void EnumerateTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IEnumerator<IMProtocolWrapper> enumer = setting.Accounts.GetEnumerator();
			Assert.IsNotNull(enumer);

			while (enumer.MoveNext())
			{
				IMProtocolWrapper protocol = enumer.Current;
				Assert.IsNotNull(protocol);
				Assert.IsNotNull(protocol.Protocol);
				Assert.IsNotNull(protocol.Protocol.ConfigurationSettings);
			}
		}

		[TestMethod]
		public void Enumerate2Test()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IEnumerator enumer = setting.Accounts.GetEnumerator();
			Assert.IsNotNull(enumer);

			while (enumer.MoveNext())
			{
				object protocol = enumer.Current;
				Assert.IsNotNull(protocol);
			}
		}

		[TestMethod]
		public void AddAccountTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);

			IMProtocol protocol = new IMProtocol();
			protocol.Username = "GenericUsername";
			setting.Accounts.Add(new IMProtocolWrapper() { Protocol = protocol });

			UserProfile db = UserProfile.Create(Settings.Default.UserProfileTest);
			Assert.IsTrue(db.Accounts.Any(a => a.Username == "GenericUsername" && a.AccountType == "Default"));
			Assert.IsNotNull(protocol.ConfigurationSettings);
		}

		[TestMethod]
		public void NotifyChangeTest()
		{
			SQLCESettings setting = new SQLCESettings(Settings.Default.UserProfileTest);
			IMProtocolWrapper protocol = setting.Accounts.First(a => a.Protocol.Username == "TestUpdate");
			protocol.Protocol.Username = "TestNewUpdate";

			UserProfile db = UserProfile.Create(Settings.Default.UserProfileTest);
			Assert.IsTrue(db.Accounts.Any(a => a.AccountType == "default" && a.Username == "TestNewUpdate"));
		}
	}
}
