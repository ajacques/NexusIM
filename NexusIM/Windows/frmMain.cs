using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NexusIM.Controls;
using NexusIM.Managers;
using NexusIM.Properties;
using NexusIM.Windows;
using InstantMessage.Events;

namespace InstantMessage
{
	enum frmMainState
	{
		NoAccounts,
		ConnectingToCore,
		Ready
	}
	delegate void GenericEvent();
	/// <summary>
	/// frmMain is the primary form of this application. It gives the user the ability to get to all other windows
	/// </summary>
	partial class frmMain : Form, IDisposable
	{
		public frmMain()
		{
			InitializeComponent();

			mInstance = this;

			FormState = frmMainState.NoAccounts;

			if (onFormLoad != null)
				onFormLoad();
		}

		protected override void Dispose(bool disposing)
		{
			sysTrayIcon.Visible = false;
			AccountManager.onStatusChange -= new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			base.Dispose(disposing);
		}

		public frmMainState FormState
		{
			get {
				return mState;
			}
			set {
				mState = value;

				UpdateState();
			}
		}
		public BuddyList ContactList
		{
			get {
				return listView1;
			}
		}
		public static frmMain Instance
		{
			get {
				return mInstance;
			}
		}

		//  ---- Variables ----
		public static event GenericEvent onFormLoad;
		private static frmMain mInstance; // Singleton
		private int SnapDist = 25;
		private bool isAppCloseInvoke;
		private int lastSid = 0; // Last Status Id
		private frmMainState mState;

		private void UpdateState()
		{
			if (InvokeRequired)
			{
				BeginInvoke(new GenericEvent(UpdateState));
				return;
			}

			pnlLogin.Visible = false;
			pnlCoreLogin.Visible = false;
			pnlReady.Visible = false;

			switch (mState)
			{
				case frmMainState.NoAccounts:
					pnlLogin.Visible = true;
					break;
				case frmMainState.Ready:
					pnlReady.Visible = true;
					break;
				case frmMainState.ConnectingToCore:
					pnlCoreLogin.Visible = true;
					break;
			}
		}

		[Obsolete("Use UpdateState instead", false)]
		public void UpdateContactListStatus()
		{
			return;
			addBuddyToolStripMenuItem.Enabled = true;

			cmbProtocol.SelectedIndex = 0;

			compactListToolStripMenuItem_CheckStateChanged(null, null);

			if (AccountManager.Accounts.Count > 0)
			{
				addBuddyToolStripMenuItem.Enabled = true;
				lblContactListError.Visible = false;
				listView1.Enabled = true;
				listView1.Visible = true;
				lblLink.Visible = false;
				comboBox1.Enabled = true;
				comboBox1.Visible = true;
				cmbStatus.Enabled = true;
				cmbStatus.Visible = true;
				txtSearch.Enabled = true;
				txtSearch.Visible = true;
				listView1.Location = new Point(0, 106);
				pnlLogin.Visible = false;
			} else {
				txtUsername.Visible = true;
				txtPassword.Visible = true;
				addBuddyToolStripMenuItem.Enabled = false;
				listView1.Location = new Point(0, 5);
				//lblContactListError.Text = "You have not added any accounts yet";
				//lblContactListError.Visible = true;
				listView1.Visible = false;
				//lblLink.Visible = true;
				comboBox1.Visible = false;
				cmbStatus.Enabled = false;
				txtSearch.Visible = false;
				cmbStatus.Visible = false;
				lblUsername.Visible = true;
				lblPassword.Visible = true;
				btnDoLogin.Visible = true;
			}
		}

		// Form Event Callbacks
		private void frmMain_Load(object sender, EventArgs e)
		{
			this.comboBox1.SelectedIndex = 0;
			this.comboBox1.SelectedText = AccountManager.Status.ToString();
			AccountManager.onStatusChange += new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			AccountManager.Start();

			UpdateContactListStatus();
			//MARGINS margins = {0, 0, 0, 100};
			//DwmExtendFrameIntoClientArea((HWND)this->Handle, &margins);

			bool compact = Convert.ToBoolean(IMSettings.SettingInterface.GetCustomSetting("compactview", "False"));

			smallListView.Checked = compact;
			listView1.CompactView = compact;

			comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);

