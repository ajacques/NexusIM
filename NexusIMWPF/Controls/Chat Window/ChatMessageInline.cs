using System;
using System.Globalization;
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

			mUsername.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
			mTimestamp.Text = String.Format(CultureInfo.InstalledUICulture, SettingCache.GetValue("ChatMsgTimestampFormat"), DateTime.Now);
			mTimestamp.Foreground = mUsername.Foreground;

			this.Inlines.Add(mTimestamp);
			this.Inlines.Add(new Run(" "));
			this.Inlines.Add(mUsername);
			this.Inlines.Add(new Run(": "));
			this.Inlines.Add(mMessage);
		}

		public Color UsernameColor
		{
			set	{
				mTimestamp.Foreground = mUsername.Foreground = new SolidColorBrush(value);
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

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		private Run mTimestamp;
		private Run mUsername;
		private Run mMessage;
	}
}