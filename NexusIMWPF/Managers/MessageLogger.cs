using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Threading;

namespace NexusIM.Managers
{
	static class MessageLogger
	{
		public static void Setup(string connectionString)
		{
			mConnectionString = connectionString;
		}
		public static void LogMessageToRemote(IContact recipient, string message)
		{
			ChatHistory db = ChatHistory.Create(mConnectionString);
			PrivateMessage msg = new PrivateMessage();
			msg.MessageBody = message;
			msg.Timestamp = DateTime.UtcNow;
			msg.Sender = recipient.Protocol.Username;
			msg.Recipient = recipient.Username;

			db.PrivateMessages.InsertOnSubmit(msg);

			db.SubmitChanges();
		}

		private static string mConnectionString = "Data Source=ChatHistory.sdf;Persist Security Info=False;";
	}
}