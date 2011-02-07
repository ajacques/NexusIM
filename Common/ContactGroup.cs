using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public class ContactGroup
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
		public bool Equals(IContact other)
		{
			throw new NotImplementedException();
		}

		#region IContact Members


		public string Nickname
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}