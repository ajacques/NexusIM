using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NewsFeedPushServer
{
	[DataContract(Namespace="NxPush")]
	public enum StreamState
	{
		[EnumMember]
		NotLoggedIn,
		[EnumMember]
		Ready,
		[EnumMember]
		Streaming
	}

	[DataContract(Namespace = "")]
	public class StreamStateChange
	{
		[DataMember(Name = "__type")]
		public readonly string type = "StreamStateChange";
		[DataMember(Name = "State")]
		public StreamState mState;
	}
}