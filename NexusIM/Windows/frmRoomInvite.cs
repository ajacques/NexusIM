using System;
using System.Windows.Forms;
using InstantMessage.Events;

namespace InstantMessage
{
	public partial class frmRoomInvite : Form
	{
		public frmRoomInvite()
		{
			InitializeComponent();
		}

		public IMRoomInviteEventArgs EventArgs
		{
			get {
				return mEventArgs;
			}
			set {
				mEventArgs = value;
			}
		}

		private IMRoomInviteEventArgs mEventArgs = null;

		private void frmRoomInvite_Shown(object sender, EventArgs e)
		{

		}
	}
}