using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace NexusPhone
{
	public static class ExtensionMethods
	{
		public static string ValueToString(this JToken source)
		{
			return ((JValue)source).Value.ToString();
		}
	}
}
