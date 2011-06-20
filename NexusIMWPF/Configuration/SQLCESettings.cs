using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Linq;
using InstantMessage;
using InstantMessage.Protocols.Irc;
using InstantMessage.Protocols.Yahoo;
using Microsoft.SqlServerCe.VersionManagement;

namespace NexusIM
{
	public sealed class SQLCESettings : ISettings
	{
		public SQLCESettings(string connectionString)
		{
			ConnectionString = connectionString;
			Prepare();
		}

		public void Prepare()
		{
			SqlCeVersion version = SqlCeVersionManager.GetVersion(ConnectionString);
			Trace.Write("SqlCeSettings: Database file version: " + version);
			if (version < new SqlCeVersion(4, 0))
			{
				Trace.WriteLine(" ... Upgrading");
				UpgradeDatafile();
				VerifyIntegrity();
			} else
				Trace.WriteLine(String.Empty);
		}

		private void UpgradeDatafile()
		{
			SqlCeEngine engine = new SqlCeEngine(ConnectionString);
			engine.Upgrade();

			engine.Dispose();
		}
		private void VerifyIntegrity()
		{
			SqlCeEngine engine = new SqlCeEngine(ConnectionString);
			if (!engine.Verify())
				throw new ApplicationException("UserProfile database failed integrity check");
		}

		// Nested Classes
		private class SqlDictionary : IDictionary<string, string>
		{
			#region IDictionary<string,string> Members

			public void Add(string key, string value)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);
				Setting setting = new Setting();
				setting.Key = key;
				setting.Value = value;

				db.Settings.InsertOnSubmit(setting);
				db.SubmitChanges();
				db.Dispose();
			}
			public bool ContainsKey(string key)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

