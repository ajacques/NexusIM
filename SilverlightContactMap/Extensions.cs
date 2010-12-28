using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightContactMap
{
	public static class Extensions
	{
		public static string ToHumanReadableString(this DateTime input)
		{
			TimeSpan difference = DateTime.UtcNow.Subtract(input);

			string sctext = "{0} {1} ago";
			int count = 0;
			string span = ""; // Seconds Minutes Hours Days Weeks Months

			if (difference.Days > 7)
			{
				count = difference.Days / 7;
				span = "week";
			} else if (difference.Days > 0) {
				count = difference.Days;
				span = "day";
			} else if (difference.Hours > 0) {
				count = difference.Hours;
				span = "hour";
			} else if (difference.Minutes > 0) {
				count = difference.Minutes;
				span = "minute";
			} else {
				count = difference.Seconds;
				span = "second";
			}

			if (count != 1)
				span += "s"; // Not good for globalization TODO: Fix

			return String.Format(sctext, count, span);
		}
	}
}
