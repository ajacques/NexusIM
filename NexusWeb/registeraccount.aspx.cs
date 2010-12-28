using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.UI;
using NexusWeb.Properties;

namespace NexusWeb.Pages
{
	public partial class RegisterAccount : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("http://api.recaptcha.net/js/recaptcha_ajax.js"));
			ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("~/js/registeraccount.js"));

			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("~/Services/ValidationFunctions.svc"));

			if (Request.HttpMethod == "POST")
			{
				BeginCaptchaValidate();

				EndCaptchaValidate();
			}
		}

		private void BeginCaptchaValidate()
		{
			mTestThread = new Thread(new ThreadStart(TestReCaptcha));
			mTestThread.Start();
		}

		private void EndCaptchaValidate()
		{
			mTestThread.Join();
		}

		private void TestReCaptcha()
		{
			string challenge = Request["recaptcha_challenge_field"];
			string response = Request["recaptcha_response_field"];
			string requestip = Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress;

			HttpWebRequest request = HttpWebRequest.Create("http://api-verify.recaptcha.net/verify") as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ServicePoint.Expect100Continue = false;
			
			// Begin POST data upload
			Stream poststream = request.GetRequestStream();
			StreamWriter writer = new StreamWriter(poststream);
			string postdata = String.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}", Settings.Default.RecaptchaPrivateKey, requestip, challenge, response);

			writer.WriteLine(postdata);
			writer.Close();

			HttpWebResponse wresponse = request.GetResponse() as HttpWebResponse;
			StreamReader reader = new StreamReader(wresponse.GetResponseStream());
			mCaptchaResult = Convert.ToBoolean(reader.ReadLine());
			mCaptchaMessage = reader.ReadLine();

			reader.Close();
		}

		private bool mCaptchaResult = false;
		private string mCaptchaMessage;
		private Thread mTestThread;
	}
}