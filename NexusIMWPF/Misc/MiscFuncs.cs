using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InstantMessage.Events;
using InstantMessage;

namespace NexusIM
{
	internal delegate void GenericEvent();
	static class TimezoneFuncs
	{
		public static string ConvertToLocal(string input)
		{
			string returnVal = input; // Start with "Away since 5:45PM (GMT-8)"

			// Here be dragons
			MatchCollection detectedtimes = Regex.Matches(input, @"((([0-9]|1[0-2]):[0-5][0-9]((:|\.)[0-5][0-9])?( )?(AM|PM))|(([0]?[0-9]|1[0-9]|2[0-3])(:|\.)[0-5][0-9]((:|\.)[0-5][0-9])?))(( )?\(?)(GMT|UTC) ?((([\-|\+]([0-1]?[0-9])):?([0|3]?0|45)?|[0-9])\)?)?");

			IEnumerator timeEnum = detectedtimes.GetEnumerator();

			while (timeEnum.MoveNext())
			{
				Match time = (Match)timeEnum.Current;
				if (time.Groups[16].Success) // Did the user include the timezone stamp for us - "(GMT-8)"
				{
					DateTime dtime = DateTime.Parse(time.Groups[1].Value.ToString());
					dtime = dtime.AddHours(-(Convert.ToInt32(time.Groups[18].Value.ToString()) - TimeZoneInfo.Local.BaseUtcOffset.Hours));
					if (time.Groups[20].Success)
						dtime = dtime.AddMinutes(-(Convert.ToInt32(time.Groups[20].Value.ToString()) - TimeZoneInfo.Local.BaseUtcOffset.Minutes));

					returnVal = returnVal.Remove(time.Index, time.Length);

					string regenTime = dtime.ToString("t");

					returnVal = returnVal.Insert(time.Index, regenTime);
				}
			}

			return returnVal;
		}
		public static string ConvertToRemote(string input, string timezone)
		{
			string returnVal = input;

			// Here be dragons
			MatchCollection detectedtimes = Regex.Matches(input, @"((([0-9]|1[0-2]):[0-5][0-9]((:|\.)[0-5][0-9])?( )?(AM|PM))|(([0]?[0-9]|1[0-9]|2[0-3])(:|\.)[0-5][0-9]((:|\.)[0-5][0-9])?))/i");
			Match newtimezone = Regex.Match(timezone, @"([\-|\+](([0-1]?[0-9]):?([0|3]?0|45)?|[0-9]))");

			IEnumerator timeEnum = detectedtimes.GetEnumerator();

			while (timeEnum.MoveNext())
			{
				Match time = (Match)timeEnum.Current;
				DateTime dtime = DateTime.Parse(time.Groups[1].Value.ToString());
				dtime = dtime.AddHours(-(Convert.ToInt32(newtimezone.Groups[4].Value.ToString()) - TimeZoneInfo.Local.BaseUtcOffset.Hours));
				if (newtimezone.Groups[5].Success)
					dtime = dtime.AddMinutes(-(Convert.ToInt32(newtimezone.Groups[5].Value.ToString()) - TimeZoneInfo.Local.BaseUtcOffset.Minutes));

				returnVal = returnVal.Remove(time.Index, time.Length);

				string regenTime = dtime.ToString("t");

				returnVal = returnVal.Insert(time.Index, regenTime);
			}

			return returnVal;
		}
	}
	static class SymbolFuncs
	{
		public static Dictionary<string, string> RTFSymbolTable
		{
			get {
				if (rtfsymbolLookup.Count == 0)
					buildLookupTable();

				return rtfsymbolLookup;
			}
		}
		public static List<string> Symbols
		{
			get {
				if (symbols.Count == 0)
					buildSymbolTable();

				return symbols;
			}
		}
		private static Dictionary<string, string> rtfsymbolLookup = new Dictionary<string, string>();
		private static List<string> symbols = new List<string>();
		private static void buildLookupTable()
		{
			rtfsymbolLookup["∞"] = @"\u8734";
		}
		private static void buildSymbolTable()
		{
			symbols.Add("°");
			symbols.Add("∞");
			symbols.Add("«");
			symbols.Add("»");
		}
	}
}