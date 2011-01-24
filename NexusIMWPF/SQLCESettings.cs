using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Data;
using System.Data.SqlClient;
using NexusIM.Managers;
using System.ComponentModel;

namespace NexusIM
{
	class SQLCESettings : ISettings
	{
		public SQLCESettings(string connectionString)
		{
			mConnectionString = connectionString;
		}

		public void Load()
		{
			UserProfile mDb = UserProfile.Create(mConnectionString);
			foreach (Account account in mDb.Accounts)
			{
				IMProtocol protocol = InterfaceManager.CreateProtocol(account.AccountType);
				protocol.Username = account.Username;
				protocol.Password = account.Password;
				foreach (AccountSetting setting in account.AccountSettings)
					protocol.ConfigurationSettings.Add(setting.Key, setting.Value);

				IMProtocolExtraData extraData = new IMProtocolExtraData();
				extraData.DatabaseId = account.Id;
				extraData.Protocol = protocol;
				extraData.Enabled = account.Enabled;
				AccountManager.AddNewAccount(extraData);
			}
			IsLoaded = true;
			if (onFileLoad != null)
				onFileLoad(this, null);
		}
		public void Save()
		{
			UserProfile mDb = UserProfile.Create(mConnectionString);

			var accDb = mDb.Accounts.AsEnumerable();
			// This code below is a Full Outer Join
			var query = from a in AccountManager.Accounts // Get all the accounts that haven't been added yet
							join d in accDb on a.DatabaseId equals d.Id into tempDb
							from db in tempDb.DefaultIfEmpty()
							select new { Local = a, Database = db };
			query = query.Union(from d in accDb // Now union all the accounts that have been deleted locally to the list
									join a in AccountManager.Accounts on d.Id equals a.DatabaseId into tempLocal
									from a2 in tempLocal.DefaultIfEmpty()
									select new { Local = a2, Database = d});

			foreach (var protocol in query)
			{
				if (protocol.Local == null) // Delete it
				{
					mDb.Accounts.DeleteOnSubmit(protocol.Database);
					continue;
				}

				if (protocol.Database == null)
				{
					Account acc = new Account();
					acc.AccountType = protocol.Local.Protocol.ShortProtocol;
					acc.Username = protocol.Local.Protocol.Username;
					acc.Password = protocol.Local.Protocol.Password;
					PropertyChangedEventHandler handler = null;
					handler = new PropertyChangedEventHandler(delegate(object sender, PropertyChangedEventArgs args)
					{
						if (args.PropertyName == "Id")
						{
							protocol.Local.DatabaseId = acc.Id;
							acc.PropertyChanged -= handler; // This will be declared by the time we get to it
						}
					});
					acc.PropertyChanged += handler;

					foreach (var setting in protocol.Local.Protocol.ConfigurationSettings)
					{
						AccountSetting toInsert = new AccountSetting();
						toInsert.Key = setting.Key;
						toInsert.Value = setting.Value;
						toInsert.Account = acc;
						
						acc.AccountSettings.Add(toInsert);
					}

					mDb.Accounts.InsertOnSubmit(acc);
					continue;
				}

				var settingCache = protocol.Database.AccountSettings.AsEnumerable();				
				var settingQuery = from l in protocol.Local.Protocol.ConfigurationSettings
							join d in settingCache on l.Key equals d.Key into tempDb
							from db in tempDb.DefaultIfEmpty()
							select new { Local = l, Database = db };
				settingQuery = settingQuery.Union(from d in settingCache
									join l in protocol.Local.Protocol.ConfigurationSettings on d.Key equals l.Key into tempLocal
									from a2 in tempLocal.DefaultIfEmpty()
									select new { Local = a2, Database = d });

				foreach (var setting in settingQuery)
				{
					if (setting.Database == null)
					{
						AccountSetting toInsert = new AccountSetting();
						toInsert.Key = setting.Local.Key;
						toInsert.Value = setting.Local.Value;
						toInsert.Account = protocol.Database;

						protocol.Database.AccountSettings.Add(toInsert);
						continue;
					}

					setting.Database.Key = setting.Local.Key;
					setting.Database.Value = setting.Local.Value;
				}
			}

			mDb.SubmitChanges();
		}
		public bool AutoSave
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}
		public bool IsLoaded
		{
			get;
			private set;
		}

		public Dictionary<IMBuddy, Dictionary<string, string>> ContactSettings
		{
			get { throw new NotImplementedException(); }
		}
		public void SetContactSetting(IMBuddy buddy, string setting, string value)
		{
			throw new NotImplementedException();
		}
		public void SetContactSetting(string username, IMProtocol protocol, string setting, string value)
		{
			throw new NotImplementedException();
		}
		public void SetCustomSetting(string setting, string value)
		{
			UserProfile profile = UserProfile.Create(mConnectionString);
			Setting config = profile.Settings.FirstOrDefault(s => s.Key == setting);
			if (config == null)
			{
				config = new Setting();
				config.Key = setting;
				profile.Settings.InsertOnSubmit(config);
			}

			config.Value = value;
			profile.SubmitChanges();
		}
		public void SetSettingList(string name, List<string> list)
		{
			throw new NotImplementedException();
		}
		public void DeleteContactSetting(IMBuddy buddy, string setting)
		{
			throw new NotImplementedException();
		}
		public void DeleteContactSetting(string username, IMProtocol protocol, string setting)
		{
			throw new NotImplementedException();
		}
		public void DeleteCustomSetting(string setting)
		{
			throw new NotImplementedException();
		}
		public void DeleteSettingList(string list)
		{
			throw new NotImplementedException();
		}
		public string GetContactSetting(string userName, IMProtocol protocol, string setting, string defaultValue)
		{
			throw new NotImplementedException();
		}
		public string GetContactSetting(IMBuddy buddy, string setting, string defaultValue)
		{
			throw new NotImplementedException();
		}
		public string GetCustomSetting(string setting, string defaultValue)
		{
			throw new NotImplementedException();
		}
		public List<string> GetSettingList(string list)
		{
			throw new NotImplementedException();
		}

		public event EventHandler onFileLoad;

		private string mConnectionString;
	}
}
