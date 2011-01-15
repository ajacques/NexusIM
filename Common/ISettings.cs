using System;
using System.Collections;
using System.Collections.Generic;

namespace InstantMessage
{
	/// <summary>
	/// Contains all the special processing instructions that can be used on settings
	/// </summary>
	[Flags]
	public enum SettingAttributes
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
	public interface ISettings
	{
		/// <summary>
		/// Loads all settings and accounts from the settings file.
		/// </summary>
		void Load();
		void Save();

		bool AutoSave
		{
			get;
			set;
		}
		bool IsLoaded
		{
			get;
		}
		Dictionary<IMBuddy, Dictionary<string, string>> ContactSettings
		{
			get;
		}
		event EventHandler onFileLoad;

		void SetContactSetting(IMBuddy buddy, string setting, string value);
		void SetContactSetting(string username, IMProtocol protocol, string setting, string value);
		void SetCustomSetting(string setting, string value);
		void SetSettingList(string name, List<string> list);

		void DeleteContactSetting(IMBuddy buddy, string setting);
		void DeleteContactSetting(string username,IMProtocol protocol, string setting);
		void DeleteCustomSetting(string setting);
		void DeleteSettingList(string list);

		string GetContactSetting(string userName, IMProtocol protocol, string setting, string defaultValue);
		string GetContactSetting(IMBuddy buddy, string setting, string defaultValue);
		string GetCustomSetting(string setting, string defaultValue);
		List<string> GetSettingList(string list);
	}
}
