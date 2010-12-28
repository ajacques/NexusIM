using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Xml.XPath;
using InstantMessage;
using System.Diagnostics;

namespace InstantMessage.SocialNetworks
{
	[SocialNetwork("xbxlive")]
	public class XboxLive : IMProtocol, ISocialNetwork
	{
		public XboxLive()
		{
			mCookieJar = new CookieContainer();
			
		}

		public override void BeginLogin()
		{
			Trace.WriteLine("XboxLive: Beginning Login Sequence");
			HttpWebRequest request = Homepage;
			request.BeginGetResponse(new AsyncCallback(OnHomepageDownload), request);
		}

		private void UpdateFriendList()
		{
			HttpWebRequest request = FriendsPage;
			request.BeginGetResponse(new AsyncCallback(OnFriendPageDownload), request);
		}

		// Async Callbacks
		private void OnFriendPageDownload(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;

			StreamReader reader = new StreamReader(response.GetResponseStream());
			HtmlDocument doc = new HtmlDocument();

			XPathNavigator navi = doc.CreateNavigator();
			XPathNodeIterator iterator = navi.Select(mFriendRootNodeSelect);
			foreach (XmlNode node in iterator)
				;
		}

		// Login Steps
		private void OnHomepageDownload(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;

			StreamReader reader = new StreamReader(response.GetResponseStream());
			string homepage = reader.ReadToEnd();

			if (homepage.Contains(mIsLoggedInMatchKey))
			{
				Trace.WriteLine("XboxLive: Login Successful. Downloading Friends List");
				triggerOnLogin(null);
				UpdateFriendList();
			} else {
				Trace.WriteLine("XboxLive: Not Logged-in yet");
				Match match = mLoginPageMatch.Match(homepage);
				Uri loginpage = new Uri(match.Groups[1].Value);

				HttpWebRequest loginpagerqst = WebRequest.Create(loginpage) as HttpWebRequest;
				loginpagerqst.CookieContainer = mCookieJar;
				loginpagerqst.BeginGetResponse(new AsyncCallback(OnLoginPageDownload), loginpagerqst);
			}
		}
		private void OnLoginPageDownload(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string output = reader.ReadToEnd();

			// Get the MSPOK cookie because .net won't do it itself
			Match okmatch = mMSPOKCookieMatch.Match(response.Headers["Set-Cookie"]);

			mCookieJar.SetCookies(new Uri("http://login.live.com"), okmatch.Groups[1].Value);

			Match actionMatch = mActionUriMatch.Match(output);
			Uri actionUri = new Uri(actionMatch.Groups[1].Value);

			MatchCollection postdataColl = mHiddenFieldMatch.Matches(output);
			Dictionary<string, string> postdata = new Dictionary<string, string>();

			foreach (Match match in postdataColl)
				postdata.Add(match.Groups[1].Value, match.Groups[2].Value);

			GetLoginSubmitResponse(actionUri, postdata, new AsyncCallback(OnLoginAttemptDownload));
		}
		private void OnLoginAttemptDownload(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string output = reader.ReadToEnd();

			// Final Handoff - Redirection to .xbox.com with cookies
			Match action = mActionUriMatch.Match(output);
			Uri actionUri = new Uri(action.Groups[1].Value);

			MatchCollection fields = mHiddenFieldMatch.Matches(output);
			string postdata = fields.OfType<Match>().Select(m => m.Groups[1].Value + "=" + m.Groups[2].Value).Aggregate((kv, v2) => kv + "&" + v2);

			HttpWebRequest xboxComRequest = WebRequest.Create(actionUri) as HttpWebRequest;
			xboxComRequest.Method = "POST";
			xboxComRequest.AllowAutoRedirect = false;
			StreamWriter xboxStream = new StreamWriter(xboxComRequest.GetRequestStream());

			xboxStream.Write(postdata);
			xboxStream.Flush();
			xboxComRequest.BeginGetResponse(new AsyncCallback(XboxComCookieSet), xboxComRequest);
		}
		private void XboxComCookieSet(IAsyncResult e)
		{
			HttpWebRequest request = e.AsyncState as HttpWebRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			response.Close();

			// Due to what appears to be a bug in the .net Framework with its ability to parse multiple returned cookies
			// We must parse every cookie separately

			string cookies = response.Headers["Set-Cookie"];

			mCookieJar.Add(new Cookie("RPSAuth", (Regex.Match(cookies, "RPSAuth=(.*?);").Groups[1].Value))
			{
				Domain = ".xbox.com",
				HttpOnly = true
			});

			mCookieJar.Add(new Cookie("RPSSecAuth", (Regex.Match(cookies, "RPSSecAuth=(.*?);").Groups[1].Value))
			{
				Domain = "live.xbox.com",
				HttpOnly = true,
				Secure = true
			});

			mCookieJar.Add(new Cookie("ANON", (Regex.Match(cookies, "ANON=(.*?);").Groups[1].Value))
			{
				Domain = ".xbox.com"
			});

			mCookieJar.Add(new Cookie("NAP", (Regex.Match(cookies, "NAP=(.*?);").Groups[1].Value))
			{
				Domain = ".xbox.com"
			});

			mCookieJar.Add(new Cookie("XBXTkt", (Regex.Match(cookies, "XBXTkt=(.*?);").Groups[1].Value))
			{
				Domain = ".xbox.com",
				HttpOnly = true
			});

			mCookieJar.Add(new Cookie("XBXGt", (Regex.Match(cookies, "XBXGt=(.*?);").Groups[1].Value))
			{
				Domain = ".xbox.com"
			});

			Trace.WriteLine("XboxLive: Login Sequence Complete.");

			HttpWebRequest homepage = Homepage;
			homepage.BeginGetResponse(new AsyncCallback(OnHomepageDownload), homepage);
		}

