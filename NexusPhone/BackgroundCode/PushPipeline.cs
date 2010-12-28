using System;
using System.Linq;
using System.Net;

namespace NexusPhone
{
	internal static class PushPipeline
	{
		public static IMBuddy HandleNewContactMessage(string username, IMBuddyStatus status, int protocolId)
		{
			CloudHostedProtocol protocol = AccountManager.Accounts.First(p => p.Id == protocolId);

			IMBuddy buddy = new IMBuddy(protocol);
			buddy.Username = username;

			if (status != null)
				buddy.Status = status;
			protocol.AddContact(buddy);

			return buddy;
		}

		public static void HandleStatusUpdateMessage(string username, int protocolId, IMBuddyStatus status)
		{
			CloudHostedProtocol protocol = AccountManager.Accounts.First(p => p.Id == protocolId);
			IMBuddy contact = protocol.Contacts.Where(c => c.Username == username).FirstOrDefault();
			
			contact.Status = status;
		}

		public static void HandleChatMessageMessage(string sender, string messagebody, int protocolId)
		{
			CloudHostedProtocol protocol = AccountManager.Accounts.First(p => p.Id == protocolId);
			IMBuddy contact = protocol.Contacts.Where(c => c.Username == sender).FirstOrDefault();

			contact.ReceiveMessage(new IMMessage(contact, DateTime.Now, messagebody));
		}
	}
}