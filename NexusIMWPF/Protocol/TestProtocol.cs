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

			ContactList.Add(new IMBuddy(this, "testuser") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "testuser2") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "testuser3") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "offlineuser") { Status = IMBuddyStatus.Offline });
			ContactList.Add(new IMBuddy(this, "awayuser") { Status = IMBuddyStatus.Away });
			ContactList.Add(new IMBuddy(this, "busyuser") { Status = IMBuddyStatus.Busy });
			ContactList.Add(new IMBuddy(this, "statususer") { Status = IMBuddyStatus.Available, StatusMessage = "Test Status Message" });
			ContactList.Add(new IMBuddy(this, "groupeduser") { Group = "Test Group" });

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

			if (message == "test")
			{
				IMBuddy buddy = (IMBuddy)ContactList[friendName];
				buddy.InvokeReceiveMessage("working");
				triggerOnMessageReceive(new InstantMessage.Events.IMMessageEventArgs(buddy, "working"));
			} else if (message == "delete")	{
				IContact contact;
				if (ContactList.TryGetValue(friendName, out contact))
				{
					ContactList.Remove(friendName);
				}
			}

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