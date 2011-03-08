using System;
using System.Runtime.Serialization;
using InstantMessage;

namespace NexusCore.DataContracts
{
	[DataContract]
	public class ContactLocationInfo
	{
		internal ContactLocationInfo(LocationServiceType serviceType, int rowId, AccountInfo account, string username)
		{
			mServiceType = serviceType;
			mRowId = rowId;
			mAccount = account;
			mUsername = username;
		}

		public LocationServiceType ServiceType
		{
			get {
				return mServiceType;
			}
		}
		/// <summary>
		/// Used to later request this user's location without revealing the user's identification code
		/// </summary>
		public int RowId
		{
			get {
				return mRowId;
			}
		}
		public AccountInfo AccountInfo
		{
			get {
				return mAccount;
			}
		}
		public string Username
		{
			get {
				return mUsername;
			}
		}

		[DataMember(Name = "ServiceType")]
		public LocationServiceType mServiceType;
		[DataMember(Name = "LocationId")]
		public int mRowId;
		[DataMember(Name = "AccountInfo")]
		public AccountInfo mAccount;
		[DataMember(Name = "Username")]
		public string mUsername = "";
		[DataMember(Name = "Messagable")]
		public bool mMessagable;
	}

	[DataContract]
	public class UserLocationData
	{	
		public UserLocationData() {}
		public UserLocationData(double lat, double lon)
		{
			mLatitude = lat;
			mLongitude = lon;
		}
		public UserLocationData(double lat, double lon, int accuracy, DateTime changed)
		{
			mLatitude = lat;
			mLongitude = lon;
			mAccuracy = accuracy;
			mChange = changed;
		}
		public double Latitude
		{
			get {
				return mLatitude;
			}
			internal set {
				mLatitude = value;
			}
		}
		public double Longitude
		{
			get {
				return mLongitude;
			}
			internal set {
				mLongitude = value;
			}
		}
		public int Accuracy
		{
			get {
				return mAccuracy;
			}
			internal set {
				mAccuracy = value;
			}
		}
		public DateTime TimeChanged
		{
			get {
				return mChange;
			}
			internal set {
				mChange = value;
			}
		}
		public LocationServiceType ServiceType
		{
			get {
				return mServiceType;
			}
			internal set {
				mServiceType = value;
			}
		}
		public int RowId
		{
			get {
				return mRowId;
			}
			internal set {
				mRowId = value;
			}
		}

		[DataMember(Name = "Latitude")]
		public double mLatitude;
		[DataMember(Name = "Longitude")]
		public double mLongitude;
		[DataMember(Name = "Accuracy")]
		public int mAccuracy;
		[DataMember(Name = "LastUpdated")]
		public DateTime mChange;
		[DataMember(Name = "ServiceType")]
		public LocationServiceType mServiceType;
		[DataMember(Name = "RowId")]
		public int mRowId;
		[DataMember(Name = "ReverseGeocode")]
		public string mGeocode;
	}

	[DataContract(Namespace = "com.nexus-im")]
	public class AccountInfo
	{
		internal AccountInfo(string protocol, string username)
		{
			mProtocol = protocol;
			mUsername = username;
		}
		internal AccountInfo(string protocol, string username, string password)
		{
			mProtocol = protocol;
			mUsername = username;
			mPassword = password;
		}

		public static implicit operator AccountInfo(NexusCore.Databases.Account account)
		{
			return new AccountInfo(account.acctype, account.username) { mAccountId = account.id, mEnabled = account.enabled };
		}

		public string Username
		{
			get {
				return mUsername;
			}
		}
		public string Password
		{
			get {
				return mPassword;
			}
		}
		public string Server
		{
			get {
				return mServer;
			}
			set {
				mServer = value;
			}
		}
		public Guid Guid
		{
			get {
				return mGuid;
			}
			internal set {
				mGuid = value;
			}
		}

		[DataMember(Name = "ProtocolType", Order = 0)]
		public string mProtocol;
		[DataMember(Name = "Username", Order = 1)]
		public string mUsername;
		[DataMember(Name = "Password", IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public string mPassword;
		[DataMember(Name = "EncryptedPassword", IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public byte[] mEncPassword;
		[DataMember(Name = "Server", IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public string mServer;
		[DataMember(Name = "Guid", IsRequired = false, EmitDefaultValue = false, Order = 5)]
		public Guid mGuid;
		[DataMember(Name = "Enabled", EmitDefaultValue = false, Order = 4)]
		public bool mEnabled;
		[DataMember(Name = "AccountId", EmitDefaultValue = false)]
		public int mAccountId;
		[DataMember(Name = "Status", EmitDefaultValue = false)]
		public IMProtocolStatus mStatus;
	}

	[DataContract]
	public class MessageData
	{
		public MessageData(Guid pId, string sender, string message)
		{
			mProtocol = pId;
			mSender = sender;
			mMessage = message;
		}
		[DataMember]
		public Guid mProtocol;
		[DataMember]
		public string mSender = "";
		[DataMember]
		public string mMessage = "";
	}

	[DataContract]
	public class BuddyData
	{
		public BuddyData(Guid guid, string nickname, string statusMsg, IMBuddyStatus status)
		{
			mNickname = nickname;
			statusMsg = mStatusMessage;
			mStatus = status;
			mGuid = guid;
		}

		public static implicit operator BuddyData(IMBuddy buddy)
		{
			return new BuddyData(new Guid(), buddy.Nickname, buddy.StatusMessage, buddy.Status);
		}

		[DataMember(Name = "Username")]
		public string mUsername;
		[DataMember(Name = "Nickname")]
		public string mNickname;
		[DataMember]
		public string mStatusMessage;
		[DataMember]
		public IMBuddyStatus mStatus;
		[DataMember]
		public Guid mGuid;
		[DataMember]
		public Guid mProtocolGuid;
	}

	[DataContract]
	public class MyAccountInformation
	{
		public MyAccountInformation(string username, string firstname)
		{
			mUsername = username;
			mFirstName = firstname;
		}

		public string Username
		{
			get {
				return mUsername;
			}
		}
		public string FirstName
		{
			get {
				return mFirstName;
			}
		}

		[DataMember]
		private string mUsername = "";
		[DataMember]
		private string mFirstName = "";
	}
}