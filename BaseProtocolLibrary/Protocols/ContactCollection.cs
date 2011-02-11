using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace InstantMessage.Protocols
{
	public class ContactCollection : ObservableCollection<IContact>
	{
		public IContact this[string name]
		{
			get	{
				return this.FirstOrDefault(i => i.Username == name);
			}
		}
	}
}
