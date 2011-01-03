using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmBuddyOptions : Form
	{
		public frmBuddyOptions()
		{
			InitializeComponent();
		}

		// Variables
		private IMBuddy mBuddy;

		// Properties
		public IMBuddy Buddy
		{
			get {
				return mBuddy;
			}
			set {
				mBuddy = value;
				this.Text = mBuddy.Username + " Options";
			}
		}

		private void chkCorrectTimezone_CheckedChanged(object sender, EventArgs e)
		{
			lblTimezone.Enabled = chkCorrectTimezone.Enabled;
			numTimezone.Enabled = chkCorrectTimezone.Enabled;
		}
	}
}