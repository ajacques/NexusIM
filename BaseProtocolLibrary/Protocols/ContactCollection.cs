using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace InstantMessage.Protocols
{
	public class ContactCollection : ObservableCollection<IContact>
	{
		public new void Clear()
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)this.Items));

			base.ClearItems();
		}

		public IContact this[string name]
		{
			get	{
				return this.FirstOrDefault(i => i.Username == name);
			}
		}
	}
}
