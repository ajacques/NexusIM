using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	public partial class frmSysTrayPopup : Form
	{
		public frmSysTrayPopup()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			listView1.ItemChecked -= new ItemCheckedEventHandler(listView1_ItemChecked);
			cmbStatus.SelectedIndexChanged -= new EventHandler(this.cmbStatus_SelectedIndexChanged);

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool isChanged = false;
		private int lastSid = 0;

		private void frmSysTrayPopup_Load(object sender, EventArgs e)
		{
			if (Win32.IsWinVistaAndUp() && Win32.DwmIsCompositionEnabled()) // If the current OS was DWM
				this.Location = new Point(Control.MousePosition.X - (this.Width / 2), Screen.PrimaryScreen.WorkingArea.Bottom - (this.Height + 8));
			else
				this.Location = new Point(Control.MousePosition.X - (this.Width / 2), Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);

			this.Activate();
			foreach (IMProtocol protocol in AccountManager.Accounts)
			{
				ListViewItem item = new ListViewItem();
				item.Text = protocol.Protocol;
				if (protocol.Username != "")
					item.Text += " - " + protocol.Username;
				item.ImageIndex = 0;
				item.Tag = (object)(protocol);
				item.Checked = protocol.Enabled;
				listView1.Items.Add(item);
			}
			var stritem = (from string t in cmbStatus.Items where t.ToLower() == AccountManager.Status.ToString().ToLower() select new { index = cmbStatus.Items.IndexOf(t) }).FirstOrDefault();
			cmbStatus.SelectedIndex = stritem.index;
			cmbStatus.SelectedIndexChanged += new EventHandler(this.cmbStatus_SelectedIndexChanged);

			listView1.ItemChecked += new ItemCheckedEventHandler(listView1_ItemChecked);
		}
		private void frmSysTrayPopup_Deactivate(object sender, EventArgs e)
		{
			if (isChanged)
				IMSettings.SaveAccounts();
			this.Close();
		}
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmAccounts win = new frmAccounts();
			win.Show();
		}
		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			IMProtocol protocol = (IMProtocol)(e.Item.Tag);
			if (e.Item.Checked)
			{
				protocol.Enabled = true;
				protocol.Status = IMStatus.AVAILABLE;
			} else {
				protocol.Status = IMStatus.OFFLINE;
				protocol.Enabled = false;
			}
			isChanged = true;
		}
		private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			var stritem = (from t in Enum.GetValues(typeof(IMStatus)).Cast<IMStatus>() where t.ToString().ToLower() == cmbStatus.SelectedItem.ToString() select t).FirstOrDefault();
			cmbStatus.Tag = true;

			// New Update!
			lastSid = AccountManager.SetStatus(stritem);

			//AccountManager.Status = stritem;
		}
		private void gradient_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = new Rectangle(0, 0, gradient.Width, 3);
			Rectangle rect2 = new Rectangle(0, 3, gradient.Width, gradient.Height - 3);

			Brush gradientbrush = new LinearGradientBrush(rect, Color.FromArgb(204, 217, 234), Color.FromArgb(241, 245, 251), LinearGradientMode.Vertical);
			Brush solidbrush = new SolidBrush(Color.FromArgb(241, 245, 251));

			e.Graphics.FillRectangle(gradientbrush, rect);
			e.Graphics.FillRectangle(solidbrush, rect2);
		}
		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x0084;

			switch (m.Msg)
			{
				case WM_NCHITTEST: // Edge Cursor
					{
						return;
					}
			}

			base.WndProc(ref m);
		}
	}
}