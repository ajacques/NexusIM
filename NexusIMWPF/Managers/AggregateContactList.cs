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
		public static void Setup()
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
			if (e.NewItems != null)
			{
				foreach (IMProtocolExtraData protocol in e.NewItems)
					protocol.Protocol.ContactList.CollectionChanged += new NotifyCollectionChangedEventHandler(ContactList_CollectionChanged);
			}
			
		}
		private static void ContactList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
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
					ContactList.Add(contact);
					group.Contacts.Add(contact);
				}
			}
		}
	}
}