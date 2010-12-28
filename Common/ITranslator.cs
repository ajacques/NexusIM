using System;
using System.Globalization;

namespace BaseWindowsProtocolLibrary.Translation
{
	public interface ITranslator
	{
		string Translate(string content, CultureInfo inputlang, CultureInfo outputlang);
		//IAsyncResult BeginTranslate(string content, CultureInfo inputlang, CultureInfo outputlang, AsyncCallback callback, object object);
	}
}