using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace NexusIM.Controls
{
	public class SendMessageEventArgs : EventArgs
	{
		public SendMessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message
		{
			get;
			private set;
		}
	}
	public class SendMessageBox : TextBox
	{
		public SendMessageBox()
		{
			mMessageHistory = new LinkedList<string>();
			mHistoryRoot = mMessageHistory.AddFirst(String.Empty);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == Key.Enter && Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.RightCtrl))
			{
				e.Handled = true;

				string message = this.Text;

				if (String.IsNullOrEmpty(message))
					return;

				mHistoryNode = mMessageHistory.AddAfter(mHistoryRoot, message);

				if (MessageSend != null)
					MessageSend(this, new SendMessageEventArgs(message));

				Text = String.Empty;
			}
		}

		public event EventHandler<SendMessageEventArgs> MessageSend;

		// Chat History Variables
		private LinkedList<string> mMessageHistory;
		private LinkedListNode<string> mHistoryNode;
		private LinkedListNode<string> mHistoryRoot;
	}
}