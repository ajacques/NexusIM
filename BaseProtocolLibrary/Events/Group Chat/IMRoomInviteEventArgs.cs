using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Events
{
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
}