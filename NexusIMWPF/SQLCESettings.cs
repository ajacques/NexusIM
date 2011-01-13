using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Data;
using System.Data.SqlClient;
using NexusIM.Managers;

namespace NexusIM
{
	class SQLCESettings : ISettings
	{
		public SQLCESettings(string connectionString)
		{
			mAccounts = new List<IMProtocol>();
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
				Accounts.Add(protocol);
			}
		}
		public void Save()
		{
			
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
			get {
				return true;
			}
		}
		public List<IMProtocol> Accounts
		{
			get	{
				return mAccounts;
			}
			set	{
				mAccounts = value;
			}
		}
		public Dictionary<IMBuddy, Dictionary<string, string>> ContactSettings
		{
			get { throw new NotImplementedException(); }
		}
		public event EventHandler onFileLoad;
		public void SetAccountSetting(IMProtocol account, string setting, string value)
		{
			throw new NotImplementedException();
		}
		public void SetAccountSetting(IMProtocol account, string setting, string value, SettingAttributes attrib)
		{
			throw new NotImplementedException();
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
		public void DeleteAccountSetting(IMProtocol account, string setting)
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
		public string GetAccountSetting(IMProtocol account, string setting, string defaultValue)
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

		private string mConnectionString;
		private List<IMProtocol> mAccounts;
	}
}
