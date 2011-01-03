using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{

	/// <summary>
	/// Summary for frmAccounts
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class frmAccounts : Form
	{
		public frmAccounts()
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

		public void btnNew_Click(object sender, EventArgs e)
		{
			frmNewAccount win = new frmNewAccount();
			if (win.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ListViewItem item = new ListViewItem();
				item.Text = win.txtUsername.Text;
				item.Checked = true;

				UpdateList();

				/*IMProtocol proto = IMProtocol.FromString(win.cmbProtocol.SelectedItem.ToString());

				item.Tag = (object)(proto);

				proto.Username = win.txtUsername.Text;
				proto.Password = win.txtPassword.Text;
				proto.Enabled = true;
				proto.SavePassword = win.chkSavePassword.Checked;

				this.lstAccounts.Items.Add(item);*/
			}
		}
		public void btnAccept_Click(object sender, EventArgs e)
		{
			IEnumerator proto = lstAccounts.Items.GetEnumerator();
			while (proto.MoveNext())
			{
				ListViewItem item = (ListViewItem)(proto.Current);
				IMProtocol protocol = (IMProtocol)(item.Tag);
				if (!AccountManager.Accounts.Contains(protocol))
				{
					AccountManager.Accounts.Add(protocol);
				}
			}
			IMSettings.SaveAccounts();
			AccountManager.ConnectAll();
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			frmMain.Instance.UpdateContactListStatus();
			this.Close();
		}
		public void lstAccounts_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			IMProtocol protocol = ((IMProtocol)e.Item.Tag);
			protocol.Enabled = e.Item.Checked;
		}
		public void frmAccounts_Load(object sender, EventArgs e)
		{
			//Settings.LoadAccounts();
			UpdateList();
			this.lstAccounts.ItemChecked += new ItemCheckedEventHandler(this.lstAccounts_ItemChecked);
		}
		private void UpdateList()
		{
			foreach (IMProtocol protocol in AccountManager.Accounts)
			{
				ListViewItem item = new ListViewItem();
				if (protocol.Username != "")
					item.Text = protocol.Username;
				else
					item.Text = protocol.Protocol;
				item.Tag = (object)(protocol);
				item.Checked = protocol.Enabled;
				this.lstAccounts.Items.Add(item);
				if (protocol.ProtocolStatus == IMProtocolStatus.ERROR)
				{
					System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
					System.IO.Stream file = thisExe.GetManifestResourceStream("NexusIM.Resources.error.png");

					lstAccounts.statusImages.Add(item, Image.FromStream(file));
				}
			}
			if (lstAccounts.Items.Count >= 1)
			{
				btnDelete.Enabled = true;
				btnEdit.Enabled = true;
			}
		}
		public void lstAccounts_DoubleClick(object sender, EventArgs e)
		{
			if (this.lstAccounts.SelectedIndices.Count > 0)
			{

			}
		}
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (lstAccounts.SelectedItems.Count >= 1)
			{
				AccountManager.Accounts.Remove(((IMProtocol)lstAccounts.SelectedItems[0].Tag));
				lstAccounts.Items.Remove(lstAccounts.SelectedItems[0]);
			}
			lstAccounts.updateChecks();
		}
		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (lstAccounts.SelectedIndices.Count == 0)
				return;

			frmNewAccount win = new frmNewAccount();
			IMProtocol protocol = AccountManager.GetAccountByUsername(lstAccounts.SelectedItems[0].Text);
			win.cmbProtocol.Enabled = false;
			win.txtUsername.Text = protocol.Username;
			win.txtPassword.Text = protocol.Password;
			win.cmbProtocol.SelectedItem = protocol.Protocol;

			if (win.ShowDialog() == DialogResult.OK)
			{
				protocol.Username = win.txtUsername.Text;
				protocol.Password = win.txtPassword.Text;
			}
		}
		private void lstAccounts_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstAccounts.SelectedIndices.Count >= 1)
			{
				btnEdit.Enabled = true;
				btnDelete.Enabled = true;
			} else {
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
			}
		}
	}
}