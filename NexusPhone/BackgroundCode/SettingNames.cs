using System;
using System.IO.IsolatedStorage;

namespace NexusPhone
{
	static class DefinedSettings
	{
		public static string CoreSessionId
		{
			get	{
				if (IsolatedStorageSettings.ApplicationSettings.Contains("NXSessionId"))
					return (string)IsolatedStorageSettings.ApplicationSettings["NXSessionId"];
				else
					return null;
			}
			set	{
				IsolatedStorageSettings.ApplicationSettings["NXSessionId"] = value;
			}
		}
	}
}