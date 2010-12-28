using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NexusIM.Managers;
using NexusIM.NexusCore;

namespace InstantMessage
{
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

	static class WCFExtensions
	{
		public static void StartSession(this CoreServiceClient client)
		{
			new OperationContextScope(client.InnerChannel);
		}
		public static string GetSessionId(this CoreServiceClient client)
		{
			HttpResponseMessageProperty responseProperty = OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
			return (string)responseProperty.Headers[HttpResponseHeader.SetCookie];
		}

		public static void InsertSessionId(this CoreServiceClient client, string sessionid)
		{
			new OperationContextScope(client.InnerChannel);
			HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
			requestProperty.Headers[HttpRequestHeader.Cookie] = sessionid;
			OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;
		}
	}
}