using System;
using System.Collections;
using System.Collections.Generic;

namespace InstantMessage
{
	/// <summary>
	/// This class is the pipeline for all UI related communicated between the application and the protocols
	/// </summary>
	public abstract class IProtocolManager
	{
		// TODO: Mark everything here as abstract
		public abstract void AddContactListItem(IMBuddy item);
		public abstract void UpdateContactListItem(IMBuddy item);
		public abstract void RemoveContactListItem(IMBuddy buddy);
		public virtual void OpenBuddyWindow(IMBuddy buddy, bool isUserInvoked) {}
		[Obsolete("The Library shouldn't close windows", false)]
		public virtual void CloseBuddyWindow(IMBuddy buddy) {}
		[Obsolete("The Library shouldn't need this", false)]
		public virtual bool IsBuddyWindowOpen(IMBuddy buddy) { return false; }
		public virtual void ShowReceivedMessage(IMBuddy buddy, string message) {}
		public virtual IMProtocol CreateCustomProtocol(string name) { return null; }
	}
}