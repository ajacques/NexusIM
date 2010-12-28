using System;
using System.Runtime.Serialization;

namespace NexusWeb.Services.DataContracts
{
	[DataContract]
	public class AlbumDetails
	{
		[DataMember]
		public string Name;
	}
}