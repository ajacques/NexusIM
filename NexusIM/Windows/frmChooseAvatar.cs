using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NexusIM.Windows
{
	internal partial class frmChooseAvatar : Form
	{
		public frmChooseAvatar()
		{
			InitializeComponent();
		}

		private void frmChooseAvatar_Load(object sender, EventArgs e)
		{
			string usertemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
			string imagepath = Path.Combine(usertemp, Environment.UserName + ".bmp");
			string domainimgpath = Path.Combine(usertemp, Environment.UserDomainName + "+" + Environment.UserName + ".bmp");

			if (File.Exists(imagepath))
				ShowWindowAvatar(imagepath);
			else if (File.Exists(domainimgpath))
				ShowWindowAvatar(domainimgpath);
		}

		private void ShowWindowAvatar(string path)
		{
			picWindows.Image = Image.FromFile(path);
		}

		private void frmChooseAvatar_Paint(object sender, PaintEventArgs e)
		{
			LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 200), SystemColors.Control, Color.LightBlue);
			e.Graphics.FillRectangle(brush, new Rectangle(0, 200, e.ClipRectangle.Width, e.ClipRectangle.Height));
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

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			chooseImageDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			chooseImageDialog.ShowDialog();
		}
	}
}