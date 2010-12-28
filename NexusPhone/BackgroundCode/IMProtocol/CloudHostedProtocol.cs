using System;
using NexusPhone.NexusCore;
using System.Collections.Generic;
using System.Linq;

namespace NexusPhone
{
	public class CloudHostedProtocol
	{
		public CloudHostedProtocol()
		{
#if DEBUG
			mAccInfo = new AccountInfo() { Username = "[Test Username]", Enabled = true};
#endif
		}
		public CloudHostedProtocol(AccountInfo accinfo)
		{
			mAccInfo = accinfo;
			mContacts = new List<IMBuddy>();
		}

		public void SendMessage(IMMessage message)
		{
			AccountManager.WebIMService.SendMessageAsync(Guid, message.Recipient.Username, message.Body, message);
		}
		public void AddContactRange(IEnumerable<IMBuddy> contacts)
		{
			mContacts.AddRange(contacts);
		}
		public void AddContact(IMBuddy buddy)
		{
			mContacts.Add(buddy);
		}

		public string Username
		{
			get {
				return mAccInfo.Username;
			}
		}
		public string AccountType
		{
			get {
				return mAccInfo.ProtocolType;
			}
		}
		public Guid Guid
		{
			get {
				return mAccInfo.Guid;
			}
		}
		public int Id
		{
			get	{
				return mAccInfo.AccountId;
			}
		}
		public AccountInfo AccountInfo
		{
			get	{
				return mAccInfo;
			}
		}
		public IEnumerable<IMBuddy> Contacts
		{
			get	{
				return mContacts;
			}
		}

		private AccountInfo mAccInfo;
		private List<IMBuddy> mContacts;
	}
}