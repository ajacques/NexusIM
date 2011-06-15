using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InstantMessage;
using NexusCore.DataContracts;
using NexusCore.PushChannel;

namespace NexusCore.Controllers
{
	class StorageItem
	{
		public StorageItem() { }
		public TimeSpan StagnatingFor()
		{
			return DateTime.UtcNow.Subtract(LastChecked);
		}

		public IMProtocol Protocol;
		public int ProtocolId;
		public DateTime LastChecked;
		public List<BuddyData> mContacts = new List<BuddyData>();
		public List<BuddyData> mNewContacts = new List<BuddyData>();
		public List<MessageData> mMessages = new List<MessageData>(); // Messages waiting to be pushed to client
		public PushChannelContext PushContext;
	}
	/// <summary>
	/// Endpoint for all communication with the BaseProtocolLibrary. Allows for standardized management of accounts
	/// </summary>
	class WebIMProtocolManager
	{
		static WebIMProtocolManager()
		{
			if (mInstance != null)
				return;

			mInstance = new WebIMProtocolManager();
		}
		public static StorageItem SetupAccount(AccountInfo accountInfo)
		{
			IMProtocol protocol = IMProtocol.FromString(accountInfo.mProtocol);

			protocol.Username = accountInfo.Username;
			protocol.Password = accountInfo.Password;

			if (!String.IsNullOrEmpty(accountInfo.Server))
				protocol.Server = accountInfo.Server;

			protocol.Guid = accountInfo.Guid;
			SettingDbSerializer.Register(protocol);
			protocol.LoadSettings();

			return Manage(protocol);
		}
		public static IMProtocol Login(AccountInfo accountInfo)
		{
			IMProtocol protocol = SetupAccount(accountInfo).Protocol;

			protocol.BeginLogin();
			protocol.EndLogin();

			Manage(protocol);

			return protocol;
		}
		public static StorageItem Manage(IMProtocol protocol)
		{
			if (!mRotation.Any(si => si.Protocol == protocol))
			{
				StorageItem item = new StorageItem() { LastChecked = DateTime.UtcNow, Protocol = protocol };
				mRotation.Add(item);
				return item;
			}
			throw new ArgumentException();
		}
		public static void Remove(IMProtocol protocol)
		{
			StorageItem i = FindItem(protocol);
			mRotation.Remove(i);
		}
		public static void ResetIdleTimer(Guid protocolid)
		{
			StorageItem item = mRotation.Where(si => si.Protocol.Guid == protocolid).First();
			item.LastChecked = DateTime.UtcNow;
		}
		public static IMProtocol FindProtocol(string type, string username)
		{
			return mRotation.Select(si => si.Protocol).Where(p => p.Protocol == type && p.Username == username).FirstOrDefault();
		}
		public static StorageItem FindItem(IMProtocol protocol)
		{
			return mRotation.Where(p => p.Protocol == protocol).FirstOrDefault();
		}
		public static StorageItem FindItem(Guid protocolid)
		{
			return mRotation.Where(p => p.Protocol.Guid == protocolid).FirstOrDefault();
		}

		public static List<StorageItem> StorageBin
		{
			get	{
				return mRotation;
			}
		}

		private static WebIMProtocolManager mInstance;
		private static List<StorageItem> mRotation = new List<StorageItem>();
	}
}