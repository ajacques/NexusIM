using System;
using System.Collections;
using MSNPSharp;
using NexusIM.Managers;
using InstantMessage.Events;

namespace InstantMessage
{
	public class IMMSNfProtocol : IMProtocol
	{
		public IMMSNfProtocol()
		{
			protocolType = "MSN";
			mProtocolTypeShort = "msn";
			messenger = new Messenger();
			messenger.OIMService.OIMReceived += new EventHandler<OIMReceivedEventArgs>(OIMService_OIMReceived);
			messenger.Nameserver.SignedIn += new EventHandler<EventArgs>(Nameserver_SignedIn);
			messenger.Nameserver.ContactOnline += new EventHandler<ContactEventArgs>(Nameserver_ContactOnline);
			messenger.Nameserver.ContactOffline += new EventHandler<ContactEventArgs>(Nameserver_ContactOffline);
			messenger.Nameserver.SignedOff += new EventHandler<SignedOffEventArgs>(Nameserver_SignedOff);
			messenger.Nameserver.AuthenticationError += new EventHandler<ExceptionEventArgs>(Nameserver_AuthenticationError);
			messenger.Nameserver.ServerErrorReceived += new EventHandler<MSNErrorEventArgs>(Nameserver_ServerErrorReceived);
			messenger.Nameserver.ExceptionOccurred += new EventHandler<ExceptionEventArgs>(Nameserver_ExceptionOccurred);
			messenger.Nameserver.AutoSynchronize = true;
		}

		public override void BeginLogin()
		{
			messenger.Credentials.Account = mUsername;
			messenger.Credentials.Password = mPassword;
			messenger.Connect();
			mEnabled = true;
		}
		public override void Disconnect()
		{
			messenger.Disconnect();
			CleanupBuddyList();
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (!mEnabled)
				return;

			if (newstatus == IMStatus.IDLE && mEnabled == false)
				return;

			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus) && mEnabled)
				BeginLogin();
			else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus))
				Disconnect();
		}
		public override void SendMessage(string friendName, string message)
		{
			//messenger.Nameserver.SendOIMMessage(friendName, message);
			messenger.OIMService.SendOIMMessage(friendName, message);
		}
		public override void RemoveFriend(string uname, string group)
		{
			IMBuddy buddy = IMBuddy.FromUsername(uname, this);
			buddy.ContactItemVisible = false;
			buddylist.Remove(buddy);
			messenger.ContactService.RemoveContact(messenger.ContactList.GetContact(uname));
		}
		public override string GetServerString(string username)
		{
			return "messenger.hotmail.com";
		}

		// Private Functions
		private void HandleStatus(IMBuddy buddy, PresenceStatus newstatus)
		{
			buddy.Status = IMBuddyStatus.Available;
			if (newstatus == PresenceStatus.Idle)
				buddy.Status = IMBuddyStatus.Idle;
			else if (newstatus == PresenceStatus.Away)
				buddy.Status = IMBuddyStatus.Away;
			else if (newstatus == PresenceStatus.Offline)
				buddy.Status = IMBuddyStatus.Offline;
		}

		// Internal Callbacks
		private void OIMService_OIMReceived(object sender, OIMReceivedEventArgs e)
		{
			IMBuddy buddy = IMBuddy.FromUsername(e.Email, this);
			buddy.ShowRecvMessage(e.Message);
		}
		private void Nameserver_SignedIn(object sender, EventArgs e)
		{
			IEnumerator contacts = messenger.ContactList.GetEnumerator();

			while (contacts.MoveNext())
			{
				try {
					Contact contact = ((System.Collections.Generic.KeyValuePair<Int32, Contact>)contacts.Current).Value;
					IMBuddy buddy = new IMBuddy(this, contact.Name);
					if (contact.NickName != null)
						buddy.Nickname = contact.NickName;
					HandleStatus(buddy, contact.Status);

					if (contact.ContactGroups.Count > 0)
						buddy.Group = contact.ContactGroups[0].Name;
	
					buddy.Populate();
					buddylist.Add(buddy);
				} catch (Exception f) {}
			}
			status = IMProtocolStatus.ONLINE;
		}
		private void Nameserver_ContactOnline(object sender, ContactEventArgs e)
		{
			try {
				IMBuddy buddy = AccountManager.GetByName(e.Contact.Name);
				HandleStatus(buddy, e.Contact.Status);
			} catch (Exception) { }
		}
		private void Nameserver_ContactOffline(object sender, ContactEventArgs e)
		{
			try
			{
				IMBuddy buddy = AccountManager.GetByName(e.Contact.Name);
				HandleStatus(buddy, e.Contact.Status);
			}
			catch (Exception) { }
		}
		private void Nameserver_SignedOff(object sender, SignedOffEventArgs e)
		{
			if (e.SignedOffReason == SignedOffReason.OtherClient)
			{
				triggerOnDisconnect(this, new IMDisconnectEventArgs(DisconnectReason.OtherClient));
			}
			status = IMProtocolStatus.OFFLINE;
		}
		private void Nameserver_AuthenticationError(object sender, ExceptionEventArgs e)
		{
			if (e.Exception.InnerException.Message == "Authentication Failure")
			{
				triggerOnError(new IMErrorEventArgs(IMErrorEventArgs.ErrorReason.INVALID_PASSWORD));
			}
		}
		private void Nameserver_ServerErrorReceived(object sender, MSNErrorEventArgs e)
		{
			e = e;
		}
		private void Nameserver_ExceptionOccurred(object sender, ExceptionEventArgs e)
		{
			e = e;
		}
		private void contact_DisplayImageChanged(object sender, EventArgs e)
		{
			e = e;
		}

		// Internal Variables
		Messenger messenger;
	}
}