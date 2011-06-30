using System;
using System.Collections;
using InstantMessage;
using System.Collections.Generic;

namespace InstantMessage
{
	internal static class IMSettings
	{
		public static void Setup(ISettings controller)
		{
			mSettingMgr = controller;
		}

		public static ICollection<IMProtocolWrapper> Accounts
		{
			get	{
				return mSettingMgr.Accounts;
			}
		}
		public static IDictionary<string, string> Settings
		{
			get	{
				return mSettingMgr.Settings;
			}
		}
		public static IDictionary<IMProtocol, IDictionary<string, string>> ProtocolSettings
		{
			get	{
				return mSettingMgr.ProtocolSettings;
			}
		}
		public static IChatAreaPool ChatAreaPool
		{
			get	{
				return mSettingMgr.ChatAreaPool;
			}
		}

		private static ISettings mSettingMgr;
	}
}