using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using InstantMessage.Protocols;

namespace NexusIM.Protocol
{
	class SelfContact : IContact
	{
		public SelfContact(IMProtocol protocol)
		{
			mProtocol = protocol;
		}

		public IMProtocol Protocol
		{
			get { throw new NotImplementedException(); }
		}

		public string Username
		{
			get {
				return mProtocol.Username;
			}
		}

		public string Nickname
		{
			get {
				return mProtocol.Username;
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

		private IMProtocol mProtocol;
	}
}
