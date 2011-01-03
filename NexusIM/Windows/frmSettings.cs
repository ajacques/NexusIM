using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public class frmSettings : Form
	{
		public frmSettings()
		{
			InitializeComponent();
		}

		private TabControl tabControl;
		private TabPage tabChat;
		private Button btnCancel;
		private Button btnAccept;
		private TabPage tabProtocol;
		private ComboBox cmbProtocol;
		private Label lblTimeToIdle;
		private NumericUpDown numTimeToIdle;
		private CheckBox chkIdle;
		private CheckBox chkPsychic;
		private Label label1;
		private CheckBox chkTimezoneRecalculate;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabProtocol = new System.Windows.Forms.TabPage();
			this.cmbProtocol = new System.Windows.Forms.ComboBox();
			this.tabChat = new System.Windows.Forms.TabPage();
			this.chkPsychic = new System.Windows.Forms.CheckBox();
			this.lblTimeToIdle = new System.Windows.Forms.Label();
			this.numTimeToIdle = new System.Windows.Forms.NumericUpDown();
			this.chkIdle = new System.Windows.Forms.CheckBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnAccept = new System.Windows.Forms.Button();
			this.chkTimezoneRecalculate = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.tabProtocol.SuspendLayout();
			this.tabChat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTimeToIdle)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabProtocol);
			this.tabControl.Controls.Add(this.tabChat);
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(284, 218);
			this.tabControl.TabIndex = 0;
			// 
			// tabProtocol
			// 
			this.tabProtocol.Controls.Add(this.cmbProtocol);
			this.tabProtocol.Location = new System.Drawing.Point(4, 22);
			this.tabProtocol.Name = "tabProtocol";
			this.tabProtocol.Padding = new System.Windows.Forms.Padding(3);
			this.tabProtocol.Size = new System.Drawing.Size(276, 192);
			this.tabProtocol.TabIndex = 1;
			this.tabProtocol.Text = "Protocol";
			this.tabProtocol.UseVisualStyleBackColor = true;
			// 
			// cmbProtocol
			// 
			this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProtocol.FormattingEnabled = true;
			this.cmbProtocol.Location = new System.Drawing.Point(78, 6);
			this.cmbProtocol.Name = "cmbProtocol";
			this.cmbProtocol.Size = new System.Drawing.Size(121, 21);
			this.cmbProtocol.TabIndex = 0;
			// 
			// tabChat
			// 
			this.tabChat.AutoScroll = true;
			this.tabChat.Controls.Add(this.label1);
			this.tabChat.Controls.Add(this.chkTimezoneRecalculate);
			this.tabChat.Controls.Add(this.chkPsychic);
			this.tabChat.Controls.Add(this.lblTimeToIdle);
			this.tabChat.Controls.Add(this.numTimeToIdle);
			this.tabChat.Controls.Add(this.chkIdle);
			this.tabChat.Location = new System.Drawing.Point(4, 22);
			this.tabChat.Name = "tabChat";
			this.tabChat.Padding = new System.Windows.Forms.Padding(3);
			this.tabChat.Size = new System.Drawing.Size(276, 192);
			this.tabChat.TabIndex = 0;
			this.tabChat.Text = "Chat";
			this.tabChat.UseVisualStyleBackColor = true;
			// 
			// chkPsychic
			// 
			this.chkPsychic.AutoSize = true;
			this.chkPsychic.Location = new System.Drawing.Point(9, 69);
			this.chkPsychic.Name = "chkPsychic";
			this.chkPsychic.Size = new System.Drawing.Size(225, 17);
			this.chkPsychic.TabIndex = 2;
			this.chkPsychic.Text = "Open Chat Window on Typing Notification";
			this.chkPsychic.UseVisualStyleBackColor = true;
			// 
			// lblTimeToIdle
			// 
			this.lblTimeToIdle.AutoSize = true;
			this.lblTimeToIdle.Enabled = false;
			this.lblTimeToIdle.Location = new System.Drawing.Point(69, 32);
			this.lblTimeToIdle.Name = "lblTimeToIdle";
			this.lblTimeToIdle.Size = new System.Drawing.Size(84, 13);
			this.lblTimeToIdle.TabIndex = 0;
			this.lblTimeToIdle.Text = "minutes until idle";
			// 
			// numTimeToIdle
			// 
			this.numTimeToIdle.Enabled = false;
			this.numTimeToIdle.Location = new System.Drawing.Point(26, 30);
			this.numTimeToIdle.Name = "numTimeToIdle";
			this.numTimeToIdle.Size = new System.Drawing.Size(37, 20);
			this.numTimeToIdle.TabIndex = 1;
			this.numTimeToIdle.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// chkIdle
			// 
			this.chkIdle.AutoSize = true;
			this.chkIdle.Checked = true;
			this.chkIdle.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkIdle.Location = new System.Drawing.Point(9, 7);
			this.chkIdle.Name = "chkIdle";
			this.chkIdle.Size = new System.Drawing.Size(104, 17);
			this.chkIdle.TabIndex = 0;
			this.chkIdle.Text = "Report Idle Time";
			this.chkIdle.UseVisualStyleBackColor = true;
			this.chkIdle.CheckedChanged += new System.EventHandler(this.chkIdle_CheckedChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(197, 227);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnAccept
			// 
			this.btnAccept.Location = new System.Drawing.Point(116, 227);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 1;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// chkTimezoneRecalculate
			// 
			this.chkTimezoneRecalculate.AutoSize = true;
			this.chkTimezoneRecalculate.Location = new System.Drawing.Point(9, 92);
			this.chkTimezoneRecalculate.Name = "chkTimezoneRecalculate";
			this.chkTimezoneRecalculate.Size = new System.Drawing.Size(138, 17);
			this.chkTimezoneRecalculate.TabIndex = 3;
			this.chkTimezoneRecalculate.Text = "Timezone Recalcuation";
			this.chkTimezoneRecalculate.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(242, 32);
			this.label1.TabIndex = 4;
			this.label1.Text = "Automatically attempt to adjust different timezones to the local timezone";
			// 
			// frmSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSettings";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Settings";
			this.Load += new System.EventHandler(this.frmSettings_Load);
			this.tabControl.ResumeLayout(false);
			this.tabProtocol.ResumeLayout(false);
			this.tabChat.ResumeLayout(false);
			this.tabChat.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTimeToIdle)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private void frmSettings_Load(object sender, EventArgs e)
		{
			chkIdle.Checked = Convert.ToBoolean(IMSettings.GetCustomSetting("enableidle", "True"));
			numTimeToIdle.Value = Convert.ToInt32(IMSettings.GetCustomSetting("timetoidle", "5"));
			chkPsychic.Checked = Convert.ToBoolean(IMSettings.GetCustomSetting("psychicchat", "False"));
			chkTimezoneRecalculate.Checked = Convert.ToBoolean(IMSettings.GetCustomSetting("timezonerecalc", "False"));

			if (chkIdle.Checked)
			{
				numTimeToIdle.Enabled = true;
				lblTimeToIdle.Enabled = true;
			}
		}
		private void chkIdle_CheckedChanged(object sender, EventArgs e)
		{
			if (chkIdle.Checked)
			{
				numTimeToIdle.Enabled = true;
				lblTimeToIdle.Enabled = true;
			} else {
				numTimeToIdle.Enabled = false;
				lblTimeToIdle.Enabled = false;
			}
		}
		private void btnAccept_Click(object sender, EventArgs e)
		{
			IMSettings.SetCustomSetting("enableidle", Convert.ToString(chkIdle.Checked));
			IMSettings.SetCustomSetting("timetoidle", Convert.ToString(numTimeToIdle.Value));
			IMSettings.SetCustomSetting("psychicchat", Convert.ToString(chkPsychic.Checked));
			IMSettings.SetCustomSetting("timezonerecalc", Convert.ToString(chkTimezoneRecalculate.Checked));
		}
	}
}
