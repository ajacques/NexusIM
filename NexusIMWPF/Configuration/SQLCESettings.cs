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
using NexusIM.Managers;

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
				AccountSetting setting = mSource.FirstOrDefault(a => a.Key == key);

				if (setting == null)
					return false;

				mSource.Remove(setting);

				return true;
			}
			public bool TryGetValue(string key, out string value)
			{
				AccountSetting setting = mSource.FirstOrDefault(s => s.Key == key);

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
					AccountSetting setting = mSource.FirstOrDefault(s => s.Key == key);
					if (setting == null)
						return null;

					return setting.Value;
				}
				set	{
					AccountSetting setting = mSource.FirstOrDefault(s => s.Key == key);

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
		private class SqlAccountList : ICollection<IMProtocolWrapper>
		{
			public SqlAccountList(string connectionString)
			{
				mConnectionString = connectionString;
				mWrapperHandler = new PropertyChangedEventHandler(IMProtocolWrapper_PropertyChanged);
				mProtocolHandler = new PropertyChangedEventHandler(IMProtocol_PropertyChanged);

				StopwatchManager.Start("acc-compile");
				FindAccById = CompiledQuery.Compile<UserProfile, int, Account>((db, accid) => db.Accounts.FirstOrDefault(acc => acc.Id == accid));
				StopwatchManager.Stop("acc-compile", "SqlAccountList Query Precompilation ({0}) completed in {1}");
			}

			private void IMProtocolWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				IMProtocolWrapper wrapper = (IMProtocolWrapper)sender;

				switch (e.PropertyName)
				{
					case "AutoConnect":
						break;

					default:
						return;
				}
				UserProfile profile = UserProfile.Create(mConnectionString);
				Account account = FindAccById(profile, wrapper.DatabaseId);

				if (account == null)
					return;

				account.AutoConnect = wrapper.AutoConnect;

				profile.SubmitChanges();
			}
			private void IMProtocol_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				IMProtocol protocol = (IMProtocol)sender;
				UserProfile profile = UserProfile.Create(mConnectionString);
				IMProtocolWrapper wrapper = AccountManager.Accounts.Find(protocol);
				Account account = FindAccById(profile, wrapper.DatabaseId);

				if (account == null)
					return;

				switch (e.PropertyName)
				{
					case "Username":
						account.Username = protocol.Username;
						break;
					case "Password":
						account.Password = protocol.Password;
						break;
					case "Server":
						account.Server = protocol.Server;
						break;
					case "Nickname":
						protocol.ConfigurationSettings["nickname"] = ((IRCProtocol)protocol).Nickname;
						break;
					case "RealName":
						protocol.ConfigurationSettings["realname"] = ((IRCProtocol)protocol).RealName;
						break;
					default:
						return;
				}

				profile.SubmitChanges();
			}

			private class SqlAccountEnumerator : IEnumerator<IMProtocolWrapper>
			{
				public SqlAccountEnumerator(Table<Account> source, PropertyChangedEventHandler wrapperHandler, PropertyChangedEventHandler protocolHandler)
				{
					mEnumerator = source.GetEnumerator();
					mContext = source.Context;
					mWrapperHandler = wrapperHandler;
					mProtocolHandler = protocolHandler;
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
						protocol.PropertyChanged += mProtocolHandler;
						extraData.PropertyChanged += mWrapperHandler;
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
				private PropertyChangedEventHandler mWrapperHandler;
				private PropertyChangedEventHandler mProtocolHandler;
			}

			#region IEnumerable<IMProtocolExtraData> Members

			public IEnumerator<IMProtocolWrapper> GetEnumerator()
			{
				UserProfile db = UserProfile.Create(mConnectionString);
				return new SqlAccountEnumerator(db.Accounts, mWrapperHandler, mProtocolHandler);
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
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
			private PropertyChangedEventHandler mWrapperHandler;
			private PropertyChangedEventHandler mProtocolHandler;
			
			// Compiled Queries
			private Func<UserProfile, int, Account> FindAccById;
		}
		private class SqlChatAreaPool : IChatAreaPool
		{
			public SqlChatAreaPool()
			{
				StopwatchManager.Start("pool-compile");
				GetWindowPool = CompiledQuery.Compile<UserProfile, IMProtocol, string, ChatWindowPool>((db, protocol, objectId) => db.ChatWindowPools.Where(cwp => cwp.Account.Username == protocol.Username && cwp.Account.AccountType == protocol.ShortProtocol && cwp.Username == objectId).FirstOrDefault());
				StopwatchManager.Stop("pool-compile", "SqlChatAreaPool Query Precompilation ({0}) completed in {1}");
			}

			#region IChatAreaPool Members

			public void PutInPool(int poolid, IMProtocol protocol, string objectId)
			{
				UserProfile db = UserProfile.Create(SQLCESettings.ConnectionString);
				ChatWindowPool pool = GetWindowPool(db, protocol, objectId);
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

			// Compiled Queries
			Func<UserProfile, IMProtocol, string, ChatWindowPool> GetWindowPool;
		}

		// Properties
		public ICollection<IMProtocolWrapper> Accounts
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