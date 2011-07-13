using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;

namespace NexusIM.Controls.Inlines
{
	public class UserActionInline : TimestampedInline
	{
		public UserActionInline()
		{
			mUsername = new Run();
			mMessage = new Run();

			this.LeftSpan.Inlines.Add(mUsername);
			this.Inlines.Add(new Run(" "));
			this.Inlines.Add(mMessage);
		}

		public Color UsernameColor
		{
			set	{
				this.LeftSpan.Foreground = new SolidColorBrush(value);
			}
		}
		public string Username
		{
			get	{
				return mUsername.Text;
			}
			set	{
				mUsername.Text = "***" + value;
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

		private Run mUsername;
		private Run mMessage;
	}
}