		/// <summary>
		/// Returns a prepare HttpWebRequest to download the Xbox Live Homepage
		/// </summary>
		private HttpWebRequest Homepage
		{
			get {
				HttpWebRequest request = WebRequest.Create(mXboxLiveHomepage) as HttpWebRequest;
				request.CookieContainer = mCookieJar;

				return request;
			}
		}
		private HttpWebRequest FriendsPage
		{
			get {
				HttpWebRequest request = WebRequest.Create(mXboxLiveFriends) as HttpWebRequest;
				request.CookieContainer = mCookieJar;

				return request;
			}
		}
		private IAsyncResult GetLoginSubmitResponse(Uri loginSubmitUri, Dictionary<string, string> postdata, AsyncCallback callback)
		{
			HttpWebRequest request = WebRequest.Create(loginSubmitUri) as HttpWebRequest;
			request.CookieContainer = mCookieJar;
			request.Method = "POST";
			request.ServicePoint.Expect100Continue = false;
			request.ContentType = "application/x-www-form-urlencoded";
			StreamWriter writer = new StreamWriter(request.GetRequestStream());

			postdata.Add("LoginOptions", "3");
			postdata.Add("login", Username);
			postdata.Add("passwd", Password);
			postdata.Add("SI", "++++Sign+in++++");

			string post = postdata.Select(kv => kv.Key + "=" + kv.Value).Aggregate((kv, v2) => kv + "&" + v2);

			writer.WriteLine(post);
			writer.Flush();

			return request.BeginGetResponse(callback, request);
		}

		private static readonly Uri mXboxLiveHomepage = new Uri("http://live.xbox.com/en-US/default.aspx");
		private static readonly Uri mXboxLiveFriends = new Uri("http://live.xbox.com/en-US/profile/Friends.aspx");
		private static readonly XPathExpression mFriendRootNodeSelect = XPathExpression.Compile("//*[starts-with(@class, 'XbcTableFriend')]");
		private static readonly Regex mLoginPageMatch = new Regex("window.location = \\\"(http://login.live.com/login.srf.*)\\\";");
		private static readonly Regex mActionUriMatch = new Regex("action=\\\"([a-zA-Z0-9:/.?=&%]*)\\\""); // action="(.*?)"
		private static readonly Regex mHiddenFieldMatch = new Regex("input type=\"hidden\".*? name=\"(.*?)\".*? value=\"(.*?)\""); // Matches extra input fields in the login page that need to be sent along with the username/password
		private static readonly Regex mMSPOKCookieMatch = new Regex("(MSPOK=[a-z0-9$-]*)");
		private static readonly Regex mPlayMatch = new Regex("(?:([0-9]{2}/[0-9]{2}/[0-9]{2})|([0-9]*) (minutes|hours) ago) playing (.*)");
		private static readonly string mIsLoggedInMatchKey = "Sign out</a>";
		private CookieContainer mCookieJar; // Tasty cookies
	}
}