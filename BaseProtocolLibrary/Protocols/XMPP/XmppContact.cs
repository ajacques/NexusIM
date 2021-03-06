﻿using System;

namespace InstantMessage.Protocols.XMPP
{
	public class XmppContact : IContact
	{
		public XmppContact(XmppProtocol protocol, Jid jid)
		{
			Protocol = protocol;
			Jid = jid;
		}

		public IMProtocol Protocol
		{
			get;
			private set;
		}

		public string Username
		{
			get {
				return Jid.ToString();
			}
		}

		public Jid Jid
		{
			get;
			private set;
		}

		public string Nickname
		{
			get;
			set;
		}

		public string DisplayName
		{
			get {
				return Username;
			}
		}

		public string Group
		{
			get {
				return String.Empty;
			}
		}

		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
			(Protocol as XmppProtocol).SendMessage(this, message);
		}

		public bool Equals(IContact other)
		{
			return other is XmppContact && Jid == ((XmppContact)other).Jid;
		}

		public IMBuddyStatus Status
		{
			get;
			internal set;
		}

		public string StatusMessage
		{
			get {
				return String.Empty;
			}
		}
	}
}
