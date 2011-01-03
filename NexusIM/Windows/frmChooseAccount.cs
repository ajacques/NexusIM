using System;
using System.Linq;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	public partial class frmChooseAccount : Form
	{
		public frmChooseAccount()
		{
			InitializeComponent();
		}

		public class ChooseAccountEventArgs : EventArgs
		{
			public ChooseAccountEventArgs(IMProtocol p)
			{
				mProtocol = p;
			}
			public IMProtocol Protocol
			{
				get {
					return mProtocol;
				}
				set {
					mProtocol = value;
				}
			}
			private IMProtocol mProtocol = null;
		}

		public string ProtocolTypeFilter
		{
			get {
				return mPTypeFilter;
			}
			set {
				mPTypeFilter = value;
			}
		}
		public Nullable<IMProtocolStatus> ProtocolStatusFilter
		{
			get {
				return mPSFilter;
			}
			set {
				mPSFilter = value;
			}
		}
		public event EventHandler<ChooseAccountEventArgs> OnChooseAccount;

		private Nullable<IMProtocolStatus> mPSFilter;
		private string mPTypeFilter = "";

		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x0084;

			switch (m.Msg)
			{
				case WM_NCHITTEST:
					{
						return;
					}
			}

			base.WndProc(ref m);
		}

		private void frmChooseAccount_Shown(object sender, EventArgs e)
		{
			var results = from IMProtocol p in AccountManager.Accounts select new { p };

			if (!String.IsNullOrEmpty(mPTypeFilter))
				results = from p in results where p.p.Protocol.ToLower() == mPTypeFilter select new { p.p };

			if (mPSFilter.HasValue)
				results = from p in results where p.p.ProtocolStatus == mPSFilter.Value select new { p.p };

			foreach (var protocol in results)
			{
				cmbAccount.Items.Add(protocol.p.ToString());
			}

			if (cmbAccount.Items.Count > 0)
				cmbAccount.SelectedIndex = 0;
			else
				this.Close();
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			var result = (from IMProtocol p in AccountManager.Accounts where p.ToString() == cmbAccount.SelectedItem.ToString() select new { p }).FirstOrDefault();

			this.Close();

			if (OnChooseAccount != null)
				OnChooseAccount(this, new ChooseAccountEventArgs(result.p));
		}
	}
}