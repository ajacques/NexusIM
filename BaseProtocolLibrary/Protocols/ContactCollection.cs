using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace InstantMessage.Protocols
{
	public class ContactCollection : AdvancedSet<IContact>
	{
		public IContact this[string name]
		{
			get	{
				return this.FirstOrDefault(i => i.Username == name);
			}
		}
	}
}