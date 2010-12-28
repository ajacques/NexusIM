
namespace InstantMessage
{
	public class ChatMessage : MessageContents
	{
		public ChatMessage(string sender, bool isself, string message)
		{
			msgcontents = message;
			msgsender = sender;
			isSelf = isself;
		}
		public override string ToString()
		{
			string response = "";

			response = "<span style=\"color: ";
			if (isSelf)
				response += "blue";
			else
				response += "red";
			response += "; font-weight: bold;\">" + msgsender + ": </span>";

			response += msgcontents;

			return response;
		}
		public string Sender
		{
			get {
				return msgsender;
			}
		}
		public bool IsSelf
		{
			get {
				return isSelf;
			}
		}
		public string Contents
		{
			get {
				return msgcontents;
			}
		}

		private string msgcontents = "";
		private string msgsender = "";
		private bool isSelf = false;
	}
	public class BuzzMessage : MessageContents
	{
		public BuzzMessage(string sender)
		{
			bsender = sender;
		}
		public override string ToString()
		{
			return "<span style=\"fore-color: red\">" + bsender + " buzzed you!</span>";
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
			return "<span style=\"fore-color: red\">You buzzed " + mRecipient + ".</span>";
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
			return "<span style=\"font-weight: bold\">" + mPerson + " has joined the room.</span>";
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

			if (mMessage != "")
				return "<span style=\"font-weight: bold\">" + mPerson + " has left the room (" + mMessage + ").</span>";
			else
				return "<span style=\"font-weight: bold\">" + mPerson + " has left the room.</span>";
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