			// TODO: Move these to the AccountManager
			SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
			SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
			SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);

			this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2));

			//SpeechManager.Setup();

			listView1.Focus();
		}
		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (SuperTaskbarManager.IsSetup) // Only trigger an update if it's setup
				SuperTaskbarManager.TriggerUpdate();
			else if (!SuperTaskbarManager.AttemptedSetup) // If this system hasn't been setup yet
				SuperTaskbarManager.Setup();

			UpdateContactListStatus();
			if ((this.Location.X + this.Width) > Screen.FromControl(this).Bounds.Width)
				this.Location = new Point(Screen.FromControl(this).Bounds.Width - this.Width, (Screen.FromControl(this).Bounds.Height / 2) - (this.Height / 2));
		}
		private void frmMain_Resize(object sender, EventArgs e)
		{
			System.Drawing.Size mainSize = this.Size;

			// Re-align controls
			comboBox1.Location = new Point(comboBox1.Location.X, 27);
			listView1.Size = new Size(mainSize.Width, mainSize.Height - 100);
			pnlLogin.Location = new Point(pnlLogin.Location.X, (int)(mainSize.Height * 0.3308));

			SnapIt(sender, e);

			if (AccountManager.Accounts.Count == 0)
				this.Size = new Size(217, this.Height);
		}
		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (ContactDetailPopup != null)
				ContactDetailPopup.Close();
			if (!isAppCloseInvoke)
			{
				this.Hide();
				e.Cancel = true;
				this.showBuddyListToolStripMenuItem.Checked = false;
			} else {
				Dispose();
				if (SuperTaskbarManager.IsSetup)
					SuperTaskbarManager.Shutdown();
			}
		}
		private void frmMain_ResizeBegin(object sender, EventArgs e)
		{
			if (AccountManager.Accounts.Count == 0)
				this.Size = new Size(217, this.Height);
			SnapIt(sender, e);
		}
		private void frmMain_ResizeEnd(object sender, EventArgs e)
		{
			if (AccountManager.Accounts.Count == 0)
				this.Size = new Size(217, this.Height);
		}

		// Buddy List
		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count > 0)
			{
				ListViewItem test = listView1.SelectedItems[0];
				IMBuddy buddy = test.Tag as IMBuddy;
				ProtocolManager.Instance.OpenBuddyWindow(buddy, true);
			}
		}
		private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
		{
			/*ContactDetailPopupItemTrack = e.Item;
			if (ContactDetailPopup != null)
				ContactDetailPopup.Close();
			ContactDetailPopup = new frmContactDetailPopup();
			ContactDetailPopup.Buddy = (IMBuddy)((IMDataStruct)e.Item.Tag).buddy;*/
		}
		private void listView1_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = listView1.GetItemAt(e.X, e.Y);
			if (false && ContactDetailPopup != null && ContactDetailPopup.isVisible)
			{
				if (ContactDetailPopupItemTrack != item)
				{
					ContactDetailPopup.Close();
					ContactDetailPopup = null;
					ContactDetailPopupItemTrack = null;
				}
			}
		}
		private void listView1_MouseLeave(object sender, EventArgs e)
		{
			if (false && ContactDetailPopup != null)
				ContactDetailPopup.Close();
		}
		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			return;
			if (e.Item != null)
			{
				ContactDetailPopupItemTrack = e.Item;
				if (ContactDetailPopup != null)
					ContactDetailPopup.Close();
				ContactDetailPopup = new frmContactDetailPopup();
				ContactDetailPopup.Buddy = (IMBuddy)((IMDataStruct)e.Item.Tag).buddy;
				ContactDetailPopup.Show();
			} else {
				if (ContactDetailPopup != null)
					ContactDetailPopup.Close();
			}
		}
		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (listView1.SelectedIndices.Count > 0 && e.Button == MouseButtons.Right) // Right Click
			{
				// Prepare the context menu
				ListViewItem item = listView1.SelectedItems[0];
				IMBuddy buddy = item.Tag as IMBuddy;
				
				appearOnlineToolStripMenuItem.Text = String.Format(resources.GetString("appearOnlineToolStripMenuItem.Text"), buddy.DisplayName);
				appearOfflineToolStripMenuItem.Text = String.Format(resources.GetString("appearOfflineToolStripMenuItem.Text"), buddy.DisplayName);
				appearPermanentlyOfflineToolStripMenuItem.Text = String.Format(resources.GetString("appearPermanentlyOfflineToolStripMenuItem.Text"), buddy.DisplayName);

				if (buddy.Protocol.Status != IMStatus.INVISIBLE)
					appearOfflineToolStripMenuItem.Visible = false;
				else
					appearOfflineToolStripMenuItem.Visible = true;

				if (buddy.VisibilityStatus == UserVisibilityStatus.Online)
					appearOnlineToolStripMenuItem.Image = Resources.Point;
				else if (buddy.VisibilityStatus == UserVisibilityStatus.Offline)
					appearOfflineToolStripMenuItem.Image = Resources.Point;
				else if (buddy.VisibilityStatus == UserVisibilityStatus.Permanently_Offline)
					appearPermanentlyOfflineToolStripMenuItem.Image = Resources.Point;
			}
		}

		// System Events
		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			Trace.WriteLine("Display settings has been changed. Verifying that the window is on screen");

			bool adjustLoc = false;
			Rectangle screenrect = Screen.GetWorkingArea(this);

			// Test to see if the left hand corner is in the screen
			if (!screenrect.Contains(this.Location))
				adjustLoc = true;

			Rectangle formrect = new Rectangle(this.Location, this.Size);

			// Test to see if the the entire form is on screen
			if (!screenrect.Contains(formrect))
				adjustLoc = true;

			if (adjustLoc)
				this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2));
		}
		private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			if (e.Mode == PowerModes.Suspend)
			{
				AccountManager.DisconnectAll();
			} else {
				AccountManager.ConnectAll();
			}
		}
		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionLock)
				AccountManager.Status = IMStatus.AWAY;
			else if (e.Reason == SessionSwitchReason.SessionUnlock)
				AccountManager.Status = IMStatus.AVAILABLE;
		}
		private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
		{
			AccountManager.DisconnectAll();
			isAppCloseInvoke = true;
			this.Close();
		}

		// Protocol Events
		private void AccountManager_onStatusChange(object sender, StatusUpdateEventArgs e)
		{
			comboBox1.Enabled = true;
			if (e == null || lastSid != e.StatusUpdateId)
			{
				var stritem = (from string t in comboBox1.Items where t.ToLower() == AccountManager.Status.ToString().ToLower() select t).FirstOrDefault();
				if (stritem != null)
					comboBox1.SelectedItem = stritem;
			}
		}

		// Quick Login
		private void btnDoLogin_Click(object sender, EventArgs e)
		{
			btnDoLogin.Enabled = false;
			btnDoLogin.Text = "Connecting...";

			IMProtocol protocol = IMProtocol.FromString(cmbProtocol.Text);

			protocol.Username = txtUsername.Text;
			protocol.Password = txtPassword.Text;
			protocol.Enabled = true;
			IMProtocol.onError += new EventHandler<IMErrorEventArgs>(IMProtocol_onError);
			IMProtocol.onLogin += new EventHandler(IMProtocol_onLogin);

			AccountManager.Accounts.Add(protocol);
			AccountManager.Status = IMStatus.AVAILABLE;
			AccountManager.ConnectAll();
		}
		private void IMProtocol_onLogin(object sender, EventArgs e)
		{
			UpdateContactListStatus();
			IMProtocol.onLogin -= new EventHandler(IMProtocol_onLogin);
			IMProtocol.onError -= new EventHandler<IMErrorEventArgs>(IMProtocol_onError);
		}
		private void IMProtocol_onError(object sender, IMErrorEventArgs e)
		{
			if (e.Reason == IMErrorEventArgs.ErrorReason.INVALID_PASSWORD)
			{
				this.BeginInvoke(new MethodInvoker(delegate()
				{
					btnDoLogin.Enabled = true;
					btnDoLogin.Text = "Login";
					if (String.IsNullOrEmpty(e.Message))
						error.SetError(txtPassword, "Invalid password");
					else
						error.SetError(txtPassword, e.Message);
				}));
			}
			IMProtocol.onLogin -= new EventHandler(IMProtocol_onLogin);
			IMProtocol.onError -= new EventHandler<IMErrorEventArgs>(IMProtocol_onError);
		}

		// Menu bar
		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmAccounts win = new frmAccounts();
			if (win.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				UpdateContactListStatus();

			}
			this.sysTrayIcon.Text = "NexusIM\r\nConnected";
		}
		private void addBuddyToolStripMenuItem_Click(object sender,EventArgs e)
		{
			frmAddBuddy win = new frmAddBuddy();
			win.ShowDialog();
		}
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show(Resources.ExitProgramMessage, "Exit Program", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
			if (result == DialogResult.Yes)
			{
				AccountManager.DisconnectAll();
				isAppCloseInvoke = true;
				if (!IMSettings.SettingInterface.AutoSave)
					IMSettings.SettingInterface.Save();

				this.Close();
			}
		}
		private void OpenOptions_Click(object sender, EventArgs e)
		{
			frmSettings win = new frmSettings();
			win.ShowDialog();
		}
		private void newContactGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmContactGroups win = new frmContactGroups();
			if (win.ShowDialog() == DialogResult.OK)
			{

			}
		}
		private void joinChatRoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmJoinChatRoom win = new frmJoinChatRoom();
			win.ShowDialog();
		}
		private void smallListView_CheckStateChanged(object sender, EventArgs e)
		{
			listView1.CompactView = smallListView.Checked;
			IMSettings.SettingInterface.SetCustomSetting("compactview", smallListView.Checked.ToString());
		}
		private void compactListToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			listView1.CompactView = compactListToolStripMenuItem.Checked;
		}

		// System Tray
		private void sysTrayIcon_Click(object sender, EventArgs e)
		{
			if (((MouseEventArgs)e).Button == System.Windows.Forms.MouseButtons.Left)
			{
				frmSysTrayPopup win = new frmSysTrayPopup();

				MethodInvoker invoker = new MethodInvoker(delegate() { win.Show(); });
				this.BeginInvoke(invoker);
			}
		}
		private void sysTrayIcon_DoubleClick(object sender, EventArgs e)
		{
			if (this.Visible == true)
			{
				this.showBuddyListToolStripMenuItem.Checked = false;
				this.Hide();
			} else {
				this.showBuddyListToolStripMenuItem.Checked = true;
				this.Show();
				this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2));
				this.Activate();

				if (SuperTaskbarManager.IsSetup)
					SuperTaskbarManager.TriggerUpdate();
			}
		}

		// System Tray Context Menu
		private void showBuddyListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.Visible == true)
			{
				this.showBuddyListToolStripMenuItem.Checked = false;
				this.Hide();
			} else {
				this.showBuddyListToolStripMenuItem.Checked = true;
				this.Show();
				this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2));
				this.Activate();

				if (SuperTaskbarManager.IsSetup)
					SuperTaskbarManager.TriggerUpdate();
			}
		}
		private void compactMemoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			long before = GC.GetTotalMemory(false);
			GC.Collect();
			sysTrayIcon.ShowBalloonTip(5000, "Garbage Collector", "Generation: " + GC.GetGeneration(this) + "\r\nCollected: " + ((before - GC.GetTotalMemory(false)) / 1024) + "KB", ToolTipIcon.Info);
		}

		// Status Selector
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var stritem = (from t in Enum.GetValues(typeof(IMStatus)).Cast<IMStatus>() where t.ToString().ToLower() == comboBox1.SelectedItem.ToString().ToLower() select t).FirstOrDefault();
			comboBox1.Tag = true;
			lastSid = AccountManager.SetStatus(stritem);

			listView1.Focus();
		}

		// Contact Context Menu
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			username = ((IMDataStruct)listView1.SelectedItems[0].Tag).buddy;
			if (Environment.OSVersion.Version.Major >= 6)
			{
				TaskDialog confirmation = new TaskDialog();
				TaskDialogButton btnRemove = new TaskDialogButton("RemoveButton", "Remove");
				TaskDialogButton btnCancel = new TaskDialogButton("CancelButton", "Cancel");

				btnRemove.Click += new EventHandler(btnRemove_Click);

				confirmation.FooterCheckBoxText = "Remove from Address Book";
				confirmation.Icon = TaskDialogStandardIcon.Information;
				confirmation.InstructionText = "Are you sure you want to remove " + listView1.SelectedItems[0].Text;
				confirmation.Controls.Add(btnRemove);
				confirmation.Controls.Add(btnCancel);
				confirmation.Show();
			} else {
				if (MessageBox.Show("Are you sure you want to remove this friend?", "NexusIM", MessageBoxButtons.YesNo) == DialogResult.Yes)
					username.Protocol.RemoveFriend(username.Username);
			}
		}
		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmBuddyOptions win = new frmBuddyOptions();
			win.Buddy = (IMBuddy)((IMDataStruct)listView1.SelectedItems[0].Tag).buddy;
			win.ShowDialog();
		}
		
		private void lblLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmAccounts win = new frmAccounts();
			win.Show();
		}
		private IMBuddy username;
		private void btnRemove_Click(object sender, EventArgs e)
		{
			username.Protocol.RemoveFriend(username.Username);
		}
		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			if (!txtSearch.HintEnabled) // If it's in editing mode
			{
				List<IMBuddy> buddies = AccountManager.MergeAllBuddyLists(); // Get one big array with all buddies

				var buddyItems = from IMBuddy b in buddies where !b.IsInternalBuddy && !b.IsManaged select new { b.DisplayName, b };

				foreach (var buddy in buddyItems)
				{
					if (buddy.DisplayName.Contains(txtSearch.Text))
					{
						buddy.b.ContactItemVisible = true;
					} else {
						buddy.b.ContactItemVisible = false;
					}
				}
			}
		}
		protected override void WndProc(ref Message m)
		{
			if (this.Visible && m.Msg == Win32.WM_DISPLAYCHANGE)
			{
				frmMain_Shown(this, new EventArgs());
			} else if (m.Msg == Win32.WM_SIZING || m.Msg == 0x0005) {
				m.Result = (IntPtr)0x1;
				return;
			}
			base.WndProc(ref m);
		}
		private frmContactDetailPopup ContactDetailPopup = null;
		private ListViewItem ContactDetailPopupItemTrack = null;
		private void txtCustomStatus_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				e.Handled = true;
				if (cmbStatus.Text.Length >= 2)
				{
					cmbStatus.ForeColor = SystemColors.ControlDarkDark;
					AccountManager.StatusMessage = cmbStatus.Text;
				}
			}
		}
		private bool DoSnap(int pos, int edge)
		{
			int delta = pos - edge;
			return delta > 0 && delta <= SnapDist;
		}
		private void SnapIt(object sender, EventArgs e)
		{
			Screen scn = Screen.FromPoint(this.Location);

			// -- Inside Screen

			// Horizontal
			if (DoSnap(this.Left, scn.WorkingArea.Left))
				this.Left = scn.WorkingArea.Left;
			if (DoSnap(scn.WorkingArea.Right, this.Right))
				this.Left = scn.WorkingArea.Right - this.Width;

			// Vertical
			if (DoSnap(this.Top, scn.WorkingArea.Top))
				this.Top = scn.WorkingArea.Top;
			if (DoSnap(scn.WorkingArea.Bottom, this.Bottom))
				this.Top = scn.WorkingArea.Bottom - this.Height;
		}

		private void smallListView_Click(object sender, EventArgs e)
		{
			new frmChooseAvatar().ShowDialog();

			SensorUtilityManager.Setup();
			frmAbout win = new frmAbout();
			win.ShowDialog();
		}
	}
}