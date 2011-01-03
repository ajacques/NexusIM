using System;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	/// <summary>
	/// Allows the user to create new and edit accounts.
	/// </summary>
	public partial class frmNewAccount : Form
	{
		public frmNewAccount()
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
			cmbProtocol.SelectedIndex = 0;
		}

		public Guid Guid
		{
			get {
				return mGuid;
			}
		}

		private Guid mGuid;

		private void btnAccept_Click(object sender, System.EventArgs e)
		{
			if (protocol == null)
			{
				protocol = IMProtocol.FromString(this.cmbProtocol.SelectedItem.ToString());
				AccountManager.Accounts.Add(protocol);
			}
			bool errors = false;
			this.error.Clear();
			if (this.cmbProtocol.SelectedIndex == -1)
			{
				errors = true;
				this.error.SetError(this.cmbProtocol, "Invalid Selection");
			}
			if (this.txtUsername.Text.Length == 0 && protocol.RequiresUsername)
			{
				errors = true;
				this.error.SetError(this.txtUsername, "Need to enter a username");
			}
			if (this.txtPassword.Text.Length == 0 && protocol.RequiresPassword)
			{
				errors = true;
				this.error.SetError(this.txtPassword, "Need to enter a password");
			}

			if (!errors)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				if (protocol != null)
				{
					protocol.Username = txtUsername.Text;
					protocol.Password = txtPassword.Text;
					protocol.Server = txtServer.Text;
					protocol.Guid = Guid.NewGuid();

					AccountManager.ConnectAll();
				}
				this.Close();
			}
		}
		private void frmNewAccount_Load(object sender, EventArgs e)
		{
			if (System.Environment.OSVersion.Version.Major > 6)
			{
				//MARGINS margins = {-1};
				//DwmExtendFrameIntoClientArea((HWND)this->Handle.ToPointer(), &margins);
			}

			txtServer.Text = IMProtocol.FromString(cmbProtocol.SelectedItem.ToString()).GetServerString(txtUsername.Text);

			//CREDUI_INFO uiInfo;
			//uiInfo.pszCaptionText = L"Test";

			//ULONG authPkg;
			//LPVOID authBuf;

			//CredUIPromptForCredentials(uiInfo, ERROR_SUCCESS, &authPkg, NULL, sizeof(ULONG), &)
		}
		private void scrollTimer_Tick(object sender, EventArgs e)
		{
			int speed = 5;
			int minHeight = 165;
			int maxHeight = 200;
			if ((string)scrollTimer.Tag == "down")
			{
				if (this.Height <= (maxHeight - speed))
				{
					this.Height = this.Height + speed;
				} else if (this.Height < (maxHeight))	{
					this.Height = maxHeight;
				} else {
					scrollTimer.Tag = "up";
					btnDetails.Text = "Less";
					scrollTimer.Stop();
					txtServer.Visible = true;
					lblServer.Visible = true;
				}
			} else {
				if (this.Height > (minHeight + speed))
				{
					txtServer.Visible = false;
					lblServer.Visible = false;
					this.Height = this.Height - speed;
				} else if (this.Height > (minHeight)) {
					this.Height = minHeight;
				} else {
					scrollTimer.Tag = "down";
					btnDetails.Text = "More";
					scrollTimer.Stop();
				}
			}
		}
		private void btnDetails_Click(object sender, System.EventArgs e)
		{
			scrollTimer.Start();
		}
		private void frmNewAccount_Resize(object sender, System.EventArgs e)
		{
			btnDetails.Location = new System.Drawing.Point(btnDetails.Location.X, this.Height - 62);
			btnAccept.Location = new System.Drawing.Point(btnAccept.Location.X, this.Height - 62);
			btnCancel.Location = new System.Drawing.Point(btnCancel.Location.X, this.Height - 62);
		}
		private void txtUsername_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			txtServer.Text = IMProtocol.FromString(cmbProtocol.SelectedItem.ToString()).GetServerString(txtUsername.Text);
		}
		private void cmbProtocol_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cmbProtocol.SelectedItem.ToString() == "Yahoo")
			{
				txtServer.Text = "chatserver.yahoo.com";
				//this->txtUsername->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &frmNewAccount::txtUsername_KeyPress);
			}
			else
			{
				txtServer.Text = "";
			}
		}
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}