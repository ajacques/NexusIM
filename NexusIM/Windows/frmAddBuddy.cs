using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{

	/// <summary>
	/// Summary for frmAddBuddy
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class frmAddBuddy : Form
	{
		public frmAddBuddy()
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}
		private Dictionary<int, IMProtocol> protocolDictionary = new Dictionary<int, IMProtocol>();
		protected ComboBox cmbAccount;
		protected Label lblAccount;
		protected TextBox txtUsername;
		protected Label lblUsername;
		protected Label lblNickname;
		protected TextBox txtNickname;
		protected Label lblGroup;
		protected ComboBox cmbGroup;
		protected Button btnCancel;
		protected Button btnAccept;
		protected ErrorProvider error;
		private Label lblIntroduction;
		private TextBox txtReason;
		private Button btnDetails;
		private Timer scrollTimer;
		private IContainer components;
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		protected void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.cmbAccount = new System.Windows.Forms.ComboBox();
			this.lblAccount = new System.Windows.Forms.Label();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.lblUsername = new System.Windows.Forms.Label();
			this.lblNickname = new System.Windows.Forms.Label();
			this.txtNickname = new System.Windows.Forms.TextBox();
			this.lblGroup = new System.Windows.Forms.Label();
			this.cmbGroup = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnAccept = new System.Windows.Forms.Button();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			this.txtReason = new System.Windows.Forms.TextBox();
			this.lblIntroduction = new System.Windows.Forms.Label();
			this.btnDetails = new System.Windows.Forms.Button();
			this.scrollTimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.SuspendLayout();
			// 
			// cmbAccount
			// 
			this.cmbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbAccount.FormattingEnabled = true;
			this.cmbAccount.Location = new System.Drawing.Point(87, 12);
			this.cmbAccount.Name = "cmbAccount";
			this.cmbAccount.Size = new System.Drawing.Size(144, 21);
			this.cmbAccount.TabIndex = 0;
			this.cmbAccount.SelectedValueChanged += new System.EventHandler(this.cmbAccount_SelectedValueChanged);
			// 
			// lblAccount
			// 
			this.lblAccount.AutoSize = true;
			this.lblAccount.Location = new System.Drawing.Point(31, 15);
			this.lblAccount.Name = "lblAccount";
			this.lblAccount.Size = new System.Drawing.Size(50, 13);
			this.lblAccount.TabIndex = 1;
			this.lblAccount.Text = "Account:";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(87, 40);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(144, 20);
			this.txtUsername.TabIndex = 3;
			// 
			// lblUsername
			// 
			this.lblUsername.AutoSize = true;
			this.lblUsername.Location = new System.Drawing.Point(23, 43);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(58, 13);
			this.lblUsername.TabIndex = 4;
			this.lblUsername.Text = "Username:";
			// 
			// lblNickname
			// 
			this.lblNickname.AutoSize = true;
			this.lblNickname.Location = new System.Drawing.Point(23, 70);
			this.lblNickname.Name = "lblNickname";
			this.lblNickname.Size = new System.Drawing.Size(58, 13);
			this.lblNickname.TabIndex = 5;
			this.lblNickname.Text = "Nickname:";
			this.lblNickname.Visible = false;
			// 
			// txtNickname
			// 
			this.txtNickname.Location = new System.Drawing.Point(88, 67);
			this.txtNickname.Name = "txtNickname";
			this.txtNickname.Size = new System.Drawing.Size(144, 20);
			this.txtNickname.TabIndex = 6;
			this.txtNickname.Visible = false;
			// 
			// lblGroup
			// 
			this.lblGroup.AutoSize = true;
			this.lblGroup.Location = new System.Drawing.Point(42, 96);
			this.lblGroup.Name = "lblGroup";
			this.lblGroup.Size = new System.Drawing.Size(39, 13);
			this.lblGroup.TabIndex = 7;
			this.lblGroup.Text = "Group:";
			this.lblGroup.Visible = false;
			// 
			// cmbGroup
			// 
			this.cmbGroup.FormattingEnabled = true;
			this.cmbGroup.Location = new System.Drawing.Point(88, 94);
			this.cmbGroup.Name = "cmbGroup";
			this.cmbGroup.Size = new System.Drawing.Size(144, 21);
			this.cmbGroup.TabIndex = 8;
			this.cmbGroup.Text = "Friends";
			this.cmbGroup.Visible = false;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(198, 145);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnAccept
			// 
			this.btnAccept.Location = new System.Drawing.Point(117, 145);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 10;
			this.btnAccept.Text = "Add Buddy";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// txtReason
			// 
			this.txtReason.Location = new System.Drawing.Point(88, 121);
			this.txtReason.Name = "txtReason";
			this.txtReason.Size = new System.Drawing.Size(144, 20);
			this.txtReason.TabIndex = 11;
			this.txtReason.Visible = false;
			// 
			// lblIntroduction
			// 
			this.lblIntroduction.AutoSize = true;
			this.lblIntroduction.Location = new System.Drawing.Point(4, 124);
			this.lblIntroduction.Name = "lblIntroduction";
			this.lblIntroduction.Size = new System.Drawing.Size(77, 13);
			this.lblIntroduction.TabIndex = 12;
			this.lblIntroduction.Text = "Intro Message:";
			this.lblIntroduction.Visible = false;
			// 
			// btnDetails
			// 
			this.btnDetails.Location = new System.Drawing.Point(6, 145);
			this.btnDetails.Name = "btnDetails";
			this.btnDetails.Size = new System.Drawing.Size(75, 23);
			this.btnDetails.TabIndex = 13;
			this.btnDetails.Text = "More";
			this.btnDetails.UseVisualStyleBackColor = true;
			this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
			// 
			// scrollTimer
			// 
			this.scrollTimer.Interval = 1;
			this.scrollTimer.Tag = "down";
			this.scrollTimer.Tick += new System.EventHandler(this.scrollTimer_Tick);
			// 
			// frmAddBuddy
			// 
			this.AcceptButton = this.btnAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 180);
			this.Controls.Add(this.btnDetails);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblIntroduction);
			this.Controls.Add(this.txtReason);
			this.Controls.Add(this.cmbGroup);
			this.Controls.Add(this.lblGroup);
			this.Controls.Add(this.txtNickname);
			this.Controls.Add(this.lblNickname);
			this.Controls.Add(this.lblUsername);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.lblAccount);
			this.Controls.Add(this.cmbAccount);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAddBuddy";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Buddy";
			this.Load += new System.EventHandler(this.frmAddBuddy_Load);
			this.Resize += new System.EventHandler(this.frmAddBuddy_Resize);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		protected void frmAddBuddy_Load(object sender, System.EventArgs e)
		{
			var protocols = from IMProtocol p in AccountManager.Accounts where p.Enabled == true && p.ProtocolStatus == IMProtocolStatus.ONLINE select new { p };

			foreach (var protocol in protocols)
			{
				string name = protocol.p.Protocol;
				if (protocol.p.Username != "")
					name += " - " + protocol.p.Username;
			}

			System.Collections.IEnumerator protoEnum = AccountManager.Accounts.ToArray().GetEnumerator();
			int i = 0;
			while (protoEnum.MoveNext())
			{
				IMProtocol prot = (IMProtocol)(protoEnum.Current);
				cmbAccount.Items.Add(prot.Protocol + " - " + prot.Username);
				protocolDictionary[i] = prot;
				i++;
			}
			cmbAccount.SelectedIndex = 0;

			IEnumerator groups = frmMain.Instance.ContactList.Groups.GetEnumerator();
			while (groups.MoveNext())
			{
				ListViewGroup group = (ListViewGroup)groups.Current;
				cmbGroup.Items.Add(group.Name);
			}

			this.Size = new Size(290, 135);
		}
		protected void btnAccept_Click(object sender, System.EventArgs e)
		{
			bool isError = false;
			// Validation Checks
			if (cmbAccount.SelectedIndex == -1)
			{
				isError = true;
				error.SetError(this.cmbAccount, "Please choose an account");
			}
			if (this.txtUsername.Text.Length == 0)
			{
				isError = true;
				error.SetError(this.txtUsername, "Please enter a username");
			}

			if (!isError)
			{
				protocolDictionary[cmbAccount.SelectedIndex].AddFriend(txtUsername.Text, txtNickname.Text, cmbGroup.Text, txtReason.Text);
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.Close();
			}
		}
		private void frmAddBuddy_Resize(object sender, EventArgs e)
		{
			btnDetails.Location = new System.Drawing.Point(btnDetails.Location.X, this.Height - 62);
			btnAccept.Location = new System.Drawing.Point(btnAccept.Location.X, this.Height - 62);
			btnCancel.Location = new System.Drawing.Point(btnCancel.Location.X, this.Height - 62);
		}
		private void btnDetails_Click(object sender, EventArgs e)
		{
			scrollTimer.Start();
		}
		private void scrollTimer_Tick(object sender, EventArgs e)
		{
			int speed = 5;
			int minHeight = 135;
			int maxHeight = 205;
			if ((string)scrollTimer.Tag == "down")
			{
				if (this.Height <= (maxHeight - speed))
				{
					this.Height = this.Height + speed;
				}
				else if (this.Height < (maxHeight))
				{
					this.Height = maxHeight;
				} else {
					scrollTimer.Tag = "up";
					btnDetails.Text = "Less";
					scrollTimer.Stop();
					txtNickname.Visible = true;
					txtReason.Visible = true;
					cmbGroup.Visible = true;
					lblIntroduction.Visible = true;
					lblNickname.Visible = true;
					lblGroup.Visible = true;
				}
			} else {
				if (this.Height > (minHeight + speed))
				{
					txtNickname.Visible = false;
					txtReason.Visible = false;
					cmbGroup.Visible = false;
					lblIntroduction.Visible = false;
					lblNickname.Visible = false;
					lblGroup.Visible = false;
					this.Height = this.Height - speed;
				}
				else if (this.Height > (minHeight))
				{
					this.Height = minHeight;
				}
				else
				{
					scrollTimer.Tag = "down";
					btnDetails.Text = "More";
					scrollTimer.Stop();
				}
			}
				
		}
		private void cmbAccount_SelectedValueChanged(object sender, EventArgs e)
		{
			if (protocolDictionary[cmbAccount.SelectedIndex].SupportsIntroMessage)
			{
				txtReason.Enabled = true;
				lblIntroduction.Enabled = true;
			} else {
				txtReason.Enabled = false;
				lblIntroduction.Enabled = false;
			}
		}
	}
}