using System;
using System.Windows.Documents;
using System.Windows.Media;
using NexusIM.Misc;

namespace NexusIM.Controls
{
	public class ChatMessageInline : Span
	{
		public ChatMessageInline()
		{
			mUsername = new Run();
			mMessage = new Run();
			mTimestamp = new Run();
			mAuthSpan = new Span();

			mAuthSpan.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
			mTimestamp.Text = DateTime.Now.ToString(SettingCache.GetValue("ChatMsgTimestampFormat"));
			
			mAuthSpan.Inlines.Add(new Run("["));
			mAuthSpan.Inlines.Add(mTimestamp);
			mAuthSpan.Inlines.Add(new Run("] "));
			mAuthSpan.Inlines.Add(mUsername);

			this.Inlines.Add(mAuthSpan);
			this.Inlines.Add(new Run(": "));
			this.Inlines.Add(mMessage);
		}

		public Color UsernameColor
		{
			set	{
				mAuthSpan.Foreground = new SolidColorBrush(value);
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

		private Span mAuthSpan;
		private Run mTimestamp;
		private Run mUsername;
		private Run mMessage;
	}
}