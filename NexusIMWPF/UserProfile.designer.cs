﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NexusIM
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	public partial class UserProfile : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertAccount(Account instance);
    partial void UpdateAccount(Account instance);
    partial void DeleteAccount(Account instance);
    partial void InsertAccountSetting(AccountSetting instance);
    partial void UpdateAccountSetting(AccountSetting instance);
    partial void DeleteAccountSetting(AccountSetting instance);
    partial void InsertSetting(Setting instance);
    partial void UpdateSetting(Setting instance);
    partial void DeleteSetting(Setting instance);
    #endregion
		
		public UserProfile(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserProfile(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserProfile(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserProfile(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Account> Accounts
		{
			get
			{
				return this.GetTable<Account>();
			}
		}
		
		public System.Data.Linq.Table<AccountSetting> AccountSettings
		{
			get
			{
				return this.GetTable<AccountSetting>();
			}
		}
		
		public System.Data.Linq.Table<Setting> Settings
		{
			get
			{
				return this.GetTable<Setting>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Accounts")]
	public partial class Account : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _AccountType;
		
		private string _Username;
		
		private string _Password;
		
		private bool _Enabled;
		
		private bool _AutoConnect;
		
		private EntitySet<AccountSetting> _AccountSettings;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnAccountTypeChanging(string value);
    partial void OnAccountTypeChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnPasswordChanging(string value);
    partial void OnPasswordChanged();
    partial void OnEnabledChanging(bool value);
    partial void OnEnabledChanged();
    partial void OnAutoConnectChanging(bool value);
    partial void OnAutoConnectChanged();
    #endregion
		
		public Account()
		{
			this._AccountSettings = new EntitySet<AccountSetting>(new Action<AccountSetting>(this.attach_AccountSettings), new Action<AccountSetting>(this.detach_AccountSettings));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AccountType", DbType="NVarChar(8) NOT NULL", CanBeNull=false)]
		public string AccountType
		{
			get
			{
				return this._AccountType;
			}
			set
			{
				if ((this._AccountType != value))
				{
					this.OnAccountTypeChanging(value);
					this.SendPropertyChanging();
					this._AccountType = value;
					this.SendPropertyChanged("AccountType");
					this.OnAccountTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(32) NOT NULL", CanBeNull=false)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Password", DbType="NVarChar(32)")]
		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				if ((this._Password != value))
				{
					this.OnPasswordChanging(value);
					this.SendPropertyChanging();
					this._Password = value;
					this.SendPropertyChanged("Password");
					this.OnPasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Enabled", DbType="bit NOT NULL")]
		public bool Enabled
		{
			get
			{
				return this._Enabled;
			}
			set
			{
				if ((this._Enabled != value))
				{
					this.OnEnabledChanging(value);
					this.SendPropertyChanging();
					this._Enabled = value;
					this.SendPropertyChanged("Enabled");
					this.OnEnabledChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AutoConnect", DbType="bit NOT NULL")]
		public bool AutoConnect
		{
			get
			{
				return this._AutoConnect;
			}
			set
			{
				if ((this._AutoConnect != value))
				{
					this.OnAutoConnectChanging(value);
					this.SendPropertyChanging();
					this._AutoConnect = value;
					this.SendPropertyChanged("AutoConnect");
					this.OnAutoConnectChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Account_AccountSetting", Storage="_AccountSettings", ThisKey="Id", OtherKey="AccountId")]
		public EntitySet<AccountSetting> AccountSettings
		{
			get
			{
				return this._AccountSettings;
			}
			set
			{
				this._AccountSettings.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_AccountSettings(AccountSetting entity)
		{
			this.SendPropertyChanging();
			entity.Account = this;
		}
		
		private void detach_AccountSettings(AccountSetting entity)
		{
			this.SendPropertyChanging();
			entity.Account = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="AccountSettings")]
	public partial class AccountSetting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _AccountId;
		
		private string _Key;
		
		private string _Value;
		
		private EntityRef<Account> _Account;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnAccountIdChanging(int value);
    partial void OnAccountIdChanged();
    partial void OnKeyChanging(string value);
    partial void OnKeyChanged();
    partial void OnValueChanging(string value);
    partial void OnValueChanged();
    #endregion
		
		public AccountSetting()
		{
			this._Account = default(EntityRef<Account>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AccountId", DbType="Int NOT NULL")]
		public int AccountId
		{
			get
			{
				return this._AccountId;
			}
			set
			{
				if ((this._AccountId != value))
				{
					if (this._Account.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnAccountIdChanging(value);
					this.SendPropertyChanging();
					this._AccountId = value;
					this.SendPropertyChanged("AccountId");
					this.OnAccountIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Key", DbType="NVarChar(20) NOT NULL", CanBeNull=false)]
		public string Key
		{
			get
			{
				return this._Key;
			}
			set
			{
				if ((this._Key != value))
				{
					this.OnKeyChanging(value);
					this.SendPropertyChanging();
					this._Key = value;
					this.SendPropertyChanged("Key");
					this.OnKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Value", DbType="NVarChar(500) NOT NULL", CanBeNull=false)]
		public string Value
		{
			get
			{
				return this._Value;
			}
			set
			{
				if ((this._Value != value))
				{
					this.OnValueChanging(value);
					this.SendPropertyChanging();
					this._Value = value;
					this.SendPropertyChanged("Value");
					this.OnValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Account_AccountSetting", Storage="_Account", ThisKey="AccountId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true)]
		public Account Account
		{
			get
			{
				return this._Account.Entity;
			}
			set
			{
				Account previousValue = this._Account.Entity;
				if (((previousValue != value) 
							|| (this._Account.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Account.Entity = null;
						previousValue.AccountSettings.Remove(this);
					}
					this._Account.Entity = value;
					if ((value != null))
					{
						value.AccountSettings.Add(this);
						this._AccountId = value.Id;
					}
					else
					{
						this._AccountId = default(int);
					}
					this.SendPropertyChanged("Account");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="Settings")]
	public partial class Setting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Key;
		
		private string _Value;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnKeyChanging(string value);
    partial void OnKeyChanged();
    partial void OnValueChanging(string value);
    partial void OnValueChanged();
    #endregion
		
		public Setting()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Key", DbType="NVarChar(12) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Key
		{
			get
			{
				return this._Key;
			}
			set
			{
				if ((this._Key != value))
				{
					this.OnKeyChanging(value);
					this.SendPropertyChanging();
					this._Key = value;
					this.SendPropertyChanged("Key");
					this.OnKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Value", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Value
		{
			get
			{
				return this._Value;
			}
			set
			{
				if ((this._Value != value))
				{
					this.OnValueChanging(value);
					this.SendPropertyChanging();
					this._Value = value;
					this.SendPropertyChanged("Value");
					this.OnValueChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
