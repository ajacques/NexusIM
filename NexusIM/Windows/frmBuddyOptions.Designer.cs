namespace InstantMessage
{
	partial class frmBuddyOptions
	{
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
			this.numTimezone = new System.Windows.Forms.NumericUpDown();
			this.lblTimezone = new System.Windows.Forms.Label();
			this.chkCorrectTimezone = new System.Windows.Forms.CheckBox();
			this.lblAutocorrectTimezone = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numTimezone)).BeginInit();
			this.SuspendLayout();
			// 
			// numTimezone
			// 
			this.numTimezone.Location = new System.Drawing.Point(127, 71);
			this.numTimezone.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
			this.numTimezone.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            -2147483648});
			this.numTimezone.Name = "numTimezone";
			this.numTimezone.Size = new System.Drawing.Size(46, 20);
			this.numTimezone.TabIndex = 0;
			this.numTimezone.Value = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
			// 
			// lblTimezone
			// 
			this.lblTimezone.AutoSize = true;
			this.lblTimezone.Location = new System.Drawing.Point(37, 73);
			this.lblTimezone.Name = "lblTimezone";
			this.lblTimezone.Size = new System.Drawing.Size(84, 13);
			this.lblTimezone.TabIndex = 1;
			this.lblTimezone.Text = "Timezone: UTC-";
			// 
			// chkCorrectTimezone
			// 
			this.chkCorrectTimezone.AutoSize = true;
			this.chkCorrectTimezone.Location = new System.Drawing.Point(12, 12);
			this.chkCorrectTimezone.Name = "chkCorrectTimezone";
			this.chkCorrectTimezone.Size = new System.Drawing.Size(138, 17);
			this.chkCorrectTimezone.TabIndex = 2;
			this.chkCorrectTimezone.Text = "Auto-correct Timezones";
			this.chkCorrectTimezone.UseVisualStyleBackColor = true;
			this.chkCorrectTimezone.CheckedChanged += new System.EventHandler(this.chkCorrectTimezone_CheckedChanged);
			// 
			// lblAutocorrectTimezone
			// 
			this.lblAutocorrectTimezone.Location = new System.Drawing.Point(28, 32);
			this.lblAutocorrectTimezone.Name = "lblAutocorrectTimezone";
			this.lblAutocorrectTimezone.Size = new System.Drawing.Size(235, 29);
			this.lblAutocorrectTimezone.TabIndex = 3;
			this.lblAutocorrectTimezone.Text = "Automatically attempt to correct timezone differences";
			// 
			// frmBuddyOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.lblAutocorrectTimezone);
			this.Controls.Add(this.chkCorrectTimezone);
			this.Controls.Add(this.lblTimezone);
			this.Controls.Add(this.numTimezone);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmBuddyOptions";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.numTimezone)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numTimezone;
		private System.Windows.Forms.Label lblTimezone;
		private System.Windows.Forms.CheckBox chkCorrectTimezone;
		private System.Windows.Forms.Label lblAutocorrectTimezone;

	}
}