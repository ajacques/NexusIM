using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NewsFeedPushServer
{
	internal static class Extensions
	{
		public static string ContentToString(this MemoryStream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}
