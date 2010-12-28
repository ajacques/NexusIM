using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NexusWeb.Services.DataContracts
{
	[DataContract(Namespace = "")]
	public class GeoCity
	{
		public string City
		{
			get	{
				return mCity;
			}
		}
		public GeoLevel1 AdminLevel1
		{
			get	{
				return mAdminLevel1;
			}
		}

		[DataMember(Name = "City")]
		public string mCity;
		[DataMember(Name = "AdminLevel1")]
		public GeoLevel1 mAdminLevel1;
		[DataMember(Name = "AdminLevel2")]
		public GeoLevel2 mAdminLevel2;
		[DataMember(Name = "Country")]
		public GeoCountry mCountry;
	}

	[DataContract]
	public class GeoLevel1
	{
		[DataMember]
		public string Name;
		[DataMember]
		public GeoLevel1Type Type;
	}

	[DataContract]
	public class GeoLevel2
	{
		[DataMember]
		public string Name;
		[DataMember]
		public GeoLevel2Type Type;
	}

	[DataContract]
	public class GeoCountry
	{
		[DataMember]
		public string ISO3;
		[DataMember]
		public string FullName;
	}
	
	public enum GeoLevel2Type
	{
		Unknown,
		Other,
		County,
		Census_Division,
		Parish,
		Water_Body,
		Borough
	}
	public enum GeoLevel1Type
	{
		Unknown,
		Other,
		Province,
		Region,
		District,
		State
	}
}