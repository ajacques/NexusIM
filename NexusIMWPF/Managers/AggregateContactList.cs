using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using InstantMessage;
using System.Collections.ObjectModel;

namespace NexusIM.Managers
{
	public static class AggregateContactList
	{
		static AggregateContactList()
		{
			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);

			ContactList = new ObservableCollection<IContact>();
			Groups = new ObservableCollection<GroupOfContacts>();
		}

		public static ObservableCollection<IContact> ContactList
		{
			get;
			private set;
		}
		public static ObservableCollection<GroupOfContacts> Groups
		{
			get;
			private set;
		}

		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (IContact contact in e.NewItems)
			{
				GroupOfContacts group = Groups.FirstOrDefault(g => g.GroupName == contact.Group);
				if (group == null)
				{
					group = new GroupOfContacts();
					group.GroupName = contact.Group;
					Groups.Add(group);
				}
				group.Contacts.Add(contact);
			}
		}
	}
}
