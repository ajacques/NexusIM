using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	public class IMNetworkFeatures
	{
		public IMNetworkFeatures(bool requirePassword = false)
		{
			RequiresPassword = requirePassword;
		}

		/// <summary>
		/// True if the current protocol supports multi user chat.
		/// </summary>
		public bool SupportsMultiUserChat
		{
			get;
			private set;
		}
		public bool SupportsPerUserVisibility
		{
			get;
			private set;
		}
		/// <summary>
		/// True if the current protocol supports getting another user's attention
		/// </summary>
		public bool SupportsUserNotify
		{
			get;
			private set;
		}
		public bool SupportsIntroMessage
		{
			get;
			private set;
		}
		/// <summary>
		/// True if this account type requires a username
		/// </summary>
		public bool RequiresUsername
		{
			get;
			private set;
		}
		/// <summary>
		/// True if this account type requires a password
		/// </summary>
		public bool RequiresPassword
		{
			get;
			private set;
		}
	}
}