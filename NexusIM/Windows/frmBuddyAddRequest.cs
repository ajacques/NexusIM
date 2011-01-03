using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmBuddyAddRequest : Form
	{
		public frmBuddyAddRequest()
		{
			InitializeComponent();
		}

		public string Username
		{
			get {
				return mUsername;
			}
			set {
				mUsername = value;
			}
		}
		public string DisplayName
		{
			get {
				return mDisplayName;
			}
			set {
				mDisplayName = value;
			}
		}
		public string IntroductionMessage
		{
			get {
				return mIntroMsg;
			}
			set {
				mIntroMsg = value;
			}
		}
		public IMProtocol Protocol
		{
			get {
				return mProtocol;
			}
			set {
				mProtocol = value;
			}
		}

		private string mUsername = "";
		private string mDisplayName = "";
		private string mIntroMsg = "";
		private IMProtocol mProtocol = null;

		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x0084;

			switch (m.Msg)
			{
				case WM_NCHITTEST:
					{
						return;
					}
			}

			base.WndProc(ref m);
		}
		private void frmBuddyAddRequest_Shown(object sender, EventArgs e)
		{
			string message = "{0} has requested to be your friend.";
			if (mUsername != "")
				lblMessage.Text = String.Format(message, mUsername);

			if (mDisplayName != "")
				lblMessage.Text = String.Format(message, mDisplayName);

			if (mIntroMsg != "")
				lblIntroMsg.Text = mIntroMsg;
		}
		private void btnAccept_Click(object sender, EventArgs e)
		{
			mProtocol.ReplyToBuddyAddRequest(mUsername, true);
			this.Close();
		}
		private void btnDeny_Click(object sender, EventArgs e)
		{
			mProtocol.ReplyToBuddyAddRequest(mUsername, false);
			this.Close();
		}
	}
}
