using System;
using System.ComponentModel;

namespace NexusPhone
{
	public class IMMessage : INotifyPropertyChanged
	{
		public IMMessage()
		{
#if DEBUG
			Sender = new IMBuddy();
			Sender.Username = "[test]";
			Body = "[Test Message Here]";
#endif
		}
		public IMMessage(IMBuddy sender, DateTime stamp, string message)
		{
			Sender = sender;
			Timestamp = stamp;
			Body = message;
			Read = false;
		}
		public IMMessage(IMBuddy recipient, string message)
		{
			Recipient = recipient;
			Read = false;
			Body = message;
			Timestamp = DateTime.Now;
		}
		protected void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public IMBuddy Sender
		{
			get;
			private set;
		}
		public IMBuddy Recipient
		{
			get;
			private set;
		}
		public string Body
		{
			get;
			private set;
		}
		public DateTime Timestamp
		{
			get;
			private set;
		}
		public bool Read
		{
			get;
			private set;
		}
		public string Username
		{
			get	{
				return Sender == null ? "Me" : Sender.Username;
			}
		}

		public GenericEvent OnDelivery;
		public event PropertyChangedEventHandler PropertyChanged;
	}
}