using System;
using System.Text;

namespace NexusPhone
{
	/// <summary>
	/// Less advanced version of the Guid class. Allows for smaller Guid strings.
	/// </summary>
	class BasicGuid
	{
		public static BasicGuid NewGuid()
		{
			BasicGuid guid = new BasicGuid();
			guid.guid = RandomString(20);

			return guid;
		}
		public override string ToString()
		{
			return guid;
		}
		private static string RandomString(int length)
		{
			string legalChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXZY0123456789-_";
			StringBuilder sb = new StringBuilder();
			Random r = new Random();

			for (int i = 0; i < length; i++)
				sb.Append(legalChars.Substring(r.Next(0, legalChars.Length - 1), 1));

			return sb.ToString();
		}

		private string guid;
	}
}