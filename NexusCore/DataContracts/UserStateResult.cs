using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NexusCore.DataContracts
{
	[DataContract]
	public class UserStateResult
	{
		[DataMember(IsRequired = false)]
		public IEnumerable<AccountInfo> EnabledAccounts;

		[DataMember(IsRequired = false)]
		public bool PushChannelValid;
	}
}