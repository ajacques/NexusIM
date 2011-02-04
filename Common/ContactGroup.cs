using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public class ContactGroup : IContact, IHasPresence
	{
		public ContactGroup()
		{

		}

		public void SendMessage(string message)
		{
			throw new NotImplementedException();
		}

		public string Username
		{
			get {
				throw new NotImplementedException();
			}
		}

		public IMBuddyStatus Status
		{
			get { throw new NotImplementedException(); }
		}

		public string StatusMessage
		{
			get { throw new NotImplementedException(); }
		}

		public IMProtocol Protocol
		{
			get { throw new NotImplementedException(); }
		}
	}
}