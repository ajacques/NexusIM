using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using InstantMessage;

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
				foreach (IMProtocolWrapper protocol in e.NewItems)
					protocol.Protocol.ContactList.CollectionChanged += new NotifyCollectionChangedEventHandler(ContactList_CollectionChanged);
			}
			
		}
		private static void ContactList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (IContact contact in e.NewItems)
				{
					string groupname = contact.Group;
					if (String.IsNullOrEmpty(contact.Group))
						groupname = "Friends";

					GroupOfContacts group = Groups.FirstOrDefault(g => g.GroupName == groupname);
					if (group == null)
					{
						group = new GroupOfContacts();
						group.GroupName = groupname;
						Groups.Add(group);
					}
					ContactList.Add(contact);
					group.Contacts.Add(contact);
				}
			}
			if (e.OldItems != null)
			{
				foreach (IContact contact in e.OldItems)
					ContactList.Remove(contact);
			}
		}
	}
}