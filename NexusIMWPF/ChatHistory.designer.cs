﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
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
	
	
	public partial class ChatHistory : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPrivateMessage(PrivateMessage instance);
    partial void UpdatePrivateMessage(PrivateMessage instance);
    partial void DeletePrivateMessage(PrivateMessage instance);
    #endregion
		
		public ChatHistory(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ChatHistory(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ChatHistory(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ChatHistory(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<PrivateMessage> PrivateMessages
		{
			get
			{
				return this.GetTable<PrivateMessage>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="PrivateMessages")]
	public partial class PrivateMessage : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.DateTime _Timestamp;
		
		private string _MessageBody;
		
		private string _Sender;
		
		private string _Recipient;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTimestampChanging(System.DateTime value);
    partial void OnTimestampChanged();
    partial void OnMessageBodyChanging(string value);
    partial void OnMessageBodyChanged();
    partial void OnSenderChanging(string value);
    partial void OnSenderChanged();
    partial void OnRecipientChanging(string value);
    partial void OnRecipientChanged();
    #endregion
		
		public PrivateMessage()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Timestamp", DbType="DateTime NOT NULL", IsPrimaryKey=true)]
		public System.DateTime Timestamp
		{
			get
			{
				return this._Timestamp;
			}
			set
			{
				if ((this._Timestamp != value))
				{
					this.OnTimestampChanging(value);
					this.SendPropertyChanging();
					this._Timestamp = value;
					this.SendPropertyChanged("Timestamp");
					this.OnTimestampChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MessageBody", DbType="NVarChar(4000) NOT NULL", CanBeNull=false)]
		public string MessageBody
		{
			get
			{
				return this._MessageBody;
			}
			set
			{
				if ((this._MessageBody != value))
				{
					this.OnMessageBodyChanging(value);
					this.SendPropertyChanging();
					this._MessageBody = value;
					this.SendPropertyChanged("MessageBody");
					this.OnMessageBodyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Sender", DbType="NVarChar(64) NOT NULL", CanBeNull=false)]
		public string Sender
		{
			get
			{
				return this._Sender;
			}
			set
			{
				if ((this._Sender != value))
				{
					this.OnSenderChanging(value);
					this.SendPropertyChanging();
					this._Sender = value;
					this.SendPropertyChanged("Sender");
					this.OnSenderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Recipient", DbType="NVarChar(64) NOT NULL", CanBeNull=false)]
		public string Recipient
		{
			get
			{
				return this._Recipient;
			}
			set
			{
				if ((this._Recipient != value))
				{
					this.OnRecipientChanging(value);
					this.SendPropertyChanging();
					this._Recipient = value;
					this.SendPropertyChanged("Recipient");
					this.OnRecipientChanged();
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
