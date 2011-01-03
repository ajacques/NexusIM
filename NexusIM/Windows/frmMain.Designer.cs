using System.Windows.Forms;
using NexusIM.Controls;

namespace InstantMessage
{
	partial class frmMain
	{
		private System.ComponentModel.IContainer components;
		private System.ComponentModel.ComponentResourceManager resources;
		
		// Quick Login
		private System.Windows.Forms.LinkLabel lblLink;
		private System.Windows.Forms.Panel pnlLogin;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnDoLogin;

		private System.Windows.Forms.Label lblContactListError;
		private System.Windows.Forms.ComboBox cmbProtocol;
		private System.Windows.Forms.ToolStripMenuItem newContactGroupToolStripMenuItem;
		public System.Windows.Forms.ImageList protocolPics;
		private System.Windows.Forms.NotifyIcon sysTrayIcon;
		private System.Windows.Forms.ContextMenuStrip contactContext;
		private NexusIM.Controls.HintTextBox txtSearch;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ContextMenuStrip sysTrayMenu;
		private System.Windows.Forms.ToolStripMenuItem smallListView;

		// System Tray Menu
		private System.Windows.Forms.ToolStripMenuItem showBuddyListToolStripMenuItem;

		// Messenger
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;

		// Accounts
		private System.Windows.Forms.ToolStripMenuItem accountsToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contactsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem joinChatRoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addBuddyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.accountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.compactListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.contactsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addBuddyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.joinChatRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newContactGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.smallListView = new System.Windows.Forms.ToolStripMenuItem();
			this.contactContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sendMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.stealthSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appearOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appearOfflineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appearPermanentlyOfflineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sysTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.myStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showBuddyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.sysTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.lblContactListError = new System.Windows.Forms.Label();
			this.lblLink = new System.Windows.Forms.LinkLabel();
			this.protocolPics = new System.Windows.Forms.ImageList(this.components);
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.lblUsername = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.pnlLogin = new System.Windows.Forms.Panel();
			this.cmbProtocol = new System.Windows.Forms.ComboBox();
			this.btnDoLogin = new System.Windows.Forms.Button();
			this.pnlCoreLogin = new System.Windows.Forms.Panel();
			this.lblCoreLogin = new System.Windows.Forms.Label();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			this.cmbStatus = new System.Windows.Forms.ComboBox();
			this.images = new System.Windows.Forms.ImageList(this.components);
			this.pnlReady = new System.Windows.Forms.Panel();
			this.txtSearch = new NexusIM.Controls.HintTextBox();
			this.listView1 = new NexusIM.Controls.BuddyList();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.compactMemoryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.contactContext.SuspendLayout();
			this.sysTrayMenu.SuspendLayout();
			this.pnlLogin.SuspendLayout();
			this.pnlCoreLogin.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.pnlReady.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accountsToolStripMenuItem,
            this.contactsToolStripMenuItem,
            this.toolsToolStripMenuItem});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			// 
			// accountsToolStripMenuItem
			// 
			this.accountsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.compactListToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem1});
			this.accountsToolStripMenuItem.Name = "accountsToolStripMenuItem";
			resources.ApplyResources(this.accountsToolStripMenuItem, "accountsToolStripMenuItem");
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OpenOptions_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			// 
			// compactListToolStripMenuItem
			// 
			this.compactListToolStripMenuItem.CheckOnClick = true;
			this.compactListToolStripMenuItem.Name = "compactListToolStripMenuItem";
			resources.ApplyResources(this.compactListToolStripMenuItem, "compactListToolStripMenuItem");
			this.compactListToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.compactListToolStripMenuItem_CheckStateChanged);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
			// 
			// exitToolStripMenuItem1
			// 
			this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
			resources.ApplyResources(this.exitToolStripMenuItem1, "exitToolStripMenuItem1");
			this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// contactsToolStripMenuItem
			// 
			this.contactsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBuddyToolStripMenuItem,
            this.joinChatRoomToolStripMenuItem,
            this.newContactGroupToolStripMenuItem});
			this.contactsToolStripMenuItem.Name = "contactsToolStripMenuItem";
			resources.ApplyResources(this.contactsToolStripMenuItem, "contactsToolStripMenuItem");
			// 
			// addBuddyToolStripMenuItem
			// 
			resources.ApplyResources(this.addBuddyToolStripMenuItem, "addBuddyToolStripMenuItem");
			this.addBuddyToolStripMenuItem.Name = "addBuddyToolStripMenuItem";
			this.addBuddyToolStripMenuItem.Click += new System.EventHandler(this.addBuddyToolStripMenuItem_Click);
			// 
			// joinChatRoomToolStripMenuItem
			// 
			this.joinChatRoomToolStripMenuItem.Name = "joinChatRoomToolStripMenuItem";
			resources.ApplyResources(this.joinChatRoomToolStripMenuItem, "joinChatRoomToolStripMenuItem");
			this.joinChatRoomToolStripMenuItem.Click += new System.EventHandler(this.joinChatRoomToolStripMenuItem_Click);
			// 
			// newContactGroupToolStripMenuItem
			// 
			this.newContactGroupToolStripMenuItem.Name = "newContactGroupToolStripMenuItem";
			resources.ApplyResources(this.newContactGroupToolStripMenuItem, "newContactGroupToolStripMenuItem");
			this.newContactGroupToolStripMenuItem.Click += new System.EventHandler(this.newContactGroupToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallListView});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
			// 
			// smallListView
			// 
			this.smallListView.Name = "smallListView";
			resources.ApplyResources(this.smallListView, "smallListView");
			this.smallListView.CheckStateChanged += new System.EventHandler(this.smallListView_CheckStateChanged);
			this.smallListView.Click += new System.EventHandler(this.smallListView_Click);
			// 
			// contactContext
			// 
			this.contactContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendMessageToolStripMenuItem,
            this.toolStripMenuItem3,
            this.stealthSettingsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.removeToolStripMenuItem,
            this.propertiesToolStripMenuItem});
			this.contactContext.Name = "contactContext";
			resources.ApplyResources(this.contactContext, "contactContext");
			// 
			// sendMessageToolStripMenuItem
			// 
			resources.ApplyResources(this.sendMessageToolStripMenuItem, "sendMessageToolStripMenuItem");
			this.sendMessageToolStripMenuItem.Name = "sendMessageToolStripMenuItem";
			this.sendMessageToolStripMenuItem.Click += new System.EventHandler(this.listView1_DoubleClick);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
			// 
			// stealthSettingsToolStripMenuItem
			// 
			this.stealthSettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appearOnlineToolStripMenuItem,
            this.appearOfflineToolStripMenuItem,
            this.appearPermanentlyOfflineToolStripMenuItem});
			this.stealthSettingsToolStripMenuItem.Name = "stealthSettingsToolStripMenuItem";
			resources.ApplyResources(this.stealthSettingsToolStripMenuItem, "stealthSettingsToolStripMenuItem");
			// 
			// appearOnlineToolStripMenuItem
			// 
			this.appearOnlineToolStripMenuItem.Name = "appearOnlineToolStripMenuItem";
			resources.ApplyResources(this.appearOnlineToolStripMenuItem, "appearOnlineToolStripMenuItem");
			// 
			// appearOfflineToolStripMenuItem
			// 
			this.appearOfflineToolStripMenuItem.Name = "appearOfflineToolStripMenuItem";
			resources.ApplyResources(this.appearOfflineToolStripMenuItem, "appearOfflineToolStripMenuItem");
			// 
			// appearPermanentlyOfflineToolStripMenuItem
			// 
			this.appearPermanentlyOfflineToolStripMenuItem.Name = "appearPermanentlyOfflineToolStripMenuItem";
			resources.ApplyResources(this.appearPermanentlyOfflineToolStripMenuItem, "appearPermanentlyOfflineToolStripMenuItem");
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// sysTrayMenu
			// 
			this.sysTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.myStatusToolStripMenuItem,
            this.showBuddyListToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.toolStripMenuItem5,
            this.exitToolStripMenuItem});
			this.sysTrayMenu.Name = "sysTrayMenu";
			resources.ApplyResources(this.sysTrayMenu, "sysTrayMenu");
			// 
			// myStatusToolStripMenuItem
			// 
			this.myStatusToolStripMenuItem.Name = "myStatusToolStripMenuItem";
			resources.ApplyResources(this.myStatusToolStripMenuItem, "myStatusToolStripMenuItem");
			// 
			// showBuddyListToolStripMenuItem
			// 
			this.showBuddyListToolStripMenuItem.Checked = true;
			this.showBuddyListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showBuddyListToolStripMenuItem.Name = "showBuddyListToolStripMenuItem";
			resources.ApplyResources(this.showBuddyListToolStripMenuItem, "showBuddyListToolStripMenuItem");
			this.showBuddyListToolStripMenuItem.Click += new System.EventHandler(this.showBuddyListToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3")});
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.Name = "comboBox1";
			// 
			// sysTrayIcon
			// 
			this.sysTrayIcon.ContextMenuStrip = this.sysTrayMenu;
			resources.ApplyResources(this.sysTrayIcon, "sysTrayIcon");
			this.sysTrayIcon.Click += new System.EventHandler(this.sysTrayIcon_Click);
			this.sysTrayIcon.DoubleClick += new System.EventHandler(this.sysTrayIcon_DoubleClick);
			// 
			// lblContactListError
			// 
			this.lblContactListError.AutoEllipsis = true;
			this.lblContactListError.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblContactListError, "lblContactListError");
			this.lblContactListError.Name = "lblContactListError";
			// 
			// lblLink
			// 
			resources.ApplyResources(this.lblLink, "lblLink");
			this.lblLink.Name = "lblLink";
			this.lblLink.TabStop = true;
			this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
			// 
			// protocolPics
			// 
			this.protocolPics.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("protocolPics.ImageStream")));
			this.protocolPics.TransparentColor = System.Drawing.Color.Transparent;
			this.protocolPics.Images.SetKeyName(0, "aim.png");
			// 
			// txtPassword
			// 
			resources.ApplyResources(this.txtPassword, "txtPassword");
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.UseSystemPasswordChar = true;
			// 
			// txtUsername
			// 
			resources.ApplyResources(this.txtUsername, "txtUsername");
			this.txtUsername.Name = "txtUsername";
			// 
			// lblUsername
			// 
			resources.ApplyResources(this.lblUsername, "lblUsername");
			this.lblUsername.Name = "lblUsername";
			// 
			// lblPassword
			// 
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.Name = "lblPassword";
			// 
			// pnlLogin
			// 
			this.pnlLogin.Controls.Add(this.cmbProtocol);
			this.pnlLogin.Controls.Add(this.btnDoLogin);
			this.pnlLogin.Controls.Add(this.lblPassword);
			this.pnlLogin.Controls.Add(this.txtPassword);
			this.pnlLogin.Controls.Add(this.lblLink);
			this.pnlLogin.Controls.Add(this.txtUsername);
			this.pnlLogin.Controls.Add(this.lblUsername);
			resources.ApplyResources(this.pnlLogin, "pnlLogin");
			this.pnlLogin.Name = "pnlLogin";
			// 
			// cmbProtocol
			// 
			this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProtocol.FormattingEnabled = true;
			this.cmbProtocol.Items.AddRange(new object[] {
            resources.GetString("cmbProtocol.Items")});
			resources.ApplyResources(this.cmbProtocol, "cmbProtocol");
			this.cmbProtocol.Name = "cmbProtocol";
			// 
			// btnDoLogin
			// 
			resources.ApplyResources(this.btnDoLogin, "btnDoLogin");
			this.btnDoLogin.Name = "btnDoLogin";
			this.btnDoLogin.UseVisualStyleBackColor = true;
			this.btnDoLogin.Click += new System.EventHandler(this.btnDoLogin_Click);
			// 
			// pnlCoreLogin
			// 
			this.pnlCoreLogin.Controls.Add(this.lblCoreLogin);
			this.pnlCoreLogin.Controls.Add(this.lblContactListError);
			resources.ApplyResources(this.pnlCoreLogin, "pnlCoreLogin");
			this.pnlCoreLogin.Name = "pnlCoreLogin";
			// 
			// lblCoreLogin
			// 
			resources.ApplyResources(this.lblCoreLogin, "lblCoreLogin");
			this.lblCoreLogin.Name = "lblCoreLogin";
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// cmbStatus
			// 
			this.cmbStatus.FormattingEnabled = true;
			resources.ApplyResources(this.cmbStatus, "cmbStatus");
			this.cmbStatus.Name = "cmbStatus";
			// 
			// images
			// 
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Transparent;
			this.images.Images.SetKeyName(0, "Point.png");
			// 
			// pnlReady
			// 
			this.pnlReady.Controls.Add(this.txtSearch);
			this.pnlReady.Controls.Add(this.listView1);
			this.pnlReady.Controls.Add(this.comboBox1);
			this.pnlReady.Controls.Add(this.cmbStatus);
			resources.ApplyResources(this.pnlReady, "pnlReady");
			this.pnlReady.Name = "pnlReady";
			// 
			// txtSearch
			// 
			this.txtSearch.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.txtSearch.Hint = "Search Contacts...";
			this.txtSearch.HintEnabled = true;
			resources.ApplyResources(this.txtSearch, "txtSearch");
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// listView1
			// 
			this.listView1.ContextMenuStrip = this.contactContext;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			resources.ApplyResources(this.listView1, "listView1");
			this.listView1.Name = "listView1";
			this.listView1.OwnerDraw = true;
			this.listView1.ShowItemToolTips = true;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.listView1_ItemMouseHover);
			this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
			this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
			this.listView1.MouseLeave += new System.EventHandler(this.listView1_MouseLeave);
			this.listView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseMove);
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compactMemoryToolStripMenuItem1});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
			// 
			// compactMemoryToolStripMenuItem1
			// 
			this.compactMemoryToolStripMenuItem1.Name = "compactMemoryToolStripMenuItem1";
			resources.ApplyResources(this.compactMemoryToolStripMenuItem1, "compactMemoryToolStripMenuItem1");
			this.compactMemoryToolStripMenuItem1.Click += new System.EventHandler(this.compactMemoryToolStripMenuItem_Click);
			// 
			// frmMain
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlCoreLogin);
			this.Controls.Add(this.pnlLogin);
			this.Controls.Add(this.pnlReady);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.ResizeBegin += new System.EventHandler(this.frmMain_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
			this.Move += new System.EventHandler(this.SnapIt);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.contactContext.ResumeLayout(false);
			this.sysTrayMenu.ResumeLayout(false);
			this.pnlLogin.ResumeLayout(false);
			this.pnlLogin.PerformLayout();
			this.pnlCoreLogin.ResumeLayout(false);
			this.pnlCoreLogin.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.pnlReady.ResumeLayout(false);
			this.pnlReady.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private ToolStripMenuItem myStatusToolStripMenuItem;
		private ErrorProvider error;
		private BuddyList listView1;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private ToolStripMenuItem exitToolStripMenuItem1;
		private ComboBox cmbStatus;
		private ToolStripMenuItem compactListToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem2;
		private ToolStripMenuItem sendMessageToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem3;
		private ToolStripMenuItem stealthSettingsToolStripMenuItem;
		private ToolStripMenuItem appearOnlineToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem4;
		private ToolStripMenuItem appearOfflineToolStripMenuItem;
		private ToolStripMenuItem appearPermanentlyOfflineToolStripMenuItem;
		private ImageList images;
		private Panel pnlCoreLogin;
		private Label lblCoreLogin;
		private Panel pnlReady;
		private ToolStripMenuItem debugToolStripMenuItem;
		private ToolStripMenuItem compactMemoryToolStripMenuItem1;
		private ToolStripSeparator toolStripMenuItem5;
	}
}