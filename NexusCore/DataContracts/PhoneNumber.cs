using System;
using System.Runtime.Serialization;

namespace NexusCore.DataContracts
{
	[DataContract(Namespace = "com.nexus-im")]
	public enum PhoneType
	{
		[EnumMember]
		Mobile,
		[EnumMember]
		Home,
		[EnumMember]
		Office,
		Fax
	}

	[DataContract(Name = "PhoneNumber", Namespace = "com.nexus-im")]
	public class PhoneNumberStruct
	{
		public override string ToString()
		{
			if (CountryCode == 1)
			{
				return String.Format("+{0} ({1}) {2}-{3}", CountryCode.ToString(), SubscriberNumber.Substring(0, 3), SubscriberNumber.Substring(3, 3), SubscriberNumber.Substring(6));
			}
			return SubscriberNumber;
		}

		[DataMember]
		public PhoneType Type
		{
			get;
			internal set;
		}
		[DataMember]
		public ushort CountryCode
		{
			get;
			internal set;
		}
		[DataMember]
		public string SubscriberNumber
		{
			get;
			internal set;
		}
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ushort? Extension
		{
			get;
			internal set;
		}
	}
}