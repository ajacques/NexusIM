﻿using System;
using System.Collections;
using InstantMessage;
using System.Collections.Generic;

namespace InstantMessage
{
	public static class IMSettings
	{
		public static void Setup(ISettings controller)
		{
			mSettingMgr = controller;
		}

		public static IList<IMProtocol> Accounts
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

		private static ISettings mSettingMgr;
	}
}