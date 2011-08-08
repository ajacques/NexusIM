using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using InstantMessage.Protocols;
using InstantMessage.Protocols.Irc;

namespace NexusIM.Protocol
{
	class SelfContact : IContact
	{
		public SelfContact(IMProtocol protocol)
		{
			Protocol = protocol;
		}

		public IMProtocol Protocol
		{
			get;
			private set;
		}

		public string Username
		{
			get {
				return Protocol.Username;
			}
		}
		public string DisplayName
		{
			get	{
				return Username;
			}
		}

		public string Nickname
		{
			get {
				if (Protocol is IRCProtocol)
					return ((IRCProtocol)Protocol).Nickname;
				return Protocol.Username;
			}
			set {
				
			}
		}

		public string Group
		{
			get { throw new NotImplementedException(); }
		}
		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
		}

		public bool Equals(IContact other)
		{
			throw new NotImplementedException();
		}

		public IMBuddyStatus Status
		{
			get { throw new NotImplementedException(); }
		}

		public string StatusMessage
		{
			get { throw new NotImplementedException(); }
		}
	}
}
