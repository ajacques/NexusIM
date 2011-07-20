using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols;

namespace InstantMessage.Events
{
	public class IMChatRoomGenericEventArgs : EventArgs
	{
		public IContact Username
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
	public enum GroupChatUserLeaveReason
	{
		UserRequested,
		Kicked
	}
	public class UserLeaveRoomEventArgs : IMChatRoomEventArgs
	{
		public UserLeaveRoomEventArgs(IContact user, string kickReason, IContact kicker)
		{
			User = user;
		}

		public IContact User
		{
			get;
			internal set;
		}
		public GroupChatUserLeaveReason LeaveReason
		{
			get;
			internal set;
		}
		/// <summary>
		/// Gets 
		/// </summary>
		public string Message
		{
			get;
			internal set;
		}
	}
}