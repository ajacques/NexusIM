using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InstantMessage.Protocols.XMPP
{
	public sealed class Jid : IEquatable<Jid>
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

			result = new Jid(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
			return true;
		}

		// IEquatable<Jid>
		public bool Equals(Jid other)
		{
			return Equals(other, false);
		}
		public bool Equals(Jid other, bool ignoreResource)
		{
			return this.Server == other.Server && this.Username == other.Username && (ignoreResource || this.Resource == other.Resource);
		}

		// Object
		public override bool Equals(object obj)
		{
			return Equals(obj as Jid);
		}
		public override int GetHashCode()
		{
			return Username.GetHashCode() ^ Server.GetHashCode() ^ Resource.GetHashCode();
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}@{1}", Username, Server);

			if (!String.IsNullOrEmpty(Resource))
				sb.AppendFormat("/{0}", Resource);

			return sb.ToString();
		}

		public static bool operator ==(Jid left, Jid right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Jid left, Jid right)
		{
			return !left.Equals(right);
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
