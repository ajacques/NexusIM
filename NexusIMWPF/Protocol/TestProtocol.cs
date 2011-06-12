using System.Diagnostics;
using InstantMessage;

namespace NexusIM
{
	[IMNetwork("test")]
	class TestProtocol : IMProtocol
	{
		public override void BeginLogin()
		{
			base.BeginLogin();
			
			Trace.WriteLine("TestProtocol: Beginning Login");

			ContactList.Add(new IMBuddy(this, "Test User") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "Offline User") { Status = IMBuddyStatus.Offline });
			ContactList.Add(new IMBuddy(this, "Away User") { Status = IMBuddyStatus.Away });
			ContactList.Add(new IMBuddy(this, "Busy User") { Status = IMBuddyStatus.Busy });
			ContactList.Add(new IMBuddy(this, "Test User") { Status = IMBuddyStatus.Available, StatusMessage = "Test Status Message" });
			ContactList.Add(new IMBuddy(this, "Grouped User") { Group = "Test Group" });

			mLoginWaitHandle.Set();

			triggerOnLogin(null);
			status = IMProtocolStatus.Online;
		}

		public override void Disconnect()
		{
			base.Disconnect();

			ContactList.Clear();
		}

		public override void SendMessage(string friendName, string message)
		{
			base.SendMessage(friendName, message);

			Trace.WriteLine("To " + friendName + ": " + message);
		}
	}
}