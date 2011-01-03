using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	/// <summary>
	/// Summary for frmChatWindow
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class frmChatWindowBase : Form
	{
		public frmChatWindowBase()
		{
			InitializeComponent();
		}

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

		private void RebuildChatHistory()
		{
			string html = "";//"<html><body>";
			foreach (MessageContents msg in msghistory)
			{
				html += msg.ToString();
				if (!(msghistory.IndexOf(msg) == (msghistory.Count - 1)))
					html += "<br />";
			}
			//html += "</body></html>";
			webBrowser1.Document.Body.InnerHtml = html;
		}

		public void AppendCustomMessage(MessageContents msg)
		{
			AppendCustomMessage(msg, false);
		}
		public void AppendCustomMessage(MessageContents msg, bool flashwindow)
		{
			if (this.InvokeRequired)
			{
				MethodInvoker invoker = new MethodInvoker(delegate() { this.AppendCustomMessage(msg, flashwindow); });
				this.Invoke(invoker);
			} else {
				msghistory.Add(msg);
				RebuildChatHistory();
				
				if (flashwindow)
					HandleWindowFlash();

				ScrollToBottom();
			}
		}
		public void AppendChatMessage(string username, string message)
		{
			try {
				AppendChatMessage(username, message, AccountManager.GetByName(username).LocalTime);
			} catch (Exception) {
				AppendChatMessage(username, message, DateTime.Now);
			}
		}
		public void AppendChatMessage(string username, string message, DateTime receivedtime)
		{
			if (this.InvokeRequired)
			{
				MethodInvoker invoker = new MethodInvoker(delegate() { this.AppendChatMessage(username, message, receivedtime); });
				this.BeginInvoke(invoker);
			} else {
				try	{
					msghistory.Add(new ChatMessage(username, username == mProtocol.Username, message));
					RebuildChatHistory();

					HandleWindowFlash();

					HandleIMReceiveSound();

					HandleSpeechReadout(username, message);
				} catch (Exception) {}
			}
		}

		public void ScrollToBottom()
		{
			webBrowser1.Document.Window.ScrollTo(0, webBrowser1.Document.Body.ScrollRectangle.Height);
		}

		public void AppendCustomMessage(string message)
		{
			msghistory.Add(new CustomMessage(message));
		}
		public void ChangeTypingStatus(bool isTyping)
		{
			if (isTyping)
			{
				this.contactStatus.Text = mBuddy.Username + " is typing...";
			} else {
				this.contactStatus.Text = "";
			}
		}

		public void ShakeWindow()
		{
			if (this.InvokeRequired)
			{
				MethodInvoker invoker = new MethodInvoker(delegate() { this.ShakeWindow(); });
				frmMain.Instance.Invoke(invoker);
			}
			shakeTimer1.Start();
			shakeTimer2.Start();
		}	
		private string ReplaceSmileyTextWithImage(string text)
		{
			string returnVal = text;

			return returnVal;
		}
		private string ReplaceSymbolWithCode(string text)
		{
			IEnumerator enumr = SymbolFuncs.Symbols.GetEnumerator();
			while (enumr.MoveNext())
			{
				string symbol = (string)enumr.Current;

				if (text.Contains(symbol))
					text = text.Replace(symbol, SymbolFuncs.RTFSymbolTable[symbol]);
			}

			text = text.Replace("∞", @"\u8734");

			return text;
		}
		/// <summary>
		/// Checks to see if flashing the window is appropriate and if it is, flashes the window and updates the unread counter
		/// </summary>
		private void HandleWindowFlash()
		{
			if (!this.ContainsFocus || this.WindowState == FormWindowState.Minimized)
			{
				Win32.FlashWindow(this.Handle, true);
				unread++;
				this.Text = "[" + unread + "] - " + mBuddy.Username;
			}
		}
		private void HandleSpeechReadout(string username, string message)
		{
			if (mnuSpeechEnableReadout.Checked && mProtocol.Username != username)
			{
				SpeechSynthesizer synth = new SpeechSynthesizer();
				PromptBuilder prompt1 = new PromptBuilder();
				PromptStyle style = new PromptStyle();
				style.Emphasis = PromptEmphasis.Moderate;
				style.Rate = PromptRate.Medium;
				prompt1.StartStyle(style);
				prompt1.AppendText(username + " said ");
				prompt1.EndStyle();
				prompt1.AppendText(message);
				synth.SpeakAsync(prompt1);
			}
		}
		private void HandleIMReceiveSound()
		{
			if (!Convert.ToBoolean(IMSettings.GetCustomSetting("mutesounds", "False")))
			{
				SoundPlayer player = new SoundPlayer();
				player.SoundLocation = IMSettings.GetSystemSound("IM_Receive");
				if (player.SoundLocation != "")
				{
					player.Play();
				}
			}
		}

		// Properties
		public IMBuddy Buddy
		{
			get {
				return mBuddy;
			}
			set {
				mBuddy = value;
			}
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

		protected int SnapDist = 25;
		protected bool typingsent = false;
		protected List<MessageContents> msghistory = new List<MessageContents>();
		protected int unread = 0;

		private void frmChatWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			ProtocolManager.Instance.CloseBuddyWindow(Buddy);
			mBuddy.WindowOpen = false;
			mBuddy.ShowIsTypingMessage(false);
		}
		private void toolStripButton2_Click(object sender, System.EventArgs e)
		{
			if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtToSend.SelectionFont = fontDialog.Font;
			}
		}
		private void btnSymbol_ButtonClick(object sender, System.EventArgs e)
		{
			frmSymbols win = new frmSymbols();
			if (win.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtToSend.Text += win.listView1.SelectedItems[0].Text;
				txtToSend.SelectionStart = txtToSend.SelectionStart + 1;
				if (btnSymbol.DropDownItems.IndexOfKey(win.listView1.SelectedItems[0].Text) == -1)
				{
					btnSymbol.DropDownItems.Add(win.listView1.SelectedItems[0].Text);
					IMSettings.SetCustomSetting("symbolmrucount", IMSettings.GetCustomSetting("symbolmrucount", "0"));
					IMSettings.SetCustomSetting("symbolmru" + btnSymbol.DropDownItems.Count, win.listView1.SelectedItems[0].Text);
				}
			}
		}
		private void btnSymbol_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			txtToSend.Text += e.ClickedItem.Text;
			txtToSend.SelectionStart += 1;
		}
		private void mnuSpeechRecognition_CheckedChanged(object sender, EventArgs e)
		{
			SpeechRecognizer recon = new SpeechRecognizer();
			recon.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recon_SpeechRecognized);
			recon.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(recon_SpeechDetected);
		}
		private void recon_SpeechDetected(object sender, SpeechDetectedEventArgs e)
		{
			e = e;
		}
		private void recon_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			// im bored.. need something better to implement
			e = e;
		}
		private void muteSoundsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			IMSettings.SetCustomSetting("mutesounds", System.Convert.ToString(muteSoundsToolStripMenuItem.Checked));
		}
		private void frmChatWindowBase_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			AccountManager.onGlobalError += new EventHandler<GlobalErrorEventArgs>(AccountManager_onGlobalError);

			muteSoundsToolStripMenuItem.Checked = Convert.ToBoolean(IMSettings.GetCustomSetting("mutesounds", "False"));
			mnuSpeechEnableReadout.Checked = Convert.ToBoolean(IMSettings.GetCustomSetting("speechreadout", "False"));

			webBrowser1.Navigate("about:blank");

			// Recent Symbols
			short symbolcount = Convert.ToInt16(IMSettings.GetCustomSetting("symbolmrucount", "0"));
			if (symbolcount >= 1)
			{
				for (int i = 0; i < symbolcount; i++)
				{
					btnSymbol.DropDownItems.Add(IMSettings.GetCustomSetting("symbolmru" + i, "(undefined)"));
				}
			}

			//mBuddy.WindowOpen = true;

			// Feature Support
			if (mProtocol.SupportsUserNotify)
			{
				btnBuzz.Visible = true;
			}

			if (mBuddy.IsMobileContact)
			{
				btnBuzz.Visible = false;
				btnFont.Visible = false;
				btnSymbol.Visible = false;
				charsRemaining.Text = "Characters Remaining: " + mBuddy.MaxMessageLength;
			}
		}
		private void AccountManager_onGlobalError(object sender, GlobalErrorEventArgs e)
		{
			if (e.Reason == GlobalErrorReasons.NotConnectedToNetwork)
			{
				msghistory.Add(new CustomMessage("You are not currently connect to the internet. Any messages you send now will be queued until you login next time."));
				RebuildChatHistory();
			}
		}
		private void mnuSpeechEnableReadout_Click(object sender, EventArgs e)
		{
			IMSettings.SetCustomSetting("speechreadout", Convert.ToString(mnuSpeechEnableReadout.Checked));
		}
		private void btnBuzz_Click(object sender, EventArgs e)
		{
			msghistory.Add(new SendBuzzMessage(mBuddy.Username));
			mProtocol.BuzzFriend(mBuddy.DisplayName);
			RebuildChatHistory();
		}
		private void shakeTimer1_Tick(object sender, EventArgs e)
		{
			Random inGen = new Random();
			int maxShake = 5;

			this.Location = new Point(this.Location.X + inGen.Next(-maxShake, maxShake), this.Location.Y + inGen.Next(-maxShake, maxShake));
		}
		private void shakeTimer2_Tick(object sender, EventArgs e)
		{
			shakeTimer1.Stop();
			shakeTimer2.Stop();
		}
		private void btnEmoticon_Click(object sender, EventArgs e)
		{
			frmEmoticon win = new frmEmoticon();
			if (win.ShowDialog() == DialogResult.OK)
			{

			}
		}
		private void btnAlwaysOntop_Click(object sender, EventArgs e)
		{
			this.TopMost = btnAlwaysOntop.Checked;
		}
		private void frmChatWindow_Activated(object sender, EventArgs e)
		{
			unread = 0;
			if (mBuddy != null)
				this.Text = mBuddy.Username;
		}
		private bool DoSnap(int pos, int edge)
		{
			int delta = pos - edge;
			return delta > 0 && delta <= SnapDist;
		}
		private void SnapIt()
		{
			Screen scn = Screen.FromPoint(this.Location);
			if (DoSnap(this.Left, scn.WorkingArea.Left)) this.Left = scn.WorkingArea.Left;
			if (DoSnap(this.Right, scn.WorkingArea.Left)) this.Left = scn.WorkingArea.Left;
			if (DoSnap(this.Top, scn.WorkingArea.Top)) this.Top = scn.WorkingArea.Top;
			if (DoSnap(scn.WorkingArea.Right, this.Right)) this.Left = scn.WorkingArea.Right - this.Width;
			if (DoSnap(scn.WorkingArea.Bottom, this.Bottom)) this.Top = scn.WorkingArea.Bottom - this.Height;
		}
		private void frmChatWindowBase_Resize(object sender, EventArgs e)
		{
			SnapIt();
			
			txtToSend.Location = new Point(12, this.Height - (61 + 64));
			txtToSend.Size = new Size(this.Width - 40, 61);

			bottomToolStrip.Location = new Point(9, this.Height - 150);
			//bottomToolStrip.Size = new Size(this.Width - 9, 25);

			webBrowser1.Size = new Size(this.Width - 40, this.Height - 180);
		}
	}
}