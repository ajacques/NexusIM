using System;
using System.Windows.Forms;

namespace NexusIM.Windows
{
	public partial class frmContactGroupOptions : Form
	{
		public frmContactGroupOptions()
		{
			InitializeComponent();
		}

		private void frmContactGroupOptions_Load(object sender, EventArgs e)
		{

		}

		private void btnAddContact_Click(object sender, EventArgs e)
		{
			frmSelectContact win = new frmSelectContact();
			if (win.ShowDialog() == DialogResult.OK)
			{

			}
		}
	}
}
