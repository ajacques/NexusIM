using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM
{
	public class SQLCESettings : ISettings
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
				protocol.ConfigurationSettings = ProtocolSettings[protocol];

				IMProtocolExtraData extraData = new IMProtocolExtraData();
				extraData.DatabaseId = account.Id;
				extraData.Protocol = protocol;
				extraData.Enabled = account.Enabled;
				AccountManager.AddNewAccount(extraData);
			}
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

		// Nested Classes
		private class SqlDictionary : IDictionary<string, string>
		{
			public SqlDictionary(string connectionString)
			{
				mConnectionString = connectionString;
			}

			#region IDictionary<string,string> Members

			public void Add(string key, string value)
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				Setting setting = new Setting();
				setting.Key = key;
				setting.Value = value;

				db.Settings.InsertOnSubmit(setting);
				db.SubmitChanges();
				db.Dispose();
			}
			public bool ContainsKey(string key)
			{
				UserProfile db = UserProfile.Create(mConnectionString);

				return db.Settings.Any(s => s.Key == key);
			}
			public ICollection<string> Keys
			{
				get {
					UserProfile db = UserProfile.Create(mConnectionString);

					return db.Settings.Select(s => s.Key).ToList();
				}
			}
			public bool Remove(string key)
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

				if (setting == null)
					return false;

				db.Settings.DeleteOnSubmit(setting);
				db.SubmitChanges();

				return true;
			}
			public bool TryGetValue(string key, out string value)
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

				value = null;
				if (setting == null)
					return false;

				value = setting.Value;
				return true;
			}
			public ICollection<string> Values
			{
				get {
					UserProfile db = UserProfile.Create(mConnectionString);

					return db.Settings.Select(s => s.Value).ToList();
				}
			}
			public string this[string key]
			{
				get	{
					UserProfile db = UserProfile.Create(mConnectionString);

					Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

					if (setting == null)
						return null;

					return setting.Value;
				}
				set	{
					UserProfile db = UserProfile.Create(mConnectionString);

					Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

					if (setting == null)
						Add(key, value);
					else {
						setting.Value = value;
						db.SubmitChanges();
					}
				}
			}

			#endregion

			#region ICollection<KeyValuePair<string,string>> Members

			public void Add(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}
			public void Clear()
			{
				throw new NotImplementedException();
			}
			public bool Contains(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}
			public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}
			public int Count
			{
				get {
					UserProfile db = UserProfile.Create(mConnectionString);

					return db.Settings.Count();
				}
			}
			public bool IsReadOnly
			{
				get {
					return false;
				}
			}
			public bool Remove(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable<KeyValuePair<string,string>> Members

			public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
			{
				UserProfile db = UserProfile.Create(mConnectionString);

				return db.Settings.Select(s => new KeyValuePair<string, string>(s.Key, s.Value)).GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion

			private string mConnectionString;
		}
		private class SqlAccountSettingDictionary : IDictionary<string, string>
		{
			public SqlAccountSettingDictionary(DataContext context, EntitySet<AccountSetting> source)
			{
				mSource = source;
				mContext = context;
			}

			#region IDictionary<string,string> Members

			public void Add(string key, string value)
			{
				AccountSetting setting = new AccountSetting();
				setting.Key = key;
				setting.Value = value;

				mSource.Add(setting);
				mContext.SubmitChanges();
			}
			public bool ContainsKey(string key)
			{
				return mSource.Any(ps => ps.Key == key);
			}
			public ICollection<string> Keys
			{
				get {
					return mSource.Select(ps => ps.Key).ToList();
				}
			}
			public bool Remove(string key)
			{
				throw new NotImplementedException();
			}
			public bool TryGetValue(string key, out string value)
			{
				AccountSetting setting = mSource.FirstOrDefault(ps => ps.Key == key);

				if (setting == null)
				{
					value = null;
					return false;
				}

				value = setting.Value;
				return true;
			}
			public ICollection<string> Values
			{
				get {
					return mSource.Select(ps => ps.Value).ToList();
				}
			}
			public string this[string key]
			{
				get	{
					AccountSetting setting = mSource.FirstOrDefault(ps => ps.Key == key);
					if (setting == null)
						return null;

					return setting.Value;
				}
				set	{
					AccountSetting setting = mSource.FirstOrDefault(ps => ps.Key == key);

					if (setting == null)
					{
						setting = new AccountSetting();
						setting.Key = key;
						mSource.Add(setting);
					}

					setting.Value = value;
					mContext.SubmitChanges();
				}
			}

			#endregion

			#region ICollection<KeyValuePair<string,string>> Members

			public void Add(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}
			public void Clear()
			{
				throw new NotImplementedException();
			}
			public bool Contains(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}
			public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public int Count
			{
				get {
					return mSource.Count;
				}
			}
			public bool IsReadOnly
			{
				get {
					return false;
				}
			}
			public bool Remove(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable<KeyValuePair<string,string>> Members

			public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			private DataContext mContext;
			private EntitySet<AccountSetting> mSource;
		}
		private class SqlProtocolDictionary : IDictionary<IMProtocol, IDictionary<string, string>>
		{
			public SqlProtocolDictionary(string connectionString)
			{
				mConnectionString = connectionString;
			}

			#region IDictionary<IMProtocol,IDictionary<string,string>> Members

			public void Add(IMProtocol key, IDictionary<string, string> value)
			{
				throw new NotSupportedException();
			}
			public bool ContainsKey(IMProtocol key)
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				return db.Accounts.Any(s => s.Username == key.Username && s.AccountType == key.ShortProtocol);
			}
			public ICollection<IMProtocol> Keys
			{
				get {
					throw new NotSupportedException();
				}
			}
			public bool Remove(IMProtocol key)
			{
				throw new NotImplementedException();
			}
			public bool TryGetValue(IMProtocol key, out IDictionary<string, string> value)
			{
				if (ContainsKey(key))
				{
					value = this[key];
					return true;
				} else {
					value = null;
					return false;
				}
			}
			public ICollection<IDictionary<string, string>> Values
			{
				get {
					throw new NotSupportedException();
				}
			}
			public IDictionary<string, string> this[IMProtocol key]
			{
				get	{
					UserProfile db = UserProfile.Create(mConnectionString);
					Account setting = db.Accounts.FirstOrDefault(ps => ps.Username == key.Username && ps.AccountType == key.ShortProtocol);

					if (setting == null)
						return null;
					
					return new SqlAccountSettingDictionary(db, setting.AccountSettings);
				}
				set	{
					throw new NotSupportedException();
				}
			}

			#endregion

			#region ICollection<KeyValuePair<IMProtocol,IDictionary<string,string>>> Members

			public void Add(KeyValuePair<IMProtocol, IDictionary<string, string>> item)
			{
				throw new NotImplementedException();
			}
			public void Clear()
			{
				throw new NotImplementedException();
			}
			public bool Contains(KeyValuePair<IMProtocol, IDictionary<string, string>> item)
			{
				throw new NotImplementedException();
			}
			public void CopyTo(KeyValuePair<IMProtocol, IDictionary<string, string>>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}
			public int Count
			{
				get {
					UserProfile db = UserProfile.Create(mConnectionString);
					return db.Accounts.Count();
				}
			}
			public bool IsReadOnly
			{
				get {
					return true;
				}
			}
			public bool Remove(KeyValuePair<IMProtocol, IDictionary<string, string>> item)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable<KeyValuePair<IMProtocol,IDictionary<string,string>>> Members

			public IEnumerator<KeyValuePair<IMProtocol, IDictionary<string, string>>> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			private string mConnectionString;
		}
		private class SqlAccountList : IList<IMProtocol>
		{
			public SqlAccountList(string connectionString)
			{
				mConnectionString = connectionString;
			}

			private class SqlAccountEnumerator : IEnumerator<IMProtocol>
			{
				public SqlAccountEnumerator(DataContext context, Table<Account> source)
				{
					mEnumerator = source.GetEnumerator();
					mContext = context;
				}

				#region IEnumerator<IMProtocol> Members

				public IMProtocol Current
				{
					get {
						Account current = mEnumerator.Current;
						if (current == null)
							return null;

						IMProtocol protocol = IMProtocol.FromString(current.AccountType);
						protocol.Username = current.Username;
						protocol.Password = current.Password;
						protocol.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) => {
							current.Username = protocol.Username;
							current.Password = protocol.Password;
							mContext.SubmitChanges();
						});
						protocol.ConfigurationSettings = new SqlAccountSettingDictionary(mContext, current.AccountSettings);

						return protocol;
					}
				}

				#endregion

				#region IDisposable Members

				public void Dispose()
				{
					mContext = null;
					mEnumerator.Dispose();
				}

				#endregion

				#region IEnumerator Members

				object System.Collections.IEnumerator.Current
				{
					get {
						return Current;
					}
				}
				public bool MoveNext()
				{
					return mEnumerator.MoveNext();
				}
				public void Reset()
				{
					mEnumerator.Reset();
				}

				#endregion

				private DataContext mContext;
				private IEnumerator<Account> mEnumerator;
			}

			#region IEnumerable<IMProtocol> Members

			public IEnumerator<IMProtocol> GetEnumerator()
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				return new SqlAccountEnumerator(db, db.Accounts);
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion

			#region IList<IMProtocol> Members

			public int IndexOf(IMProtocol item)
			{
				throw new NotImplementedException();
			}
			public void Insert(int index, IMProtocol item)
			{
				throw new NotImplementedException();
			}
			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}
			public IMProtocol this[int index]
			{
				get	{
					throw new NotImplementedException();
				}
				set	{
					throw new NotImplementedException();
				}
			}

			#endregion

			#region ICollection<IMProtocol> Members

			public void Add(IMProtocol item)
			{
				UserProfile db = UserProfile.Create(mConnectionString);

				Account account = new Account();
				account.AccountType = item.ShortProtocol;
				account.Username = item.Username;
				account.Password = item.Password;

				foreach (var setting in item.ConfigurationSettings)
				{
					AccountSetting toInsert = new AccountSetting();
					toInsert.Key = setting.Key;
					toInsert.Value = setting.Value;
					toInsert.Account = account;

					account.AccountSettings.Add(toInsert);
				}

				db.Accounts.InsertOnSubmit(account);

				item.ConfigurationSettings = new SqlAccountSettingDictionary(db, account.AccountSettings);

				db.SubmitChanges();
			}
			public void Clear()
			{
				throw new NotImplementedException();
			}
			public bool Contains(IMProtocol item)
			{
				throw new NotImplementedException();
			}
			public void CopyTo(IMProtocol[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}
			public int Count
			{
				get { throw new NotImplementedException(); }
			}
			public bool IsReadOnly
			{
				get {
					return false;
				}
			}
			public bool Remove(IMProtocol item)
			{
				throw new NotImplementedException();
			}

			#endregion

			private string mConnectionString;
		}

		public IList<IMProtocol> Accounts
		{
			get	{
				return new SqlAccountList(mConnectionString);
			}
		}
		public IDictionary<string, string> Settings
		{
			get	{
				return new SqlDictionary(mConnectionString);
			}
		}
		public IDictionary<IMProtocol, IDictionary<string, string>> ProtocolSettings
		{
			get	{
				return new SqlProtocolDictionary(mConnectionString);
			}
		}
		
		// Variables
		private string mConnectionString;
	}
}