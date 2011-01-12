using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.IO;
using Json;

namespace BaseWindowsProtocolLibrary.Translation
{
	public class GoogleTranslator : ITranslator
	{
		public GoogleTranslator() {}

		public string Translate(string input, CultureInfo inputlang, CultureInfo outputlang)
		{
			string uri = "http://ajax.googleapis.com/ajax/services/language/translate?v=1.0";

			uri += "&langpair=" + inputlang.TwoLetterISOLanguageName;
			uri += Uri.EscapeUriString("|") + outputlang.TwoLetterISOLanguageName;
			uri += "&q=" + Uri.EscapeDataString(input);

			Uri uriDone = new Uri(uri);

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uriDone);

			WebResponse response = request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());

			Hashtable table = (Hashtable)JSON.JsonDecode(reader.ReadToEnd());
			string text = ((Hashtable)table["responseData"])["translatedText"].ToString();

			return text.Trim('"');
		}
	}
}