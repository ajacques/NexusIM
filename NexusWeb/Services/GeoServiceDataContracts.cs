using System.Runtime.Serialization;

namespace NexusWeb.Services.DataContracts
{
	[DataContract(Namespace = "")]
	public class GeoCity
	{
		[DataMember(Name = "City", EmitDefaultValue = false)]
		public string City;
		[DataMember(Name = "AdminLevel1", EmitDefaultValue = false)]
		public GeoLevel1 AdminLevel1;
		[DataMember(Name = "AdminLevel2", EmitDefaultValue = false)]
		public GeoLevel2 AdminLevel2;
		[DataMember(Name = "Country", EmitDefaultValue = false)]
		public GeoCountry Country;
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