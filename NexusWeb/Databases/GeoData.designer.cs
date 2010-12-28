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

namespace NexusWeb.Databases
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="GeoData")]
	public partial class GeoDataDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertCity(City instance);
    partial void UpdateCity(City instance);
    partial void DeleteCity(City instance);
    partial void InsertAdminLevel1(AdminLevel1 instance);
    partial void UpdateAdminLevel1(AdminLevel1 instance);
    partial void DeleteAdminLevel1(AdminLevel1 instance);
    #endregion
		
		public GeoDataDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["GeoDataConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GeoDataDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GeoDataDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GeoDataDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GeoDataDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<City> Cities
		{
			get
			{
				return this.GetTable<City>();
			}
		}
		
		public System.Data.Linq.Table<Country> Countries
		{
			get
			{
				return this.GetTable<Country>();
			}
		}
		
		public System.Data.Linq.Table<AdminLevel1> AdminLevel1s
		{
			get
			{
				return this.GetTable<AdminLevel1>();
			}
		}
		
		public System.Data.Linq.Table<AdminLevel2> AdminLevel2s
		{
			get
			{
				return this.GetTable<AdminLevel2>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.GetCountry")]
		public ISingleResult<Country> GetCountry([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lat, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lng)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), lat, lng);
			return ((ISingleResult<Country>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.GetNearestCity")]
		public ISingleResult<City> GetNearestCity([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lat, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lng)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), lat, lng);
			return ((ISingleResult<City>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.GetAdminLevel1")]
		public ISingleResult<AdminLevel1> GetAdminLevel1([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lat, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lng)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), lat, lng);
			return ((ISingleResult<AdminLevel1>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.GetAdminLevel2")]
		public ISingleResult<AdminLevel2> GetAdminLevel2([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lat, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Float")] System.Nullable<double> lng)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), lat, lng);
			return ((ISingleResult<AdminLevel2>)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Cities")]
	public partial class City : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _geonameid = default(int);
		
		private System.DateTime _modification_date = default(System.DateTime);
		
		private string _name = default(string);
		
		private string _asciiname = default(string);
		
		private string _alternatenames = default(string);
		
		private double _latitude = default(double);
		
		private double _longitude = default(double);
		
		private string _feature_class = default(string);
		
		private string _feature_code = default(string);
		
		private string _country_code = default(string);
		
		private string _cc2 = default(string);
		
		private string _admin1_code = default(string);
		
		private string _admin2_code = default(string);
		
		private string _admin3_code = default(string);
		
		private string _admin4_code = default(string);
		
		private System.Nullable<int> _population = default(System.Nullable<int>);
		
		private System.Nullable<int> _elevation = default(System.Nullable<int>);
		
		private System.Nullable<int> _gtopo30 = default(System.Nullable<int>);
		
		private string _timezone = default(string);
		
		private Microsoft.SqlServer.Types.SqlGeography _geog = default(Microsoft.SqlServer.Types.SqlGeography);
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    #endregion
		
		public City()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_geonameid", DbType="Int NOT NULL", IsPrimaryKey=true, UpdateCheck=UpdateCheck.Never)]
		public int geonameid
		{
			get
			{
				return this._geonameid;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_modification_date", DbType="Date NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public System.DateTime modification_date
		{
			get
			{
				return this._modification_date;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="NVarChar(200) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string name
		{
			get
			{
				return this._name;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_asciiname", DbType="NVarChar(200)", UpdateCheck=UpdateCheck.Never)]
		public string asciiname
		{
			get
			{
				return this._asciiname;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_alternatenames", DbType="NVarChar(MAX)", UpdateCheck=UpdateCheck.Never)]
		public string alternatenames
		{
			get
			{
				return this._alternatenames;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_latitude", DbType="Float NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public double latitude
		{
			get
			{
				return this._latitude;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_longitude", DbType="Float NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public double longitude
		{
			get
			{
				return this._longitude;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_feature_class", DbType="Char(2)", UpdateCheck=UpdateCheck.Never)]
		public string feature_class
		{
			get
			{
				return this._feature_class;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_feature_code", DbType="NVarChar(10)", UpdateCheck=UpdateCheck.Never)]
		public string feature_code
		{
			get
			{
				return this._feature_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_country_code", DbType="Char(3) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string country_code
		{
			get
			{
				return this._country_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_cc2", DbType="Char(60)", UpdateCheck=UpdateCheck.Never)]
		public string cc2
		{
			get
			{
				return this._cc2;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_admin1_code", DbType="NVarChar(20)", UpdateCheck=UpdateCheck.Never)]
		public string admin1_code
		{
			get
			{
				return this._admin1_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_admin2_code", DbType="NVarChar(80)", UpdateCheck=UpdateCheck.Never)]
		public string admin2_code
		{
			get
			{
				return this._admin2_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_admin3_code", DbType="NVarChar(20)", UpdateCheck=UpdateCheck.Never)]
		public string admin3_code
		{
			get
			{
				return this._admin3_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_admin4_code", DbType="NVarChar(20)", UpdateCheck=UpdateCheck.Never)]
		public string admin4_code
		{
			get
			{
				return this._admin4_code;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_population", DbType="Int", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<int> population
		{
			get
			{
				return this._population;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_elevation", DbType="Int", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<int> elevation
		{
			get
			{
				return this._elevation;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_gtopo30", DbType="Int", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<int> gtopo30
		{
			get
			{
				return this._gtopo30;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_timezone", DbType="Char(31)", UpdateCheck=UpdateCheck.Never)]
		public string timezone
		{
			get
			{
				return this._timezone;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_geog", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public Microsoft.SqlServer.Types.SqlGeography geog
		{
			get
			{
				return this._geog;
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
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Countries")]
	public partial class Country
	{
		
		private string _ISO = default(string);
		
		private string _ISO3 = default(string);
		
		private int _ISO_Numeric = default(int);
		
		private string _fips = default(string);
		
		private string _Name = default(string);
		
		private string _Capital = default(string);
		
		private System.Nullable<float> _Area = default(System.Nullable<float>);
		
		private System.Nullable<long> _Population = default(System.Nullable<long>);
		
		private string _Continent = default(string);
		
		private string _tld = default(string);
		
		private string _CurrencyCode = default(string);
		
		private string _CurrencyName = default(string);
		
		private string _Phone = default(string);
		
		private string _Postal_Code_Format = default(string);
		
		private string _Postal_Code_Regex = default(string);
		
		private string _Languages = default(string);
		
		private long _geonameid = default(long);
		
		private string _neighbours = default(string);
		
		private string _EquivalentFipsCode = default(string);
		
		private Microsoft.SqlServer.Types.SqlGeometry _GeoData = default(Microsoft.SqlServer.Types.SqlGeometry);
		
		public Country()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISO", DbType="VarChar(2) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string ISO
		{
			get
			{
				return this._ISO;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISO3", DbType="VarChar(3) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string ISO3
		{
			get
			{
				return this._ISO3;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[ISO-Numeric]", Storage="_ISO_Numeric", DbType="Int NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public int ISO_Numeric
		{
			get
			{
				return this._ISO_Numeric;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fips", DbType="VarChar(2)", UpdateCheck=UpdateCheck.Never)]
		public string fips
		{
			get
			{
				return this._fips;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(70) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string Name
		{
			get
			{
				return this._Name;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Capital", DbType="NVarChar(70)", UpdateCheck=UpdateCheck.Never)]
		public string Capital
		{
			get
			{
				return this._Capital;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Area(in sq km)]", Storage="_Area", DbType="Real", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<float> Area
		{
			get
			{
				return this._Area;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Population", DbType="BigInt", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<long> Population
		{
			get
			{
				return this._Population;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Continent", DbType="VarChar(2) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string Continent
		{
			get
			{
				return this._Continent;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tld", DbType="VarChar(3)", UpdateCheck=UpdateCheck.Never)]
		public string tld
		{
			get
			{
				return this._tld;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CurrencyCode", DbType="VarChar(3)", UpdateCheck=UpdateCheck.Never)]
		public string CurrencyCode
		{
			get
			{
				return this._CurrencyCode;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CurrencyName", DbType="VarChar(13)", UpdateCheck=UpdateCheck.Never)]
		public string CurrencyName
		{
			get
			{
				return this._CurrencyName;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Phone", DbType="VarChar(16)", UpdateCheck=UpdateCheck.Never)]
		public string Phone
		{
			get
			{
				return this._Phone;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Postal Code Format]", Storage="_Postal_Code_Format", DbType="VarChar(200)", UpdateCheck=UpdateCheck.Never)]
		public string Postal_Code_Format
		{
			get
			{
				return this._Postal_Code_Format;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Postal Code Regex]", Storage="_Postal_Code_Regex", DbType="VarChar(200)", UpdateCheck=UpdateCheck.Never)]
		public string Postal_Code_Regex
		{
			get
			{
				return this._Postal_Code_Regex;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Languages", DbType="VarChar(100)", UpdateCheck=UpdateCheck.Never)]
		public string Languages
		{
			get
			{
				return this._Languages;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_geonameid", DbType="BigInt NOT NULL", UpdateCheck=UpdateCheck.Never)]
		public long geonameid
		{
			get
			{
				return this._geonameid;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_neighbours", DbType="VarChar(41)", UpdateCheck=UpdateCheck.Never)]
		public string neighbours
		{
			get
			{
				return this._neighbours;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EquivalentFipsCode", DbType="VarChar(2)", UpdateCheck=UpdateCheck.Never)]
		public string EquivalentFipsCode
		{
			get
			{
				return this._EquivalentFipsCode;
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GeoData", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public Microsoft.SqlServer.Types.SqlGeometry GeoData
		{
			get
			{
				return this._GeoData;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AdminLevel1")]
	public partial class AdminLevel1 : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Remarks;
		
		private string _ISO;
		
		private string _Name;
		
		private string _AltName;
		
		private string _HASC;
		
		private string _Type;
		
		private string _Type2;
		
		private string _ValidFrom;
		
		private string _ValidTo;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnRemarksChanging(string value);
    partial void OnRemarksChanged();
    partial void OnISOChanging(string value);
    partial void OnISOChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnAltNameChanging(string value);
    partial void OnAltNameChanged();
    partial void OnHASCChanging(string value);
    partial void OnHASCChanged();
    partial void OnTypeChanging(string value);
    partial void OnTypeChanged();
    partial void OnType2Changing(string value);
    partial void OnType2Changed();
    partial void OnValidFromChanging(string value);
    partial void OnValidFromChanged();
    partial void OnValidToChanging(string value);
    partial void OnValidToChanged();
    #endregion
		
		public AdminLevel1()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="RowId", Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Remarks", DbType="VarChar(255)")]
		public string Remarks
		{
			get
			{
				return this._Remarks;
			}
			set
			{
				if ((this._Remarks != value))
				{
					this.OnRemarksChanging(value);
					this.SendPropertyChanging();
					this._Remarks = value;
					this.SendPropertyChanged("Remarks");
					this.OnRemarksChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ISO", DbType="Char(3) NOT NULL", CanBeNull=false)]
		public string ISO
		{
			get
			{
				return this._ISO;
			}
			set
			{
				if ((this._ISO != value))
				{
					this.OnISOChanging(value);
					this.SendPropertyChanging();
					this._ISO = value;
					this.SendPropertyChanged("ISO");
					this.OnISOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AltName", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string AltName
		{
			get
			{
				return this._AltName;
			}
			set
			{
				if ((this._AltName != value))
				{
					this.OnAltNameChanging(value);
					this.SendPropertyChanging();
					this._AltName = value;
					this.SendPropertyChanged("AltName");
					this.OnAltNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HASC", DbType="Char(11) NOT NULL", CanBeNull=false)]
		public string HASC
		{
			get
			{
				return this._HASC;
			}
			set
			{
				if ((this._HASC != value))
				{
					this.OnHASCChanging(value);
					this.SendPropertyChanging();
					this._HASC = value;
					this.SendPropertyChanged("HASC");
					this.OnHASCChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Type", DbType="VarChar(64)")]
		public string Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				if ((this._Type != value))
				{
					this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Type2", DbType="VarChar(64)")]
		public string Type2
		{
			get
			{
				return this._Type2;
			}
			set
			{
				if ((this._Type2 != value))
				{
					this.OnType2Changing(value);
					this.SendPropertyChanging();
					this._Type2 = value;
					this.SendPropertyChanged("Type2");
					this.OnType2Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidFrom", DbType="VarChar(32)")]
		public string ValidFrom
		{
			get
			{
				return this._ValidFrom;
			}
			set
			{
				if ((this._ValidFrom != value))
				{
					this.OnValidFromChanging(value);
					this.SendPropertyChanging();
					this._ValidFrom = value;
					this.SendPropertyChanged("ValidFrom");
					this.OnValidFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidTo", DbType="VarChar(32)")]
		public string ValidTo
		{
			get
			{
				return this._ValidTo;
			}
			set
			{
				if ((this._ValidTo != value))
				{
					this.OnValidToChanging(value);
					this.SendPropertyChanging();
					this._ValidTo = value;
					this.SendPropertyChanged("ValidTo");
					this.OnValidToChanged();
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
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AdminLevel2")]
	public partial class AdminLevel2
	{
		
		private int _Id;
		
		private string _Remarks;
		
		private string _CountryISO;
		
		private string _TwoDigitId;
		
		private string _Name;
		
		private string _VarName;
		
		private int _ParentId;
		
		private string _HASC;
		
		private string _Type;
		
		private string _EnglishType;
		
		private string _ValidFrom;
		
		private string _ValidTo;
		
		public AdminLevel2()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL")]
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
					this._Id = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Remarks", DbType="VarChar(255)")]
		public string Remarks
		{
			get
			{
				return this._Remarks;
			}
			set
			{
				if ((this._Remarks != value))
				{
					this._Remarks = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CountryISO", DbType="Char(3) NOT NULL", CanBeNull=false)]
		public string CountryISO
		{
			get
			{
				return this._CountryISO;
			}
			set
			{
				if ((this._CountryISO != value))
				{
					this._CountryISO = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TwoDigitId", DbType="Char(2) NOT NULL", CanBeNull=false)]
		public string TwoDigitId
		{
			get
			{
				return this._TwoDigitId;
			}
			set
			{
				if ((this._TwoDigitId != value))
				{
					this._TwoDigitId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this._Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_VarName", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string VarName
		{
			get
			{
				return this._VarName;
			}
			set
			{
				if ((this._VarName != value))
				{
					this._VarName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentId", DbType="Int NOT NULL")]
		public int ParentId
		{
			get
			{
				return this._ParentId;
			}
			set
			{
				if ((this._ParentId != value))
				{
					this._ParentId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HASC", DbType="Char(11) NOT NULL", CanBeNull=false)]
		public string HASC
		{
			get
			{
				return this._HASC;
			}
			set
			{
				if ((this._HASC != value))
				{
					this._HASC = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Type", DbType="VarChar(64) NOT NULL", CanBeNull=false)]
		public string Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				if ((this._Type != value))
				{
					this._Type = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EnglishType", DbType="VarChar(64) NOT NULL", CanBeNull=false)]
		public string EnglishType
		{
			get
			{
				return this._EnglishType;
			}
			set
			{
				if ((this._EnglishType != value))
				{
					this._EnglishType = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidFrom", DbType="VarChar(32)")]
		public string ValidFrom
		{
			get
			{
				return this._ValidFrom;
			}
			set
			{
				if ((this._ValidFrom != value))
				{
					this._ValidFrom = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidTo", DbType="VarChar(32)")]
		public string ValidTo
		{
			get
			{
				return this._ValidTo;
			}
			set
			{
				if ((this._ValidTo != value))
				{
					this._ValidTo = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
