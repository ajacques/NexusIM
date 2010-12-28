using System;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InstantMessage
{
	public partial class IMBuddy : ISerializable
	{
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("username", mUsername);
			info.AddValue("nickname", mNickname);
			info.AddValue("status", mStatus);
		}
	}

	internal static class DateExtensions
	{
		public static int ToUnixEpoch(this DateTime source)
		{
			DateTime epoch = new DateTime(1970, 1, 1);
			return (int)source.Subtract(epoch).TotalSeconds;
		}
	}
}