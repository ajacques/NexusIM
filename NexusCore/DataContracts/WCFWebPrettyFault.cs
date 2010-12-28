using System;
using System.Runtime.Serialization;

namespace NexusCore.PushChannel
{
	[DataContract]
	public class WCFWebPrettyFault
	{
		[DataMember]
		public string ReasonType;
		[DataMember]
		public string ReasonSubType;
		[DataMember]
		public string ReasonCode;
	}
}