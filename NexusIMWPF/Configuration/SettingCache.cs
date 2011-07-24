using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;

namespace NexusIM
{
	internal static class SettingCache
	{
		static SettingCache()
		{
			mCache = new SortedDictionary<string, string>();
		}

		public static string GetValue(string key)
		{
			string output;
			if (mCache.TryGetValue(key, out output))
				return output;

			output = IMSettings.Settings[key];
			mCache.Add(key, output);

			return output;
		}

		public static void SetValue(string key, string value)
		{
			if (mCache.ContainsKey(key))
				mCache[key] = value;
			else
				mCache.Add(key, value);

			IMSettings.Settings[key] = value;
		}

		private static SortedDictionary<string, string> mCache;
	}
}