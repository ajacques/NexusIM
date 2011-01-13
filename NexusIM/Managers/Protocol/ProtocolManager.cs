using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using InstantMessage;
using NexusIM.Controls;

namespace NexusIM.Managers
{
	class ContactInfo
	{
		public ContactInfo(IMBuddy buddy, BuddyItem item)
		{
			mBuddy = buddy;
			mItem = item;
		}
		public IMBuddy Buddy
		{
			get {
				return mBuddy;
			}
		}
		public BuddyItem Item
		{
			get {
				return mItem;
			}
		}
		public frmChatWindow Window
		{
			get {
				return mWindow;
			}
			set {
				mWindow = value;
			}
		}
		private IMBuddy mBuddy;
		private BuddyItem mItem;
		private frmChatWindow mWindow;
	}
	/// <summary>
	/// Handles all platform-specific functions. Anything to changes between platforms goes through this
	/// </summary>
	class ProtocolManager : IProtocolManager
	{
		/// <summary>
		/// Registers this class the platform handling class
		/// Must be run before any Protocols are Created
		/// </summary>
		public static void Setup()
		{
			mInstance = new ProtocolManager();
			AccountManager.onAllFinishConnecting += new EventHandler(AccountManager_onAllFinishConnecting);
			IMProtocol.CustomProtocolManager = Instance;

			mInstance.mContacts = new List<ContactInfo>();
		}
		public static void Shutdown()
		{
			mInstance = null;
			AccountManager.onAllFinishConnecting -= new EventHandler(AccountManager_onAllFinishConnecting);
		}

		public override void AddContactListItem(IMBuddy item)
		{
			if (mContactList == null)
				mContactList = frmMain.Instance.ContactList;

			if (mContacts.Count(c => c.Buddy == item) == 0)
			{
				BuddyItem genItem = new BuddyItem(item);
				mContactList.AddContact(genItem);
				mContacts.Add(new ContactInfo(item, genItem));
			}

		}
		public override void UpdateContactListItem(IMBuddy buddy)
		{
			if (mContactList == null)
				mContactList = frmMain.Instance.ContactList;
			mContactList.Invalidate(); // Causes the contact list control to re-paint itself

			var contactinfo = from c in mContacts where c.Buddy == buddy select c;

			if (contactinfo.Count() == 0)
				AddContactListItem(buddy);

			var contact = contactinfo.First();

			if (contact.Buddy.Avatar != null)
			{

			}
		}
		public override void RemoveContactListItem(IMBuddy buddy)
		{
			if (mContacts.Any(c => c.Buddy == buddy))
			{
				mContactList.RemoveContact(mContacts.First(c => c.Buddy == buddy).Item);
				mContacts.RemoveAll(c => c.Buddy == buddy);
			}
		}
		public override bool IsBuddyWindowOpen(IMBuddy buddy)
		{
			return mContacts.Where(b => b.Buddy == buddy).ToList().Exists(c => c.Window != null);
		}
		public override void OpenBuddyWindow(IMBuddy buddy, bool isUserInvoked)
		{
			if (!IsBuddyWindowOpen(buddy))
			{
				if (frmMain.Instance.InvokeRequired)
				{
					MethodInvoker invoker = new MethodInvoker(delegate() { OpenBuddyWindow(buddy, isUserInvoked); });
					frmMain.Instance.BeginInvoke(invoker); // Async version. This might cause timing issues if it's called twice in rapid succession
				} else {
					if (onWindowOpen != null)
						onWindowOpen(buddy, null);
					frmChatWindow chatwin = new frmChatWindow();
					chatwin.Buddy = buddy;
					chatwin.Protocol = buddy.Protocol;

					if (!isUserInvoked)
						chatwin.WindowState = FormWindowState.Minimized; // Can we do this in a way that causes it to still be painted?

					chatwin.Show();
					mContacts.Where(b => b.Buddy == buddy).First().Window = chatwin;
				}
			}
		}
		public override void ShowReceivedMessage(IMBuddy buddy, string message)
		{
			if (IsBuddyWindowOpen(buddy))
				OpenBuddyWindow(buddy, false);

			var window = from c in mContacts where c.Buddy == buddy select c.Window;

			window.First().AppendCustomMessage(new ChatMessage(buddy.DisplayName, false, message));
		}
		public override IMProtocol CreateCustomProtocol(string name)
		{
			// These are all protocols that the other platforms don't support yet
			if (name == "msn")
				return new IMMSNProtocol();
			else if (name == "pnm")
				return new IMPNMProtocol();
			else if (name == "jabber")
				return new IMJabberProtocol();
			else
				return null;
		}
		public void HandleContactWindowClose(IMBuddy buddy)
		{
			if (mContacts.Exists(p => p.Buddy == buddy))
				mContacts.Where(p => p.Buddy == buddy).First().Window = null;
		}

		public static ProtocolManager Instance
		{
			get	{
				return (ProtocolManager)mInstance;
			}
		}
		public List<frmChatWindow> Windows
		{
			get {
				return new List<frmChatWindow>(mContacts.Select(c => c.Window));
			}
		}

		private string computeContactImageFileName(IMBuddy buddy)
		{
			CRC32Managed crc = new CRC32Managed();

			byte[] input = Encoding.Default.GetBytes(buddy.Protocol.ShortProtocol + buddy.Protocol.Username + buddy.Username);
			byte[] output = crc.ComputeHash(input, 0, input.Length);
			return BitConverter.ToString(output).ToLower().Replace("-", "");
		}
		private static void AccountManager_onAllFinishConnecting(object sender, EventArgs e)
		{
			Notifications.Setup();
		}
		private struct AsyncDataStruct
		{
			public object ObjectA;
			public HttpWebRequest request;
		}

		public static event EventHandler onWindowOpen;

		private string mCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NexusIM\\cache");
		private static ProtocolManager mInstance;
		private List<ContactInfo> mContacts;
		private BuddyList mContactList;
	}
}