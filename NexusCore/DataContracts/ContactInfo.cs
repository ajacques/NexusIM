using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NexusCore.DataContracts
{
	[DataContract(Namespace = "com.nexus-im")]
	public class ContactInfo
	{
		[DataMember]
		public string FirstName
		{
			get;
			internal set;
		}
		[DataMember]
		public string LastName
		{
			get;
			internal set;
		}
		[DataMember]
		public IEnumerable<PhoneNumberStruct> PhoneNumbers
		{
			get;
			internal set;
		}
	}
}