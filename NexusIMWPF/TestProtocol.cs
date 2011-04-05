using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.Diagnostics;
using System.Threading;

namespace NexusIM
{
	[IMNetwork("test")]
	class TestProtocol : IMProtocol
	{
		public override void BeginLogin()
		{
			base.BeginLogin();
			
			ContactList.Add(new IMBuddy(this, "Test User") { Status = IMBuddyStatus.Available });
			ContactList.Add(new IMBuddy(this, "Offline User") { Status = IMBuddyStatus.Offline });
			ContactList.Add(new IMBuddy(this, "Away User") { Status = IMBuddyStatus.Away });
			ContactList.Add(new IMBuddy(this, "Busy User") { Status = IMBuddyStatus.Busy });
			ContactList.Add(new IMBuddy(this, "Test User") { Status = IMBuddyStatus.Available, StatusMessage = "Test Status Message" });

			mLoginWaitHandle.Set();

			triggerOnLogin(null);
		}

		public override void SendMessage(string friendName, string message)
		{
			base.SendMessage(friendName, message);

			Trace.WriteLine("To " + friendName + ": " + message);
		}
	}
}