using System.Net;
using System;
using System.IO;

namespace OAuth
{
	internal class FireEaglef : OAuthBase
	{
		public FireEaglef(string consumerKey, string consumerSecret)
		{
			mConsumerKey = consumerKey;
			mConsumerSecret = consumerSecret;
		}

		public void GetRequestToken(string callbackurl)
		{
			string nonce = base.GenerateNonce();
			string timestamp = base.GenerateTimeStamp();

			callbackurl = UrlEncode(callbackurl);

			Uri url = new Uri("https://fireeagle.yahooapis.com/oauth/request_token");

			string fullurl = base.GenerateSignature(url, mConsumerKey, mConsumerSecret, null, null, "GET", timestamp, nonce, SignatureTypes.HMACSHA1);

			fullurl += "&oauth_callback=" + callbackurl;

			HttpWebRequest request = WebRequest.Create(fullurl) as HttpWebRequest;
			request.Method = "GET";
			request.ContentType = "application/x-www-form-urlencoded";

			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string output = reader.ReadToEnd();
			reader.Close();
		}

		public string ConsumerKey
		{
			get {
				return mConsumerKey;
			}
		}
		public string ConsumerSecret
		{
			get {
				return mConsumerSecret;
			}
		}

		private string mConsumerKey;
		private string mConsumerSecret;
	}
}