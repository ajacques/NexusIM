using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NexusIM.Managers;

namespace InstantMessage
{
	public delegate void GenericEvent();
	public static class SettingExtensions
	{
		public static void FixContactSettings(this BasicXmlSettingsBinding setting)
		{
			IEnumerable<IMBuddy> buddies = AccountManager.MergeAllBuddyLists().Where(b => setting.ContactBackupSettings.ContainsKey(computeNameHash(b)));
			IEnumerable<KeyValuePair<IMBuddy, Dictionary<string, string>>> results = buddies.Select<IMBuddy, KeyValuePair<IMBuddy, Dictionary<string, string>>>(b => new KeyValuePair<IMBuddy, Dictionary<string, string>>(b, setting.ContactBackupSettings[computeNameHash(b)]));

			foreach (KeyValuePair<IMBuddy, Dictionary<string, string>> kvpair in results)
			{
				setting.ContactSettings.Add(kvpair.Key, kvpair.Value);
				setting.ContactBackupSettings.Remove(computeNameHash(kvpair.Key));
			}
		}

		private static string computeNameHash(IMBuddy buddy)
		{
			return buddy.Username + "-" + buddy.Protocol.Username + "-" + buddy.Protocol.ShortProtocol;
		}
	}
}