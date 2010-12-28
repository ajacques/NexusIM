using System;
using System.Web.UI;

namespace NexusWeb
{
	public static class DateTimeExtensions
	{
		public static string ToHumanReadableString(this DateTime input)
		{
			TimeSpan difference = DateTime.UtcNow.Subtract(input);

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

			return count + " " + span;
		}
	}

	internal static class PageExtensions
	{
		public static void AddScript(this Page page, string uri)
		{
			ScriptManager.GetCurrent(page).Scripts.Add(new ScriptReference(uri));
		}
	}
}