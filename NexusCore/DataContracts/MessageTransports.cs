using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using InstantMessage;
using NexusCore.Services;
using System.Security.Permissions;
using NexusCore.Controllers;

namespace NexusCore.DataContracts
{
	[Serializable]
	public class MessageSerializable : ISerializable
	{
		public MessageSerializable(Dictionary<string, object> items, Type deserializetype)
		{
			mItems = items;
			mDeserializeType = deserializetype;
		}

		protected MessageSerializable(SerializationInfo info, StreamingContext context)
		{
			mDeserializeType = Type.GetType(info.GetString("deserializetype"));
			mItems = (Dictionary<string, object>)info.GetValue("items", typeof(Dictionary<string, object>));
		}

		public ISwarmMessage Deserialize()
		{
			var constructor = mDeserializeType.GetConstructor(new Type[] { typeof(MessageSerializable) });
			object obj = constructor.Invoke(new object[] { mItems });

			return (ISwarmMessage)obj;
		}

		[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("items", mItems);
			info.AddValue("deserializetype", mDeserializeType.FullName);
		}

		public Dictionary<string, object> Items
		{
			get {
				return mItems;
			}
		}

		private Type mDeserializeType;
		private Dictionary<string, object> mItems;
}
	/// <summary>
	/// Generic message type. Stores data on how to transport the message.
	/// </summary>
	[DataContract]
	[KnownType(typeof(SwarmChatMessage))]
	[KnownType(typeof(LocationMessage))]
	[KnownType(typeof(SwarmStatusChangeMessage))]
	[KnownType(typeof(RequestMessage))]
	[KnownType(typeof(ResponseMessage))]
	[KnownType(typeof(DisconnectMessage))]
	[KnownType(typeof(ProtocolReadyMessage))]
	public class ISwarmMessage
	{
		public ISwarmMessage() {}

		public ISwarmMessage(Dictionary<string, object> items)
		{
			mUserid = (int)items["userid"];
			mDeviceSender = (int)items["devicesender"];
		}

		/// <summary>
		/// Gets the device that this message should be delivered to
		/// </summary>
		[Obsolete]
		public int RecipientDevice
		{
			get {
				return mDeviceRecipient;
			}
			set {
				mDeviceRecipient = value;
			}
		}
		/// <summary>
		/// Gets the device that sent this message.
		/// </summary>
		[Obsolete]
		public int SenderDevice
		{
			get {
				return mDeviceSender;
			}
			internal set {
				mDeviceSender = value;
			}
		}
		/// <summary>
		/// True if this message is broadcasted to all devices on the user's account
		/// </summary>
		[Obsolete]
		public Swarm Swarm
		{
			get {
				return SwarmManager.FindSwarmByUserId(mUserid);
			}
			set {
				mUserid = value.UserId;
			}
		}

		protected Dictionary<string, object> GetItems()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict.Add("userid", mUserid);
			dict.Add("devicesender", mDeviceSender);

			return dict;
		}

		public virtual MessageSerializable Serialize()
		{
			return new MessageSerializable(GetItems(), this.GetType());
		}

