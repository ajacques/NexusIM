using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	public enum IMProtocolErrorReason
	{
		CONNERROR,
		Invalid_Credentials,
		LIMIT_REACHED,
		Unknown,
		Warning
	}
	/// <summary>
	/// Used to pass information about a protocol error back to event handlers for higher-level error processing
	/// </summary>
	public class IMErrorEventArgs : EventArgs
	{
		public IMErrorEventArgs(IMProtocolErrorReason reason)
		{
			mReason = reason;
		}
		public IMErrorEventArgs(IMProtocolErrorReason reason, string message)
		{
			mReason = reason;
			mMessage = message;
		}
		public IMProtocolErrorReason Reason
		{
			get
			{
				return mReason;
			}
		}
		public string Message
		{
			get
			{
				return mMessage;
			}
			protected set
			{
				mMessage = value;
			}
		}
		public bool IsUserError
		{
			get
			{
				return mUserError;
			}
			protected set
			{
				mUserError = value;
			}
		}
		public bool IsUserCorrectable
		{
			get;
			protected set;
		}

		private bool mUserError;
		private IMProtocolErrorReason mReason;
		private string mMessage;
	}
}
