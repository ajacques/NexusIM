using System.ComponentModel;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmSysTrayPopup
	{
		private ListView listView1;
		private LinkLabel linkLabel1;
		private ComboBox cmbStatus;
		private PictureBox gradient;

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
			this.listView1 = new System.Windows.Forms.ListView();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.cmbStatus = new System.Windows.Forms.ComboBox();
			this.gradient = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.gradient)).BeginInit();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.Location = new System.Drawing.Point(12, 39);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(197, 120);
			this.listView1.TabIndex = 2;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.List;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(251)))));
			this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
			this.linkLabel1.Location = new System.Drawing.Point(51, 175);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(131, 15);
			this.linkLabel1.TabIndex = 3;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Open Account Window";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// cmbStatus
			// 
			this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStatus.FormattingEnabled = true;
			this.cmbStatus.Items.AddRange(new object[] {
            "Available",
            "Away",
            "Invisible",
            "Offline"});
			this.cmbStatus.Location = new System.Drawing.Point(50, 12);
			this.cmbStatus.Name = "cmbStatus";
			this.cmbStatus.Size = new System.Drawing.Size(121, 21);
			this.cmbStatus.TabIndex = 1;
			// 
			// gradient
			// 
			this.gradient.Location = new System.Drawing.Point(0, 165);
			this.gradient.Name = "gradient";
			this.gradient.Size = new System.Drawing.Size(222, 32);
			this.gradient.TabIndex = 4;
			this.gradient.TabStop = false;
			this.gradient.Paint += new System.Windows.Forms.PaintEventHandler(this.gradient_Paint);
			// 
			// frmSysTrayPopup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(221, 197);
			this.ControlBox = false;
			this.Controls.Add(this.cmbStatus);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.gradient);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSysTrayPopup";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Deactivate += new System.EventHandler(this.frmSysTrayPopup_Deactivate);
			this.Load += new System.EventHandler(this.frmSysTrayPopup_Load);
			((System.ComponentModel.ISupportInitialize)(this.gradient)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}