		[NonSerialized]
		public int mUserid;
		[DataMember]
		public int mDeviceRecipient;
		[DataMember]
		public int mDeviceSender;
	}

	[DataContract]
	public class ProtocolReadyMessage : ISwarmMessage
	{
		public ProtocolReadyMessage(IMProtocol protocol)
		{
			mProtocolId = protocol.Guid;
			mInfo = new AccountInfo(protocol.Protocol, protocol.Username, protocol.Password);
		}

		[DataMember(Name="ProtocolId")]
		protected Guid mProtocolId;
		[DataMember(Name="AccountInfo")]
		protected AccountInfo mInfo;
	}

	[DataContract]
	public class SwarmReconnectMessage : ISwarmMessage
	{
		/// <summary>
		/// Creates a new instance of the swarm stream reconnect message to broadcast to a set of computers
		/// </summary>
		/// <param name="reason">Explains why this reconnect is neccessary</param>
		/// <param name="delay">Time in milli-seconds to wait before reconnecting</param>
		/// <param name="isSoftResume">True if the session is still setup and you can just make calls to the service with the same session id. False if you need to re-login.</param>
		public SwarmReconnectMessage(SwarmStreamReconnectReason reason, int delay, bool isSoftResume)
		{
			mReason = reason;
			mDelay = delay;
			mSoftResume = isSoftResume;
		}

		[DataMember(Name="Reason")]
		private SwarmStreamReconnectReason mReason;
		[DataMember(Name="ReconnectDelay")]
		private int mDelay;
		[DataMember(Name="IsSoftResume")]
		private bool mSoftResume;
	}

	/// <summary>
	/// Used to transport instant messages to and from the protocol gateway.
	/// </summary>
	/// <remarks>
	/// Used to transport both sent and received messages. Remember to check the Sender/Recipient properties.
	/// </remarks>
	[DataContract]
	public class SwarmChatMessage : ISwarmMessage
	{
		public SwarmChatMessage(Guid accid, string destination, string message)
		{
			mViaAccount = accid;
			mRecipient = destination;
			mMessage = message;
		}

		public override MessageSerializable Serialize()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>(base.GetItems());
			dict.Add("msgsender", mViaAccount);
			dict.Add("msgrecipient", mRecipient);
			dict.Add("msgcontents", mMessage);

			return new MessageSerializable(dict, this.GetType());
		}

		[DataMember(Name = "ViaAccount")]
		public Guid mViaAccount;
		[DataMember(Name = "Recipient")]
		public string mRecipient;
		[DataMember(Name = "MessageContents")]
		public string mMessage;
	}

	[DataContract]
	public class DisconnectMessage : ISwarmMessage
	{
		public DisconnectMessage(string reason)
		{
			mReason = reason;
		}

		public string Reason
		{
			get {
				return mReason;
			}
		}

		[DataMember]
		private string mReason;
	}

	[DataContract]
	[KnownType(typeof(RequestContactInfoMessage))]
	public class RequestMessage : ISwarmMessage
	{
		public RequestMessage()
		{
			mMessageId = Guid.NewGuid();
		}

		public Guid MessageId
		{
			get {
				return mMessageId;
			}
		}

		[DataMember]
		private Guid mMessageId;
	}

	[DataContract]
	[KnownType(typeof(RequestContactInfoResponseMessage))]
	public class ResponseMessage : ISwarmMessage
	{
		public ResponseMessage(Guid messageId)
		{
			mMessageId = messageId;
		}

		public Guid MessageId
		{
			get {
				return mMessageId;
			}
		}

		[DataMember]
		private Guid mMessageId;
	}

	/// <summary>
	/// Requests detailed information on a contact from another device.
	/// </summary>
	[DataContract]
	public class RequestContactInfoMessage : RequestMessage
	{
		public RequestContactInfoMessage(string username, AccountInfo accInfo) : base()
		{
			mUsername = username;
			mAccInfo = accInfo;
		}

		public string Username
		{
			get {
				return mUsername;
			}
		}
		public AccountInfo AccountInfo
		{
			get {
				return mAccInfo;
			}
		}

		[DataMember]
		private string mUsername = "";
		[DataMember]
		private AccountInfo mAccInfo;
	}

	[DataContract]
	public class RequestContactInfoResponseMessage : ResponseMessage
	{
		public RequestContactInfoResponseMessage(string displayName, IMBuddyStatus status, Guid messageId) : base(messageId)
		{
			mDisplayName = displayName;
			mStatus = status;
		}

		public string DisplayName
		{
			get {
				return mDisplayName;
			}
		}

		public IMBuddyStatus Status
		{
			get {
				return mStatus;
			}
		}
		
		[DataMember]
		private string mDisplayName;
		[DataMember]
		private IMBuddyStatus mStatus;
	}

	/// <summary>
	/// Used to transport 
	/// </summary>
	[DataContract]
	[KnownType(typeof(LocationServiceType))]
	public class LocationMessage : ISwarmMessage
	{
		public LocationMessage(LocationServiceType serviceType, string uid)
		{
			mServiceType = serviceType;
			mUid = uid;
		}

		[DataMember]
		private LocationServiceType mServiceType;
		[DataMember]
		private string mUid = "";
	}

	[DataContract]
	public class SwarmStatusChangeMessage : ISwarmMessage
	{
		public SwarmStatusChangeMessage(SwarmStatusChangeTypes type, Guid deviceId)
		{
			mType = type;
			mDeviceId = deviceId;
		}

		public SwarmStatusChangeTypes ChangeType
		{
			get {
				return mType;
			}
		}
		public Guid DeviceId
		{
			get {
				return mDeviceId;
			}
		}

		[DataMember]
		private SwarmStatusChangeTypes mType;
		[DataMember]
		private Guid mDeviceId;
	}
}