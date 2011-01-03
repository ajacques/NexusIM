using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmQuickLogin : Form
	{
		public frmQuickLogin()
		{
			InitializeComponent();
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

		private IMProtocol mProtocol;

		private void frmQuickLogin_Shown(object sender, EventArgs e)
		{
			if (mProtocol == null)
				return;

			txtUsername.Text = mProtocol.Username;
			txtPassword.Text = mProtocol.Password;
		}

		private void btnContinue_Click(object sender, EventArgs e)
		{
			this.Close();
			mProtocol.Username = txtUsername.Text;
			mProtocol.Password = txtPassword.Text;

			mProtocol.BeginLogin();
		}
	}
}
