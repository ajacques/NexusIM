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
			ContactList.Add(new IMBuddy(this, "Test User 2") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "Test User 3") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "Offline User") { Status = IMBuddyStatus.Offline });
			ContactList.Add(new IMBuddy(this, "Away User") { Status = IMBuddyStatus.Away });
			ContactList.Add(new IMBuddy(this, "Busy User") { Status = IMBuddyStatus.Busy });
			ContactList.Add(new IMBuddy(this, "Test User") { Status = IMBuddyStatus.Available, StatusMessage = "Test Status Message" });
			ContactList.Add(new IMBuddy(this, "Grouped User") { Group = "Test Group" });

			mSelf = new IMBuddy(this, "You");
			mSelf.Status = (IMBuddyStatus)this.Status;
			ContactList.Add(mSelf);

			mLoginWaitHandle.Set();

			OnLogin();
			mProtocolStatus = IMProtocolStatus.Online;
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

		protected override void OnStatusChange(IMStatus oldStatus, IMStatus newStatus)
		{
			base.OnStatusChange(oldStatus, newStatus);

			mSelf.Status = (IMBuddyStatus)newStatus;
		}

		private IMBuddy mSelf;
	}
}