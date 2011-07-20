using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;

namespace NexusIM.Controls.Inlines
{
	public class TimestampedInline : Span
	{
		public TimestampedInline()
		{
			mLeftSpan = new Span();

			mLeftSpan = new Span();
			Run mTimestamp = new Run();

			mTimestamp.Text = DateTime.Now.ToString(SettingCache.GetValue("ChatMsgTimestampFormat"), CultureInfo.InstalledUICulture);

			mLeftSpan.Inlines.Add(new Run("["));
			mLeftSpan.Inlines.Add(mTimestamp);
			mLeftSpan.Inlines.Add(new Run("] "));

			Inlines.Add(mLeftSpan);
		}

		protected Span LeftSpan
		{
			get	{
				return mLeftSpan;
			}
		}

		private Span mLeftSpan;
	}
}