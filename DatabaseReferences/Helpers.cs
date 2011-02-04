using System;
using System.Text;

public class PasswordGenerator
{
	public static string RandomString(int length)
	{
		string legalChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXZY0123456789-/_+";
		StringBuilder sb = new StringBuilder();
		Random r = new Random();

		for (int i = 0; i < length; i++)
			sb.Append(legalChars.Substring(r.Next(0, legalChars.Length - 1), 1));

		return sb.ToString();
	}
}