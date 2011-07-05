using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace InstantMessage.Protocols
{
	public class ContactCollection : AdvancedSet<string, IContact>
	{
		public void Add(IContact contact)
		{
			base.Add(contact.Username, contact);
		}
	}
}