using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Runtime.Serialization;
using InstantMessage;

namespace NexusCore.PushChannel
{
	[DataContract(Name = "msg", Namespace = "")]
	internal class NewIMMessage : IToastPushMessage
	{
		public NewIMMessage(int protocolId, string sender, string messageBody)
		{
			mProtocolId = protocolId;
			mSender = sender;
			mMessageBody = messageBody;
		}
		public bool Equals(IPushMessage other)
		{
			return false;
		}

		public XmlElement XmlMessage
		{
			get
			{
				XmlDocument doc = new XmlDocument();

				XmlElement elem = doc.CreateElement("nx", "ChatMessage", "NXNotification");
				XmlElement pId = doc.CreateElement("nx", "ProtocolId", "NXNotification");
				XmlElement sender = doc.CreateElement("nx", "Sender", "NXNotification");
				XmlElement body = doc.CreateElement("nx", "MessageBody", "NXNotification");

				pId.InnerText = mProtocolId.ToString();
				sender.InnerText = mSender;
				body.InnerText = mMessageBody;

				elem.AppendChild(pId);
				elem.AppendChild(sender);
				elem.AppendChild(body);

				return elem;
			}
		}
		public PushMessageClass MessageClass
		{
			get
			{
				return PushMessageClass.Toast;
			}
		}
		public bool Unique
		{
			get
			{
				return false;
			}
		}
		public string Text1
		{
			get
			{
				return mSender;
			}
		}
		public string Text2
		{
			get
			{
				return mMessageBody;
			}
		}
		public TimeSpan MaxQueuePeriod
		{
			get
			{
				return mMaxQueuePeriod;
			}
		}
		[DataMember(Name = "_type")]
		public string MessageType
		{
			get
			{
				return "msg";
			}
		}
		[DataMember]
		public int ProtocolId
		{
			get
			{
				return mProtocolId;
			}
		}
		[DataMember]
		public string Sender
		{
			get	{
				return mSender;
			}
		}
		[DataMember]
		public string MessageBody
		{
			get	{
				return mMessageBody;
			}
		}

		private static TimeSpan mMaxQueuePeriod = new TimeSpan(0, 0, 1);
		private int mProtocolId;
		private string mSender;
		private string mMessageBody;
	}
	[DataContract(Name = "nc", Namespace = "")]
	internal class NewContactMessage : IPushMessage
	{
		public NewContactMessage(IMBuddy data)
		{
			mData = data;
		}
		public NewContactMessage(IMBuddy buddy, int protocolId)
		{
			mData = buddy;
			this.protocolId = protocolId;
		}
		public bool Equals(IPushMessage other)
		{
			if (!(other is NewContactMessage))
				return false;
			else
				return mData == ((NewContactMessage)other).mData;
		}

		public XmlElement XmlMessage
		{
			get
			{
				XmlDocument doc = new XmlDocument();

				XmlElement elem = doc.CreateElement("nx", "ContactUpdate", "NXNotification");
				XmlElement user = doc.CreateElement("nx", "Username", "NXNotification");
				XmlElement pId = doc.CreateElement("nx", "ProtocolId", "NXNotification");
				XmlElement status = doc.CreateElement("nx", "Availability", "NXNotification");

				user.InnerText = mData.Username;
				pId.InnerText = protocolId.ToString();
				status.InnerText = mData.Status.ToString();
				elem.AppendChild(user);
				elem.AppendChild(pId);

				if (mData.Status != IMBuddyStatus.Offline)
					elem.AppendChild(status);

				return elem;
			}
		}

		public bool Combinable
		{
			get
			{
				return true;
			}
		}
		public bool Unique
		{
			get
			{
				return true;
			}
		}
		public PushMessageClass MessageClass
		{
			get
			{
				return PushMessageClass.Internal;
			}
		}
		public TimeSpan MaxQueuePeriod
		{
			get
			{
				return mMaxQueuePeriod; // This isn't as important
			}
		}
		[DataMember(Name = "_type")]
		public string MessageType
		{
			get	{
				return "nc";
			}
		}
		[DataMember]
		public string Username
		{
			get
			{
				return mData.Username;
			}
			private set { }
		}
		[DataMember]
		public int ProtocolId
		{
			get	{
				return protocolId;
			}
			private set {
				protocolId = value;
			}
		}
		[DataMember]
		public string Availability
		{
			get {
				return mData.Status.ToString();
			}
		}

		private static TimeSpan mMaxQueuePeriod = new TimeSpan(0, 0, 5);
		private IMBuddy mData;
		private int protocolId;
	}
	[DataContract(Name = "sc", Namespace = "")]
	internal class ContactStatusChangeMessage : IPushMessage
	{
		public ContactStatusChangeMessage(int protocolId, IMBuddy contact)
		{
			mProtocolId = protocolId;
			mContact = contact;
		}

		public XmlElement XmlMessage
		{
			get
			{
				XmlDocument doc = new XmlDocument();

				XmlElement elem = doc.CreateElement("nx", "StatusChange", "NXNotification");
				XmlElement user = doc.CreateElement("nx", "Username", "NXNotification");
				XmlElement pId = doc.CreateElement("nx", "ProtocolId", "NXNotification");
				XmlElement status = doc.CreateElement("nx", "Availability", "NXNotification");

				user.InnerText = mContact.Username;
				pId.InnerText = mProtocolId.ToString();
				status.InnerText = mContact.Status.ToString();
				elem.AppendChild(user);
				elem.AppendChild(pId);
				elem.AppendChild(status);

				return elem;
			}
		}
		public PushMessageClass MessageClass
		{
			get
			{
				return PushMessageClass.Internal;
			}
		}
		public bool Equals(IPushMessage other)
		{
			if (!(other is ContactStatusChangeMessage))
				return false;
			else
				return mContact == ((ContactStatusChangeMessage)other).mContact;
		}

		public bool Combinable
		{
			get
			{
				return true;
			}
		}
		public bool Unique
		{
			get
			{
				return true;
			}
		}
		public TimeSpan MaxQueuePeriod
		{
			get
			{
				return mMaxQueuePeriod;
			}
		}
		[DataMember(Name = "_type")]
		public string MessageType
		{
			get
			{
				return "su";
			}
		}
		[DataMember]
		public int ProtocolId
		{
			get	{
				return mProtocolId;
			}
		}
		[DataMember]
		public string Username
		{
			get	{
				return mContact.Username;
			}
		}
		[DataMember]
		public string Availability
		{
			get	{
				return mContact.Status.ToString();
			}
		}

		private static TimeSpan mMaxQueuePeriod = new TimeSpan(0, 0, 5);

		private IMBuddy mContact;
		private int mProtocolId;
	}
}