using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuth
{
	public sealed class GoogleLatitudeOAuth : OAuthBase
	{
		public GoogleLatitudeOAuth(string consumerKey, string consumerSecret)
		{
			mConsumerKey = consumerKey;
			mConsumerSecret = consumerSecret;
		}



		private const string mAuthScope = "https://www.googleapis.com/auth/latitude";
		private string mConsumerKey;
		private string mConsumerSecret;
	}
}