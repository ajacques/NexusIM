using System;
using InstantMessage.Protocols;

namespace InstantMessage.Events
{
	public enum DisconnectReason
	{
		OtherClient,
		ServerError,
		User,
		Unknown,
		NetworkProblem
	}
	/// <summary>
	/// Used to pass information about a protocol error back to event handlers for higher-level error processing
	/// </summary>
	public class IMErrorEventArgs : EventArgs
	{
		public IMErrorEventArgs(ErrorReason reason)
		{
			mReason = reason;
		}
		public IMErrorEventArgs(ErrorReason reason, string message)
		{
			mReason = reason;
			mMessage = message;
		}
		public enum ErrorReason
		{
			CONNERROR,
			INVALID_USERNAME,
			INVALID_PASSWORD,
			LIMIT_REACHED,
			Unknown
		}
		public ErrorReason Reason
		{
			get	{
				return mReason;
			}
		}
		public string Message
		{
			get	{
				return mMessage;
			}
		}
		private ErrorReason mReason;
		private string mMessage;
	}
	/// <summary>
	/// Used to pass information about the protocol disconnect back to event handlers for higher-level error processing
	/// </summary>
	public class IMDisconnectEventArgs : EventArgs
	{
		public IMDisconnectEventArgs(DisconnectReason reason)
		{
			mReason = reason;
		}
		
		public DisconnectReason Reason
		{
			get	{
				return mReason;
			}
		}
		public Exception Exception
		{
			get;
			internal set;
		}

		private DisconnectReason mReason;
	}
	public class IMMessageEventArgs<T> : EventArgs
	{
		public IMMessageEventArgs(T from, string message, MessageFlags flags = MessageFlags.None)
		{
			mSender = from;
			mMessage = message;
			Flags = flags;
		}
		public T Sender
		{
			get	{
				return mSender;
			}
		}
		public string Message
		{
			get	{
				if (ComplexMessage != null)
					return ComplexMessage.ToString();
				return mMessage;
			}
		}
		public ComplexChatMessage ComplexMessage
		{
			get;
			set;
		}
		public MessageFlags Flags
		{
			get;
			internal set;
		}
		
		private T mSender;
		private string mMessage;
	}
	public class IMMessageEventArgs : IMMessageEventArgs<IContact>
	{
		public IMMessageEventArgs(IContact from, string message) : base(from, message) {}
		public IMMessageEventArgs(IContact from, string message, MessageFlags flags) : base(from, message, flags) { }
	}
	public class IMFriendEventArgs : EventArgs
	{
		public IMFriendEventArgs(IMBuddy buddy)
		{
			mBuddy = buddy;
		}
		public IMBuddy Buddy
		{
			get	{
				return mBuddy;
			}
		}
		private IMBuddy mBuddy;
	}
	public class IMFriendRequestEventArgs : EventArgs
	{
		public IMFriendRequestEventArgs(string username, string message, string displayname)
		{
			mUsername = username;
			mMessage = message;
			mDisplayName = displayname;
		}
		public IMFriendRequestEventArgs(string username, string displayname)
		{
			mUsername = username;
			mDisplayName = displayname;
		}
		public string Username
		{
			get	{
				return mUsername;
			}
		}
		public string Message
		{
			get	{
				return mMessage;
			}
		}
		public string DisplayName
		{
			get	{
				return mDisplayName;
			}
		}
		private string mUsername = "";
		private string mMessage = "";
		private string mDisplayName = "";
	}
	public class IMEmailEventArgs : EventArgs
	{
		public IMEmailEventArgs(string sender, string displayname, string subject)
		{
			mSender = sender;
			mDisplayName = displayname;
			mSubject = subject;
		}
		public string Sender
		{
			get	{
				return mSender;
			}
		}
		public string DisplayName
		{
			get	{
				return mDisplayName;
			}
		}
		public string Subject
		{
			get	{
				return mSubject;
			}
		}
		private string mSender = "";
		private string mDisplayName = "";
		private string mSubject = "";
	}
	public class IMRoomInviteEventArgs : EventArgs
	{
		public IMRoomInviteEventArgs(string sender, string roomname, string message)
		{
			mSender = sender;
			mRoomName = roomname;
			mMessage = message;
		}
		public string Sender
		{
			get	{
				return mSender;
			}
		}
		public string RoomName
		{
			get	{
				return mRoomName;
			}
		}
		public string Message
		{
			get	{
				return mMessage;
			}
		}
		private string mSender = "";
		private string mRoomName = "";
		private string mMessage = "";
	}
	public class IMSendMessageEventArgs : EventArgs
	{
		public IMSendMessageEventArgs(IMProtocol fromProtocol, IMBuddy buddy, string message)
		{
			mProtocol = fromProtocol;
			mBuddy = buddy;
			mMessage = message;
		}
		public IMProtocol Protocol
		{
			get
			{
				return mProtocol;
			}
		}
		public bool Handled
		{
			get
			{
				return mHandled;
			}
			set
			{
				mHandled = value;
			}
		}
		public IMBuddy Buddy
		{
			get
			{
				return mBuddy;
			}
		}
		public string Message
		{
			get
			{
				return mMessage;
			}
		}
		private IMProtocol mProtocol;
		private bool mHandled = false;
		private IMBuddy mBuddy;
		private string mMessage = "";
	}
	public class IMChatRoomGenericEventArgs : EventArgs
	{
		public string Username
		{
			get;
			internal set;
		}
		public string Message
		{
			get;
			internal set;
		}
		public bool UserRequested
		{
			get;
			internal set;
		}
	}
	public class IMChatRoomEventArgs : EventArgs
	{
		public IChatRoom ChatRoom
		{
			get;
			internal set;
		}
	}
}