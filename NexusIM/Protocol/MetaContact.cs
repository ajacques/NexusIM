using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InstantMessage;

namespace NexusIM.Protocol
{
	class MetaContact// : IContact
	{
		public MetaContact()
		{
			
		}

		public ReadOnlyCollection<IMBuddy> Contacts
		{
			get {
				return new ReadOnlyCollection<IMBuddy>(contacts);
			}
		}
		public string DisplayName
		{
			get {
				return "";
			}
			set {

			}
		}
		public string StatusMessage
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}
		public IMBuddyStatus Status
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}

		private List<IMBuddy> contacts = new List<IMBuddy>();

		public void sendMessage(string message)
		{
			throw new NotImplementedException();
		}
	}
}
