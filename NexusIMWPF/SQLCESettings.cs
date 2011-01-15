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
				protocol.Tag = account.Id;
				protocol.Enabled = account.Enabled;
				foreach (AccountSetting setting in account.Settings)
					protocol.ConfigurationSettings.Add(setting.Key, setting.Value);

				AccountManager.AddNewAccount(protocol);
			}
			IsLoaded = true;
			if (onFileLoad != null)
				onFileLoad(this, null);
		}
		public void Save()
		{
			UserProfile mDb = UserProfile.Create(mConnectionString);

			IEnumerable<Account> toRemove = mDb.Accounts.Where(a => !AccountManager.Accounts.Any(p => p.Tag != null && ((int)p.Tag) == a.Id));
			IEnumerable<IMProtocol> toAdd = AccountManager.Accounts.Where(a => a.Tag == null);
			var toFix = from d in mDb.Accounts
						   join a in AccountManager.Accounts on d.Id equals a.Tag
						   where d.Username != a.Username && d.Password != d.Password
						   select new {Db = d, Local = a };

			mDb.Accounts.DeleteAllOnSubmit(toRemove);

			foreach (IMProtocol protocol in toAdd)
			{
				Account acc = new Account();
				acc.Username = protocol.Username;
				acc.Password = protocol.Password;
				acc.PropertyChanged += new PropertyChangedEventHandler(delegate(object sender, PropertyChangedEventArgs args)
					{
						if (args.PropertyName == "Id")
							protocol.Tag = acc.Id;
					});

				mDb.Accounts.InsertOnSubmit(acc);

				foreach (KeyValuePair<string, string> setting in protocol.ConfigurationSettings)
				{
					AccountSetting accSetting = new AccountSetting();
					accSetting.Account = acc;
					accSetting.Key = setting.Key;
					accSetting.Value = setting.Value;

					mDb.AccountSettings.InsertOnSubmit(accSetting);
				}
			}

			foreach (var fix in toFix)
			{
				fix.Db.Username = fix.Local.Username;
				fix.Db.Password = fix.Local.Password;
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
			throw new NotImplementedException();
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
