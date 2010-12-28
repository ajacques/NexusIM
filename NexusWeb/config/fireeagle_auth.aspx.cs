using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using NexusWeb.Properties;
using OAuth;
using System.IO;
using Yahoo.FireEagle;

namespace NexusWeb.w
{
	public partial class fireeagle_auth : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["gotrequesttoken"] == null)
			{
				//FireEaglef feagle = new FireEaglef(Settings.Default.FireEagle_ConsumerKey, Settings.Default.FireEagle_ConsumerSecret);
				FireEagle feagle = new FireEagle();
				feagle.GetRequestToken();
				//feagle.GetRequestToken("http://localhost/NexusWeb/fireeagle_auth.aspx?gotrequesttoken");
			}

			//HttpWebRequest request = WebRequest.Create(Settings.Default.FireEagle_UnauthorizedRequestToken) as HttpWebRequest;
			//request.Method = "POST";
			//request.ContentType = "application/x-www-form-urlencoded";
			//string postdata = GenerateUnauthorizedRequestPOSTData();

			//Stream upstream = request.GetRequestStream();
			//StreamWriter writer = new StreamWriter(upstream);			

			//writer.WriteLine(postdata);
			//writer.Close();

			//HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			//Stream downstream = response.GetResponseStream();
			//StreamReader reader = new StreamReader(downstream);
			//string data = reader.ReadToEnd();
		}
	}
}