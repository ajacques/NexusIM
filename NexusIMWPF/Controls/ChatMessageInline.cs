using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace NexusIM.Controls
{
	public class ChatMessageInline : Span
	{
		public ChatMessageInline()
		{
			mUsername = new Run();

			this.Inlines.Add(mUsername);
		}

		public string Username
		{
			get	{
				return mUsername.Text;
			}
			set	{
				mUsername.Text = value;
			}
		}

		private Run mUsername;
	}
}