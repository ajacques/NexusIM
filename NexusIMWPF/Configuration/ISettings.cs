using System;
using System.Collections;
using System.Collections.Generic;

namespace InstantMessage
{
	/// <summary>
	/// A generic interface that is used to save settings to a file
	/// </summary>
	/// <remarks>
	/// This is the new abstraction style settings system. This will allow different configuration types for special platforms.
	/// </remarks>
	internal interface ISettings
	{
		ICollection<IMProtocolWrapper> Accounts
		{
			get;
		}
		IDictionary<string, string> Settings
		{
			get;
		}
		IDictionary<IMProtocol, IDictionary<string, string>> ProtocolSettings
		{
			get;
		}
		IChatAreaPool ChatAreaPool
		{
			get;
		}
	}
}
