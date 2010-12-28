using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Xml;
using Google.Voice;

namespace InstantMessage
{
	public class IMGoogleVoiceProtocol : IMProtocol
	{
		public IMGoogleVoiceProtocol()
		{
			protocolType = "Google Voice";
			mProtocolTypeShort = "gvoice";
			client = new GoogleVoiceClient();
		}
		public override void BeginLogin()
		{
			client.BeginLogin(Username, Password, new AsyncCallback(onPLogin), null);			
		}

		private void onPLogin(IAsyncResult e)
		{

		}

		private GoogleVoiceClient client;
	}
}