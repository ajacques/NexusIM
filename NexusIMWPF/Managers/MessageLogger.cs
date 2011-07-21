using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Threading;
using InstantMessage.Events;

namespace NexusIM.Managers
{
	static class MessageLogger
	{
		public static void Setup(string connectionString)
		{
			mConnectionString = connectionString;

			IMProtocol.AnyMessageReceived += new EventHandler<IMMessageEventArgs>(IMProtocol_AnyMessageReceived);
		}

		private static void IMProtocol_AnyMessageReceived(object sender, IMMessageEventArgs e)
		{
			ChatHistory db = ChatHistory.Create(mConnectionString);

			PrivateMessage msg = new PrivateMessage();
			msg.MessageBody = e.Message;
			msg.Timestamp = DateTime.UtcNow;
			msg.Sender = e.Sender.Username;
			msg.Recipient = ((IMProtocol)sender).Username;

			db.PrivateMessages.InsertOnSubmit(msg);
			db.SubmitChanges();
		}
		public static void LogMessageToRemote(IContact recipient, string message)
		{
			return;

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