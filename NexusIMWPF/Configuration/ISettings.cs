using System;
using System.Collections;
using System.Collections.Generic;

namespace InstantMessage
{
	/// <summary>
	/// Contains all the special processing instructions that can be used on settings
	/// </summary>
	[Flags]
	internal enum SettingAttributes
	{
		/// <summary>
		/// No special processing instructions should be applied to this setting
		/// </summary>
		None = 0x0,
		/// <summary>
		/// This setting should be encrypted in the configuration file
		/// </summary>
		Encrypted = 0x1,
		/// <summary>
		/// This setting should have an error-checking hash connected to it to prevent modification
		/// </summary>
		Hashed = 0x2
	}
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
