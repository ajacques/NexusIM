using System;
using System.Drawing;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmContactDetailPopup : Form
	{
		public frmContactDetailPopup()
		{
			InitializeComponent();
		}

		public bool isVisible = false;
		private IMBuddy mBuddy = null;
		public IMBuddy Buddy
		{
			get {
				return mBuddy;
			}
			set {
				mBuddy = value;
				UpdateBuddyStuff();
			}
		}

		private void frmContactDetailPopup_Load(object sender, EventArgs e)
		{
			this.Location = new Point(frmMain.Instance.Location.X - this.Width, Control.MousePosition.Y - (this.Height / 2));
			
			if (mBuddy.Protocol.SupportsPerUserVisibility)
			{
				chkVisibility.Enabled = true;
				if (mBuddy.Protocol.IsOnlineStatusToOthers(mBuddy.Protocol.Status))
				{
					chkVisibility.Items.Add("Appear Online to this contact");
					chkVisibility.Items.Add("Appear Permanently Offline to this contact");
				} else if (mBuddy.Protocol.Status == IMStatus.INVISIBLE) {
					chkVisibility.Items.Add("Appear Online to this contact");
					chkVisibility.Items.Add("Appear Offline to this contact");
					chkVisibility.Items.Add("Appear Permanently Offline to this contact");
				}
				if (mBuddy.Options.ContainsKey("InvisiStatus"))
				{
					if (mBuddy.Options["InvisiStatus"] == "PermInvisi")
						chkVisibility.SelectedItem = "Appear Permanently Offline to this contact";
					else if (mBuddy.Options["InvisiStatus"] == "Online")
						chkVisibility.SelectedItem = "Appear Online to this contact";
					else if (mBuddy.Options["InvisiStatus"] == "Offline")
						chkVisibility.SelectedItem = "Appear Offline to this contact";
				}
			}
			//delayShowTimer.Start();
		}

		private void delayShowTimer_Tick(object sender, EventArgs e)
		{
			MethodInvoker invoker = new MethodInvoker(delegate() { this.Show(); });
			frmMain.Instance.Invoke(invoker);
			delayShowTimer.Stop();
			frmMain.Instance.Focus();
			isVisible = true;
		}
		private void UpdateBuddyStuff()
		{
			//if (mBuddy.BuddyImage != null)
			//	buddyImage.Image = mBuddy.BuddyImage;

			lblUsername.Text = mBuddy.Username;
		}
	}
}
