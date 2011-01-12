using System;
using System.Runtime.Serialization;
using System.Data.Services.Common;
using System.Collections.Generic;
using System.Linq;
using NexusWeb.Databases;

namespace NexusWeb.Services.DataContracts
{
	[DataContract(Namespace="")]
	public enum ClientArticleSourceType
	{
		[EnumMember]
		User
	}

	[DataContract(Namespace = "")]
	[KnownType(typeof(ClientStatusUpdate))]
	[DataServiceKey("ArticleId")]
	public class ClientArticleUpdate
	{
		public int ArticleId
		{
			get	{
				return mArticleId;
			}
		}
		public int UserId
		{
			get	{
				return mUserId;
			}
		}
		public string UserPrefix
		{
			get	{
				return mUserPrefix;
			}
		}
		public DateTime Timestamp
		{
			get {
				return mTimeStamp;
			}
		}
		public IEnumerable<ClientArticleComment> Comments
		{
			get	{
				return mComments;
			}
		}

		[DataMember(Name="ArticleId")]
		public int mArticleId;
		[DataMember(Name="SourceType")]
		public ClientArticleSourceType mSourceType;
		[DataMember(Name = "UserId", IsRequired = false, EmitDefaultValue = false)]
		public int mUserId;
		[DataMember(Name = "UserPrefix", IsRequired = false, EmitDefaultValue = false)]
		public string mUserPrefix;
		[DataMember(Name = "TimeStamp")]
		public DateTime mTimeStamp;
		[DataMember(Name = "IsModification")]
		public bool mIsModification;
		[DataMember(Name = "Comments")]
		public IEnumerable<ClientArticleComment> mComments;
	}

	[DataContract(Namespace = "")]
	[DataServiceKey("ArticleId")]
	public class ClientStatusUpdate : ClientArticleUpdate
	{
		public string MessageBody
		{
			get
			{
				return mMessageBody;
			}
		}
		public GeoLocation GeoTag
		{
			get	{
				return mGeoTag;
			}
		}
		public GeoCity ResolvedGeoTag
		{
			get	{
				return mResolvedGeoTag;
			}
		}
		public bool HasComments
		{
			get	{
				return mComments.Any();
			}
		}

		[DataMember(Name = "MessageBody")]
		public string mMessageBody;
		[DataMember(Name = "GeoTag", IsRequired = false, EmitDefaultValue = false)]
		public GeoLocation mGeoTag;
		[DataMember(Name = "ResolvedGeoTag", IsRequired = false, EmitDefaultValue = false)]
		public GeoCity mResolvedGeoTag;
	}

	[DataContract(Namespace="")]
	[DataServiceKey("Id")]
	public class ClientArticleComment
	{
		public int Id
		{
			get	{
				return mId;
			}
		}
		public int UserId
		{
			get	{
				return mUserId;
			}
		}
		public DateTime Timestamp
		{
			get	{
				return mTimeStamp;
			}
		}
		public string MessageBody
		{
			get	{
				return mMessageBody;
			}
		}

		[DataMember(Name = "Id")]
		public int mId;
		[DataMember(Name = "UserId")]
		public int mUserId;
		[DataMember(Name = "Timestamp")]
		public DateTime mTimeStamp;
		[DataMember(Name = "MessageBody")]
		public string mMessageBody;
	}

	[DataContract(Namespace="")]
	public class UserDetails
	{
		public UserDetails() {}
		public UserDetails(User details)
		{
			Prefix = "nx";
			UserId = details.id;
			FirstName = details.firstname;
			LastName = details.lastname;
			DateOfBirth = details.DateOfBirth;
			LocationAllowed = details.locationsharestate;
		}

		[DataMember(Order = 0)]
		public string Prefix;
		[DataMember(Order = 1)]
		public int UserId;
		[DataMember(Order = 2, EmitDefaultValue = false)]
		public string FirstName;
		[DataMember(Order = 3, EmitDefaultValue = false)]
		public string LastName;
		[DataMember(Order = 4, EmitDefaultValue = false)]
		public DateTime? DateOfBirth;
		[DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
		public ClientStatusUpdate LastStatusUpdate;
		[DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
		public bool LocationAllowed;
		[DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
		public int LocationId;
	}

	[DataContract(Namespace="")]
	public class GeoLocation
	{
		public double Latitude
		{
			get	{
				return mLatitude;
			}
		}
		public double Longitude
		{
			get {
				return mLongitude;
			}
		}
		public float? Accuracy
		{
			get	{
				return mAccuracy;
			}
		}
		public double? Altitude
		{
			get	{
				return mAltitude;
			}
		}

		[DataMember(Name = "Latitude", Order = 0)]
		public double mLatitude;
		[DataMember(Name = "Longitude", Order = 1)]
		public double mLongitude;
		[DataMember(Name = "Accuracy", Order = 3)]
		public float? mAccuracy;
		[DataMember(Name = "Altitude", Order = 4, IsRequired = false, EmitDefaultValue = false)]
		public double? mAltitude;
	}

	[DataContract(Namespace = "")]
	public enum RequestType
	{
		[EnumMember]
		Location,
		[EnumMember]
		Friend
	}

	[DataContract(Namespace = "")]
	public class RequestDetails
	{
		public RequestDetails(Request request)
		{
			Sender = request.SenderUserId;
			Message = request.MessageBody;
			Type = (RequestType)Enum.Parse(typeof(RequestType), request.RequestType, true);
			SenderFullName = request.SenderUser.firstname + " " + request.SenderUser.lastname;
			RequestId = request.Id;
		}

		[DataMember]
		public int RequestId
		{
			get;
			private set;
		}
		[DataMember]
		public string SenderFullName
		{
			get;
			private set;
		}
		[DataMember]
		public int Sender
		{
			get;
			private set;
		}
		[DataMember]
		public string Message
		{
			get;
			private set;
		}
		[DataMember]
		public RequestType Type
		{
			get;
			private set;
		}
	}
}