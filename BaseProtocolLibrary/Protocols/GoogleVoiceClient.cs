using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Google.Voice
{
	internal class LoginAsyncResult : IAsyncResult
	{
		public LoginAsyncResult(object state, AsyncCallback callback, IAsyncResult httpresult)
		{
			mState = state;
			mCallback = callback;
			mInnerResult = httpresult;
			mWaitHandle = new ManualResetEvent(false);
		}

		internal void trigger()
		{
			mWaitHandle.Set();
			if (mCallback != null)
				mCallback(this);
		}

		public object AsyncState
		{
			get {
				return mState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get {
				return mWaitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsCompleted
		{
			get {
				return mCompleted;
			}
			internal set {
				mCompleted = true;
			}
		}

		internal IAsyncResult InnerResult
		{
			get {
				return mInnerResult;
			}
		}

		private object mState;
		private ManualResetEvent mWaitHandle;
		private AsyncCallback mCallback;
		private IAsyncResult mInnerResult;
		private bool mCompleted;
	}
	public class GoogleVoiceClient
	{
		public GoogleVoiceClient() {}

		public IAsyncResult BeginLogin(string username, string password, AsyncCallback callback, object state)
		{
			if (mLoginResult != null)
				throw new InvalidOperationException("Login already in progress");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(mAuthUrl, username, password));
			IAsyncResult hresult = request.BeginGetResponse(new AsyncCallback(onAuth), request);

			IAsyncResult result = new LoginAsyncResult(state, callback, hresult);

			return result;
		}

		public void EndLogin(IAsyncResult e)
		{
			if (mLoginResult != e)
				throw new ArgumentException("Input Async Result does not match internal login result");

			mLoginResult.IsCompleted = true;
		}

		private void onAuth(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string result = reader.ReadToEnd();

			if (response.StatusCode == HttpStatusCode.OK)
			{
				Match match = Regex.Match(result, mAuthRegexMatch, RegexOptions.IgnoreCase);

				mCookies.Add(new Cookie("SID", match.Groups[1].Value, "/", ".google.com"));
				mCookies.Add(new Cookie("LSID", match.Groups[2].Value, "/", ".google.com"));
				mCookies.Add(new Cookie("AUTH", match.Groups[3].Value, "/", ".google.com"));

				mLoginResult.trigger();
			} else if (response.StatusCode == HttpStatusCode.Forbidden) {

			}
		}

		private bool mIsAuthenticated = false;
		private LoginAsyncResult mLoginResult;
		private string mAuthUrl = "https://www.google.com/accounts/ClientLogin?accountType=GOOGLE&Email={0}&Passwd={1}&service=grandcentral&source=NexusIM";
		private string mSmsDownloadUrl = "https://www.google.com/voice/inbox/recent/sms/";
		private CookieCollection mCookies = new CookieCollection();
		private string mAuthRegexMatch = "SID=([a-z0-9_-]*)\nLSID=([a-z0-9_-]*)\nAUTH=([a-z0-9_-]*)";
	}
}
