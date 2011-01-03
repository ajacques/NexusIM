using System.ComponentModel;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmChatWindowBase
	{
		// Controls
		protected IMProtocol mProtocol;
		protected IMBuddy mBuddy = null;
		protected RichTextBox txtToSend;
		protected ToolStripStatusLabel contactStatus;
		protected FontDialog fontDialog;
		protected ToolStrip bottomToolStrip;
		protected ToolStripButton btnFont;
		protected ToolStripButton btnEmoticon;
		protected ToolStripSplitButton btnSymbol;
		protected ToolStrip topToolStrip;
		protected ToolStripSplitButton mnuSound;
		protected ToolStripMenuItem mnuSpeechRecognition;
		protected ToolStripMenuItem mnuSpeechEnableReadout;
		protected ToolStripSeparator toolStripSeparator1;
		protected ToolStripButton btnBuzz;
		protected Timer shakeTimer1;
		protected Timer shakeTimer2;
		protected ToolStripSplitButton btnWindowSettings;
		protected ToolStripMenuItem btnAlwaysOntop;
		protected ToolStripStatusLabel charsRemaining;
		protected ToolStripMenuItem muteSoundsToolStripMenuItem;
		protected ToolStripSplitButton mnuExtras;
		protected ToolStripMenuItem mnuExtrasIMVironment;
		protected ToolStripMenuItem mnuIMVironDoodle;
		private IContainer components;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		protected void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChatWindowBase));
			this.charsRemaining = new System.Windows.Forms.ToolStripStatusLabel();
			this.contactStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.bottomToolStrip = new System.Windows.Forms.ToolStrip();
			this.btnFont = new System.Windows.Forms.ToolStripButton();
			this.btnEmoticon = new System.Windows.Forms.ToolStripButton();
			this.btnSymbol = new System.Windows.Forms.ToolStripSplitButton();
			this.btnBuzz = new System.Windows.Forms.ToolStripButton();
			this.topToolStrip = new System.Windows.Forms.ToolStrip();
			this.mnuSound = new System.Windows.Forms.ToolStripSplitButton();
			this.mnuSpeechRecognition = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSpeechEnableReadout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.muteSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnWindowSettings = new System.Windows.Forms.ToolStripSplitButton();
			this.btnAlwaysOntop = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuExtras = new System.Windows.Forms.ToolStripSplitButton();
			this.mnuExtrasIMVironment = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuIMVironDoodle = new System.Windows.Forms.ToolStripMenuItem();
			this.txtToSend = new System.Windows.Forms.RichTextBox();
			this.shakeTimer1 = new System.Windows.Forms.Timer(this.components);
			this.shakeTimer2 = new System.Windows.Forms.Timer(this.components);
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.bottomToolStrip.SuspendLayout();
			this.topToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// charsRemaining
			// 
			this.charsRemaining.Name = "charsRemaining";
			this.charsRemaining.Size = new System.Drawing.Size(0, 17);
			// 
			// contactStatus
			// 
			this.contactStatus.Name = "contactStatus";
			this.contactStatus.Size = new System.Drawing.Size(0, 17);
			// 
			// fontDialog
			// 
			this.fontDialog.ShowColor = true;
			this.fontDialog.ShowEffects = false;
			// 
			// bottomToolStrip
			// 
			this.bottomToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.bottomToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFont,
            this.btnEmoticon,
            this.btnSymbol,
            this.btnBuzz});
			this.bottomToolStrip.Location = new System.Drawing.Point(9, 227);
			this.bottomToolStrip.Name = "bottomToolStrip";
			this.bottomToolStrip.Size = new System.Drawing.Size(220, 25);
			this.bottomToolStrip.Stretch = true;
			this.bottomToolStrip.TabIndex = 1;
			this.bottomToolStrip.Text = "toolStrip1";
			// 
			// btnFont
			// 
			this.btnFont.Image = global::NexusIM.Properties.Resources.text_bold;
			this.btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(51, 22);
			this.btnFont.Text = "Font";
			this.btnFont.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// btnEmoticon
			// 
			this.btnEmoticon.Image = global::NexusIM.Properties.Resources.emoticon_smile;
			this.btnEmoticon.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnEmoticon.Name = "btnEmoticon";
			this.btnEmoticon.Size = new System.Drawing.Size(78, 22);
			this.btnEmoticon.Text = "Emoticon";
			this.btnEmoticon.Click += new System.EventHandler(this.btnEmoticon_Click);
			// 
			// btnSymbol
			// 
			this.btnSymbol.Image = global::NexusIM.Properties.Resources.text_letter_omega;
			this.btnSymbol.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSymbol.Name = "btnSymbol";
			this.btnSymbol.Size = new System.Drawing.Size(79, 22);
			this.btnSymbol.Text = "Symbol";
			this.btnSymbol.ButtonClick += new System.EventHandler(this.btnSymbol_ButtonClick);
			this.btnSymbol.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnSymbol_DropDownItemClicked);
			// 
			// btnBuzz
			// 
			this.btnBuzz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnBuzz.Image = ((System.Drawing.Image)(resources.GetObject("btnBuzz.Image")));
			this.btnBuzz.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnBuzz.Name = "btnBuzz";
			this.btnBuzz.Size = new System.Drawing.Size(23, 22);
			this.btnBuzz.Text = "Buzz!";
			this.btnBuzz.Visible = false;
			this.btnBuzz.Click += new System.EventHandler(this.btnBuzz_Click);
			// 
			// topToolStrip
			// 
			this.topToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSound,
            this.btnWindowSettings,
            this.mnuExtras});
			this.topToolStrip.Location = new System.Drawing.Point(0, 0);
			this.topToolStrip.Name = "topToolStrip";
			this.topToolStrip.Size = new System.Drawing.Size(307, 25);
			this.topToolStrip.TabIndex = 4;
			// 
			// mnuSound
			// 
			this.mnuSound.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSpeechRecognition,
            this.mnuSpeechEnableReadout,
            this.toolStripSeparator1,
            this.muteSoundsToolStripMenuItem});
			this.mnuSound.Image = global::NexusIM.Properties.Resources.sound;
			this.mnuSound.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuSound.Name = "mnuSound";
			this.mnuSound.Size = new System.Drawing.Size(73, 22);
			this.mnuSound.Text = "Sound";
			// 
			// mnuSpeechRecognition
			// 
			this.mnuSpeechRecognition.CheckOnClick = true;
			this.mnuSpeechRecognition.Name = "mnuSpeechRecognition";
			this.mnuSpeechRecognition.Size = new System.Drawing.Size(179, 22);
			this.mnuSpeechRecognition.Text = "Speech Recognition";
			this.mnuSpeechRecognition.CheckedChanged += new System.EventHandler(this.mnuSpeechRecognition_CheckedChanged);
			// 
			// mnuSpeechEnableReadout
			// 
			this.mnuSpeechEnableReadout.CheckOnClick = true;
			this.mnuSpeechEnableReadout.Name = "mnuSpeechEnableReadout";
			this.mnuSpeechEnableReadout.Size = new System.Drawing.Size(179, 22);
			this.mnuSpeechEnableReadout.Text = "Speech Readout";
			this.mnuSpeechEnableReadout.Click += new System.EventHandler(this.mnuSpeechEnableReadout_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
			// 
			// muteSoundsToolStripMenuItem
			// 
			this.muteSoundsToolStripMenuItem.CheckOnClick = true;
			this.muteSoundsToolStripMenuItem.Name = "muteSoundsToolStripMenuItem";
			this.muteSoundsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.muteSoundsToolStripMenuItem.Text = "Mute Sounds";
			this.muteSoundsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.muteSoundsToolStripMenuItem_CheckedChanged);
			// 
			// btnWindowSettings
			// 
			this.btnWindowSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnWindowSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAlwaysOntop});
			this.btnWindowSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnWindowSettings.Image")));
			this.btnWindowSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnWindowSettings.Name = "btnWindowSettings";
			this.btnWindowSettings.Size = new System.Drawing.Size(32, 22);
			this.btnWindowSettings.Text = "Windows Settings";
			// 
			// btnAlwaysOntop
			// 
			this.btnAlwaysOntop.CheckOnClick = true;
			this.btnAlwaysOntop.Name = "btnAlwaysOntop";
			this.btnAlwaysOntop.Size = new System.Drawing.Size(148, 22);
			this.btnAlwaysOntop.Text = "Always Ontop";
			this.btnAlwaysOntop.Click += new System.EventHandler(this.btnAlwaysOntop_Click);
			// 
			// mnuExtras
			// 
			this.mnuExtras.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.mnuExtras.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExtrasIMVironment});
			this.mnuExtras.Image = ((System.Drawing.Image)(resources.GetObject("mnuExtras.Image")));
			this.mnuExtras.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuExtras.Name = "mnuExtras";
			this.mnuExtras.Size = new System.Drawing.Size(32, 22);
			this.mnuExtras.Text = "toolStripSplitButton1";
			this.mnuExtras.ToolTipText = "Extras";
			// 
			// mnuExtrasIMVironment
			// 
			this.mnuExtrasIMVironment.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuIMVironDoodle});
			this.mnuExtrasIMVironment.Enabled = false;
			this.mnuExtrasIMVironment.Name = "mnuExtrasIMVironment";
			this.mnuExtrasIMVironment.ShortcutKeyDisplayString = "";
			this.mnuExtrasIMVironment.Size = new System.Drawing.Size(144, 22);
			this.mnuExtrasIMVironment.Text = "IMVironment";
			// 
			// mnuIMVironDoodle
			// 
			this.mnuIMVironDoodle.CheckOnClick = true;
			this.mnuIMVironDoodle.Name = "mnuIMVironDoodle";
			this.mnuIMVironDoodle.Size = new System.Drawing.Size(112, 22);
			this.mnuIMVironDoodle.Text = "Doodle";
			// 
			// txtToSend
			// 
			this.txtToSend.DetectUrls = false;
			this.txtToSend.Location = new System.Drawing.Point(12, 255);
			this.txtToSend.MaxLength = 1000;
			this.txtToSend.Name = "txtToSend";
			this.txtToSend.Size = new System.Drawing.Size(283, 61);
			this.txtToSend.TabIndex = 0;
			this.txtToSend.Text = "";
			// 
			// shakeTimer1
			// 
			this.shakeTimer1.Interval = 50;
			this.shakeTimer1.Tick += new System.EventHandler(this.shakeTimer1_Tick);
			// 
			// shakeTimer2
			// 
			this.shakeTimer2.Interval = 500;
			this.shakeTimer2.Tick += new System.EventHandler(this.shakeTimer2_Tick);
			// 
			// webBrowser1
			// 
			this.webBrowser1.AllowNavigation = false;
			this.webBrowser1.AllowWebBrowserDrop = false;
			this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
			this.webBrowser1.Location = new System.Drawing.Point(9, 28);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(286, 196);
			this.webBrowser1.TabIndex = 5;
			this.webBrowser1.WebBrowserShortcutsEnabled = false;
			// 
			// frmChatWindowBase
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 341);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.topToolStrip);
			this.Controls.Add(this.bottomToolStrip);
			this.Name = "frmChatWindowBase";
			this.Text = "frmChatWindow";
			this.Load += new System.EventHandler(this.frmChatWindowBase_Load);
			this.Activated += new System.EventHandler(this.frmChatWindow_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmChatWindow_FormClosed);
			this.Move += new System.EventHandler(this.frmChatWindowBase_Resize);
			this.Resize += new System.EventHandler(this.frmChatWindowBase_Resize);
			this.bottomToolStrip.ResumeLayout(false);
			this.bottomToolStrip.PerformLayout();
			this.topToolStrip.ResumeLayout(false);
			this.topToolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private WebBrowser webBrowser1;
	}
}