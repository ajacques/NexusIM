using System;

namespace InstantMessage.Protocols.XMPP
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class ReadableXmppMessageAttribute : Attribute
	{
		public ReadableXmppMessageAttribute(string ns, string localname)
		{
			Namespace = ns;
			LocalName = localname;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public string LocalName
		{
			get;
			private set;
		}
	}
}
