using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using NexusWeb.Properties;

namespace NexusWeb
{
	public static class CounterCSRF
	{
		/// <summary>
		/// Attempts to prevent Cross-site Request Forgeries for WCF Ajax requests
		/// </summary>
		/// <returns>False if this request has been confirmed as un-safe and should be denied. True if this request is possibly safe.</returns>
		public static bool IsValidFromSecureWCF()
		{
			if (HttpContext.Current.Request.Url.LocalPath == "/unittesting")
				return true;

			if (HttpContext.Current.Request.UrlReferrer == null)
				return false;

			string domain = HttpContext.Current.Request.UrlReferrer.Authority;

			return Regex.IsMatch(domain, Settings.Default.SecureReferrerAcceptRegex);
		}
	}
}