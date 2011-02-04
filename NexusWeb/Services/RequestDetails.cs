using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusCore.Databases;
using System.Runtime.Serialization;

namespace NexusWeb.Services.DataContracts
{
	public enum RequestType
	{
		Location,
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