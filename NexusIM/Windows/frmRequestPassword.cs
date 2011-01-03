using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmRequestPassword : Form
	{
		public frmRequestPassword()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Protocol to use when changing passwords
		/// </summary>
		public IMProtocol Protocol
		{
			get {
				return protocol;
			}
			set {
				protocol = value;
			}
		}

		private IMProtocol protocol = null;

		private void frmRequestPassword_Shown(object sender, EventArgs e)
		{
			if (protocol == null)
				throw new ArgumentNullException("Protocol can't be null");

			lblAccount.Text = protocol.Protocol + " - " + protocol.Username;
		}
		private void btnIgnore_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		private void btnLogin_Click(object sender, EventArgs e)
		{
			protocol.Password = txtPassword.Text;
			protocol.SavePassword = chkRemember.Checked;
			try {
				protocol.BeginLogin();
			} catch (WarningException) {}

			IMSettings.SaveAccounts();

			this.Close();
		}
	}
}