using System;
using System.Windows.Browser;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;

namespace SilverIM
{
	static class Extensions
	{
		public static string GetCookie(this HtmlDocument page, string cookiename)
		{
			string[] cookies = page.Cookies.Split(';');

			foreach (string cookie in cookies)
			{
				string[] keyValue = cookie.Split('=');
				if (keyValue.Length == 2)
				{
					if (keyValue[0].Trim(' ').ToString() == cookiename)
						return keyValue[1];
				}
			}
			return null;
		}
	}
}