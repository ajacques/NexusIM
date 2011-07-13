using System;
using System.Windows.Documents;
using System.Windows.Media;
using NexusIM.Misc;
using System.Globalization;

namespace NexusIM.Controls.Inlines
{
	public class ChatMessageInline : TimestampedInline
	{
		public ChatMessageInline()
		{
			mUsername = new Run();
			mMessage = new Run();

			this.LeftSpan.Inlines.Add(mUsername);
			this.Inlines.Add(new Run(": "));
			this.Inlines.Add(mMessage);
		}

		public Color UsernameColor
		{
			set	{
				LeftSpan.Foreground = new SolidColorBrush(value);
			}
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
		public string MessageBody
		{
			get	{
				return mMessage.Text;
			}
			set	{
				mMessage.Text = value;
			}
		}

		private Run mUsername;
		private Run mMessage;
	}
}