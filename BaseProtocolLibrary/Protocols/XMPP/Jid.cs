using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InstantMessage.Protocols.XMPP
{
	sealed class Jid
	{
		static Jid()
		{
			parseRegex = new Regex("([^@]+)@([^/]+)(?:/(.+))?");
		}
		public Jid(string username, string server) : this(username, server, null) {}
		public Jid(string username, string server, string resource)
		{
			Username = username;
			Server = server;
			Resource = resource;
		}

		public static Jid Parse(string input)
		{
			Jid result;
			if (TryParse(input, out result))
				return result;

			throw new FormatException("The input is not a Jid.");
		}
		public static bool TryParse(string input, out Jid result)
		{
			result = null;
			Match match = parseRegex.Match(input);

			if (!match.Success)
				return false;

			result = new Jid(match.Groups[0].Value, match.Groups[1].Value, match.Groups[2].Value);
			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}@{1}", Username, Server);

			if (Resource != null)
				sb.AppendFormat("/{0}", Resource);

			return sb.ToString();
		}

		public string Username
		{
			get;
			private set;
		}
		public string Server
		{
			get;
			private set;
		}
		public string Resource
		{
			get;
			private set;
		}

		private static Regex parseRegex;
	}
}