				return db.Settings.Any(s => s.Key == key);
			}
			public ICollection<string> Keys
			{
				get {
					UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

					return db.Settings.Select(s => s.Key).ToList();
				}
			}
			public bool Remove(string key)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);
				Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

				if (setting == null)
					return false;

				db.Settings.DeleteOnSubmit(setting);
				db.SubmitChanges();

				return true;
			}
			public bool TryGetValue(string key, out string value)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);
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
					UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

					return db.Settings.Select(s => s.Value).ToList();
				}
			}
			public string this[string key]
			{
				get	{
					UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

					Setting setting = db.Settings.FirstOrDefault(s => s.Key == key);

					if (setting == null)
						return null;

					return setting.Value;
				}
				set	{
					UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

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
					UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

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
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);

				return db.Settings.Select(s => new KeyValuePair<string, string>(s.Key, s.Value)).GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion
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
		private class SqlAccountList : IList<IMProtocolWrapper>
		{
			public SqlAccountList(string connectionString)
			{
				mConnectionString = connectionString;
			}

			private class SqlAccountEnumerator : IEnumerator<IMProtocolWrapper>
			{
				public SqlAccountEnumerator(Table<Account> source)
				{
					mEnumerator = source.GetEnumerator();
					mContext = source.Context;
				}

				#region IEnumerator<IMProtocolExtraData> Members

				public IMProtocolWrapper Current
				{
					get {
						Account current = mEnumerator.Current;
						if (current == null)
							return null;

						IMProtocol protocol;
						switch (current.AccountType.ToLowerInvariant())
						{
							case "yahoo":
								protocol = new IMYahooProtocol();
								break;
							case "irc":
								IRCProtocol irc = new IRCProtocol();
								irc.Nickname = current.AccountSettings.Where(ac => ac.Key == "nickname").Select(ac => ac.Value).FirstOrDefault();
								irc.RealName = current.AccountSettings.Where(ac => ac.Key == "realname").Select(ac => ac.Value).FirstOrDefault();

								protocol = irc;
								break;
							default:
								protocol = IMProtocol.FromString(current.AccountType);
								break;
						}

						if (protocol == null)
						{
							string message = "Failed to find protocol class to handle protocol of type '" + current.AccountType + "'";
							Trace.WriteLine(message);
							throw new NotImplementedException(message);
						}

						IMProtocolWrapper extraData = new IMProtocolWrapper();
						extraData.Protocol = protocol;
						extraData.DatabaseId = current.Id;
						extraData.Enabled = extraData.AutoConnect = current.AutoConnect;
						protocol.Username = current.Username;
						protocol.Password = current.Password;
						protocol.Server = current.Server;
						protocol.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) => {
							current.Username = protocol.Username;
							current.Password = protocol.Password;
							current.Server = protocol.Server;

							if (e.PropertyName == "Nickname")
								protocol.ConfigurationSettings["nickname"] = ((IRCProtocol)protocol).Nickname;
							else if (e.PropertyName == "RealName")
								protocol.ConfigurationSettings["realname"] = ((IRCProtocol)protocol).RealName;

							mContext.SubmitChanges();
						});
						extraData.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) => {
							current.AutoConnect = extraData.AutoConnect;
							mContext.SubmitChanges();
						});
						protocol.ConfigurationSettings = new SqlAccountSettingDictionary(mContext, current.AccountSettings);

						return extraData;
					}
				}

				#endregion

				#region IDisposable Members

				public void Dispose()
				{
					mEnumerator.Dispose();
					mEnumerator = null;
					GC.SuppressFinalize(this);
				}

				#endregion

				#region IEnumerator Members

				object IEnumerator.Current
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

			#region IEnumerable<IMProtocolExtraData> Members

			public IEnumerator<IMProtocolWrapper> GetEnumerator()
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				return new SqlAccountEnumerator(db.Accounts);
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion

			#region IList<IMProtocolExtraData> Members

			public int IndexOf(IMProtocolWrapper item)
			{
				throw new NotImplementedException();
			}
			public void Insert(int index, IMProtocolWrapper item)
			{
				throw new NotImplementedException();
			}
			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}
			public IMProtocolWrapper this[int index]
			{
				get	{
					throw new NotImplementedException();
				}
				set	{
					throw new NotImplementedException();
				}
			}

			#endregion

			#region ICollection<IMProtocolExtraData> Members

			public void Add(IMProtocolWrapper item)
			{
				UserProfile db = UserProfile.Create(mConnectionString);

				Account account = new Account();
				account.AccountType = item.Protocol.ShortProtocol;
				account.Username = item.Protocol.Username;
				account.Password = item.Protocol.Password;
				account.AutoConnect = item.AutoConnect;

				foreach (var setting in item.Protocol.ConfigurationSettings)
				{
					AccountSetting toInsert = new AccountSetting();
					toInsert.Key = setting.Key;
					toInsert.Value = setting.Value;
					toInsert.Account = account;

					account.AccountSettings.Add(toInsert);
				}

				db.Accounts.InsertOnSubmit(account);

				item.Protocol.ConfigurationSettings = new SqlAccountSettingDictionary(db, account.AccountSettings);

				db.SubmitChanges();
			}
			public void Clear()
			{
				throw new NotImplementedException();
			}
			public bool Contains(IMProtocolWrapper item)
			{
				throw new NotImplementedException();
			}
			public void CopyTo(IMProtocolWrapper[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}
			public int Count
			{
				get {
					throw new NotImplementedException();
				}
			}
			public bool IsReadOnly
			{
				get {
					return false;
				}
			}
			public bool Remove(IMProtocolWrapper item)
			{
				if (item == null)
					return false;

				UserProfile db = UserProfile.Create(mConnectionString);
				Account acc = db.Accounts.Where(a => a.Username == item.Protocol.Username && a.AccountType == item.Protocol.ShortProtocol).FirstOrDefault();
				
				if (acc == null)
					return false;

				db.Accounts.DeleteOnSubmit(acc);
				db.SubmitChanges();

				return true;
			}

			#endregion

			private string mConnectionString;
		}
		private class SqlChatAreaPool : IChatAreaPool
		{
			#region IChatAreaPool Members

			public void PutInPool(int poolid, IMProtocol protocol, string objectId)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);
				ChatWindowPool pool = db.ChatWindowPools.FirstOrDefault(cwp => cwp.Account.Username == protocol.Username && cwp.Account.AccountType == protocol.ShortProtocol && cwp.Username == objectId);
				if (pool == null)
				{
					pool = new ChatWindowPool();
					pool.Username = objectId;
					db.ChatWindowPools.InsertOnSubmit(pool);
				}

				pool.PoolId = (short)poolid;
				db.SubmitChanges();
			}

			public void RemoveFromPool(IMProtocol protocol, string objectId)
			{
				
			}

			public int? GetPool(IMProtocol protocol, string objectId)
			{
				return null;
			}

			public int GetNextId()
			{
				return new Random().Next();
			}

			#endregion
		}

		// Properties
		public IList<IMProtocolWrapper> Accounts
		{
			get	{
				if (mAccountList == null)
					mAccountList = new SqlAccountList(ConnectionString);
				return mAccountList;
			}
		}
		public IDictionary<string, string> Settings
		{
			get	{
				if (mGenericSettings == null)
					mGenericSettings = new SqlDictionary();
				return mGenericSettings;
			}
		}
		public IDictionary<IMProtocol, IDictionary<string, string>> ProtocolSettings
		{
			get	{
				if (mProtocolSettings == null)
					mProtocolSettings = new SqlProtocolDictionary(ConnectionString);
				return mProtocolSettings;
			}
		}
		public IChatAreaPool ChatAreaPool
		{
			get	{
				if (mChatPool == null)
					mChatPool = new SqlChatAreaPool();

				return mChatPool;
			}
		}
		
		// Variables
		private static string ConnectionString
		{
			get;
			set;
		}
		private SqlAccountList mAccountList;
		private SqlDictionary mGenericSettings;
		private SqlProtocolDictionary mProtocolSettings;
		private SqlChatAreaPool mChatPool;
	}
}