using System;
using System.Windows.Forms;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM.Windows
{
	public partial class frmSelectContact : Form
	{
		public frmSelectContact()
		{
			InitializeComponent();
		}

		private IMBuddy mBuddyResult;

		/// <summary>
		/// Gets the buddy selected by the user if there is one.
		/// </summary>
		public IMBuddy Buddy
		{
			get {
				return mBuddyResult;
			}
		}

		private void frmSelectContact_Load(object sender, EventArgs e)
		{
			foreach (IMBuddy pt in AccountManager.MergeAllBuddyLists())
				contacts.AddBuddy(pt);
		}
	}
}
