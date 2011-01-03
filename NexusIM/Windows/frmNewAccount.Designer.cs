using System.ComponentModel;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmNewAccount
	{
		public IMProtocol protocol;
		public TextBox txtUsername;
		public TextBox txtPassword;
		public ComboBox cmbProtocol;
		private Button btnCancel;
		private Button btnAccept;
		private Label label1;
		private Label label2;
		private Label label3;
		private ErrorProvider error;
		private Button btnDetails;
		private Timer scrollTimer;
		private TextBox txtServer;
		private Label lblServer;
		public CheckBox chkSavePassword;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnAccept = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbProtocol = new System.Windows.Forms.ComboBox();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnDetails = new System.Windows.Forms.Button();
			this.scrollTimer = new System.Windows.Forms.Timer(this.components);
			this.lblServer = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.chkSavePassword = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(197, 102);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnAccept
			// 
			this.btnAccept.Location = new System.Drawing.Point(116, 102);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 5;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Protocol:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Username:";
			// 
			// cmbProtocol
			// 
			this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProtocol.FormattingEnabled = true;
			this.cmbProtocol.Items.AddRange(new object[] {
            "Yahoo",
            "AIM",
            "MSN",
            "Jabber",
            "IRC",
            "People Near Me"});
			this.cmbProtocol.Location = new System.Drawing.Point(70, 10);
			this.cmbProtocol.Name = "cmbProtocol";
			this.cmbProtocol.Size = new System.Drawing.Size(121, 21);
			this.cmbProtocol.TabIndex = 1;
			this.cmbProtocol.SelectedIndexChanged += new System.EventHandler(this.cmbProtocol_SelectedIndexChanged);
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(70, 37);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(121, 20);
			this.txtUsername.TabIndex = 2;
			this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUsername_KeyUp);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Password:";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(70, 64);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(121, 20);
			this.txtPassword.TabIndex = 3;
			// 
			// error
			// 
			this.error.BlinkRate = 0;
			this.error.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.error.ContainerControl = this;
			// 
			// btnDetails
			// 
			this.btnDetails.Location = new System.Drawing.Point(13, 102);
			this.btnDetails.Name = "btnDetails";
			this.btnDetails.Size = new System.Drawing.Size(75, 23);
			this.btnDetails.TabIndex = 7;
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
			// lblServer
			// 
			this.lblServer.AutoSize = true;
			this.lblServer.Location = new System.Drawing.Point(29, 93);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(41, 13);
			this.lblServer.TabIndex = 0;
			this.lblServer.Text = "Server:";
			this.lblServer.Visible = false;
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(70, 90);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(121, 20);
			this.txtServer.TabIndex = 4;
			this.txtServer.Visible = false;
			// 
			// chkSavePassword
			// 
			this.chkSavePassword.AutoSize = true;
			this.chkSavePassword.Checked = true;
			this.chkSavePassword.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSavePassword.Location = new System.Drawing.Point(197, 66);
			this.chkSavePassword.Name = "chkSavePassword";
			this.chkSavePassword.Size = new System.Drawing.Size(77, 17);
			this.chkSavePassword.TabIndex = 8;
			this.chkSavePassword.Text = "Remember";
			this.chkSavePassword.UseVisualStyleBackColor = true;
			// 
			// frmNewAccount
			// 
			this.AcceptButton = this.btnAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 137);
			this.Controls.Add(this.chkSavePassword);
			this.Controls.Add(this.btnDetails);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.lblServer);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.cmbProtocol);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmNewAccount";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "New Account";
			this.Load += new System.EventHandler(this.frmNewAccount_Load);
			this.Resize += new System.EventHandler(this.frmNewAccount_Resize);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}