using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusIM.Controls
{
	public partial class HintTextBox : TextBox
	{
		public HintTextBox()
		{
			base.Enter += new EventHandler(Control_Enter);
			base.Leave += new EventHandler(Control_Leave);
		}

		private string mHint = "";
		private bool mInHintMode = true;

		public bool HintEnabled
		{
			get {
				return mInHintMode;
			}
			set {
				mInHintMode = value;
				UpdateHint();
			}
		}

		public string Hint
		{
			get {
				return mHint;
			}
			set {
				mHint = value;
				UpdateHint();
			}
		}

		private void UpdateHint()
		{
			if (mInHintMode)
			{
				base.Text = mHint;
				base.ForeColor = SystemColors.ControlDark;
			} else {
				base.ForeColor = Color.Black;
				base.Text = "";
			}
		}

		private void Control_Enter(object sender, EventArgs e)
		{
			if (base.ForeColor == System.Drawing.SystemColors.ControlDark)
			{
				mInHintMode = false;
				UpdateHint();
			}
		}
		private void Control_Leave(object sender, EventArgs e)
		{
			if (base.Text.Length == 0)
			{
				mInHintMode = true;
				UpdateHint();
			}
		}

		private void HintTextBox_Load(object sender, EventArgs e)
		{
			UpdateHint();
		}
	}
}
