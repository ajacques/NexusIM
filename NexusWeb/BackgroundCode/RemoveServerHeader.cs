using System;
using System.Web;

namespace NexusWeb
{
	public class RemoveServerHeader : IHttpModule
	{
		/// <summary>
		/// You will need to configure this module in the web.config file of your
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: http://go.microsoft.com/?linkid=8101007
		/// </summary>
		public void Dispose()
		{
			//clean-up code here.
		}

		public void Init(HttpApplication context)
		{
			// Below is an example of how you can handle LogRequest event and provide 
			// custom logging implementation for it
			context.PreSendRequestHeaders += new EventHandler(context_PreSendRequestHeaders);
		}

		private void context_PreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpContext.Current.Response.Headers.Remove("Server");
		}
	}
}