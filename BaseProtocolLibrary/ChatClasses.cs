using System;
using System.Collections;

namespace InstantMessage
{
	public abstract class MessageContents
	{
		public abstract string ToString();
		public bool DisplayTimestamp
		{
			get {
				return timestampdisplay;
			}
			set {
				timestampdisplay = value;
			}
		}

		protected bool timestampdisplay = true;
	}
	public class ChatMessage : MessageContents
	{
		public ChatMessage(string sender, bool isself, string message, DateTime recvtime)
		{
			msgcontents = message;
			msgsender = sender;
			msgrecvtime = recvtime;
			isSelf = isself;
		}
		public override string ToString()
		{
			return "";
		}

		private string msgrtf = ""; // Either use this or msgcontents.. not both
		private string msgcontents = "";
		private string msgsender = "";
		private bool isSelf = false;
		private DateTime msgrecvtime;
	}
	public class BuzzMessage : MessageContents
	{
		public BuzzMessage(string sender)
		{
			bsender = sender;
		}
		public override string ToString()
		{
			return "";
		}
		private string bsender = "";
	}
	public class SendBuzzMessage : MessageContents
	{
		public SendBuzzMessage(string recipient)
		{
			mRecipient = recipient;
		}
		public override string ToString()
		{
			return "";
		}
		private string mRecipient = "";
	}
	public class UserRoomJoinMessage : MessageContents
	{
		public UserRoomJoinMessage(string person)
		{
			mPerson = person;
		}
		public override string ToString()
		{
			return "";
		}
		private string mPerson = "";
	}
	public class UserRoomLeaveMessage : MessageContents
	{
		public UserRoomLeaveMessage(string person)
		{
			mPerson = person;
		}
		public UserRoomLeaveMessage(string person, string message)
		{
			mPerson = person;
			mMessage = message;
		}
		public override string ToString()
		{
			return "";
		}
		private string mPerson = "";
		private string mMessage = "";
	}
	public class CustomMessage : MessageContents
	{
		public CustomMessage(string message)
		{
			mMessage = message;
		}
		public override string ToString()
		{
			return mMessage;
		}
		private string mMessage = "";
	}
}