using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using InstantMessage;

namespace InstantMessage
{
	public sealed class IMSettings
	{
		private IMSettings() {}

		public static void Load()
		{
			if (mSettingMgr != null)
				mSettingMgr.Load();
		}
		public static void SaveAccounts()
		{
			if (mSettingMgr != null)
				mSettingMgr.Save();
		}
		/// <summary>
		/// Retrieves a custom setting from the user's configuration file
		/// </summary>
		/// <param name="setting">Which setting to retrieve</param>
		/// <param name="defaultValue">Default value to return if no setting has been found</param>
		/// <returns>The setting value</returns>
		public static string GetCustomSetting(string setting, string defaultValue)
		{
			if (mSettingMgr != null)
				return mSettingMgr.GetCustomSetting(setting, defaultValue);
			else
				return defaultValue;
		}
		/// <summary>
		/// Retrieves a custom setting for a buddy from the user's configuration file
		/// </summary>
		/// <param name="buddy"></param>
		/// <param name="setting"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string GetContactSetting(IMBuddy buddy, string setting, string defaultValue)
		{
			if (mSettingMgr != null)
				return mSettingMgr.GetContactSetting(buddy, setting, defaultValue);
			else
				return defaultValue;
		}
		public static string GetContactSetting(string userName, IMProtocol protocol, string setting, string defaultValue)
		{
			if (mSettingMgr != null)
				return mSettingMgr.GetContactSetting(userName, protocol, setting, defaultValue);
			else
				return defaultValue;
		}
		/// <summary>
		/// Assigns a value to custom setting in the user's configuration file
		/// </summary>
		/// <param name="setting">The setting to change the value of</param>
		/// <param name="value">What value to use</param>
		public static void SetCustomSetting(string setting, string value)
		{
			if (mSettingMgr != null)
				mSettingMgr.SetCustomSetting(setting, value);
		}
		public static void SetContactSetting(IMBuddy buddy, string setting, string value)
		{
			if (mSettingMgr != null)
				mSettingMgr.SetContactSetting(buddy, setting, value);
		}
		public static void SetContactSetting(string username, IMProtocol protocol, string setting, string value)
		{
			if (mSettingMgr != null)
				mSettingMgr.SetContactSetting(username, protocol, setting, value);
		}
		/// <summary>
		/// Removes a custom setting from the user's configuration file
		/// </summary>
		/// <param name="setting">What setting to remove</param>
		public static void DeleteCustomSetting(string setting)
		{
			if (mSettingMgr != null)
				mSettingMgr.DeleteCustomSetting(setting);
		}
		public static void DeleteContactSetting(IMBuddy buddy, string setting)
		{
			if (mSettingMgr != null)
				mSettingMgr.DeleteContactSetting(buddy, setting);
		}
		public static void DeleteContactSetting(string username, IMProtocol protocol, string setting)
		{
			if (mSettingMgr != null)
				mSettingMgr.DeleteContactSetting(username, protocol, setting);
		}
		
		public static string GetSystemSound(string name)
		{
			return "";
		}
		public static ISettings SettingInterface
		{
			get {
				return mSettingMgr;
			}
			set {
				mSettingMgr = value;
			}
		}

		private static ISettings mSettingMgr;
	}
	[Serializable]
	public partial class HashMismatchException : Exception
	{
		public HashMismatchException() {}
		public HashMismatchException(string data) : base(data) {}
		public HashMismatchException(string message, Exception innerException) : base (message, innerException) {}
		public HashMismatchException(string data, string hash)
		{
			mData = data;
			mHash = hash;
		}
		
		public string Contents
		{
			get {
				return mData;
			}
		}
		public string Hash
		{
			get {
				return mHash;
			}
		}
		private string mData = "";
		private string mHash = "";
	}
}