using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using InstantMessage;
using NexusIM.Misc;
using System.Collections.Generic;

namespace NexusIM.Managers
{
	internal static class AggregateContactList
	{
		public static void Setup()
		{
			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);

			ContactList = new AdvancedSet<IContact>(new ContactComparer());
			Groups = new AdvancedSet<GroupOfContacts>(new GroupComparer()); //new AdvancedSet<GroupOfContacts>();
		}

		public static AdvancedSet<IContact> ContactList
		{
			get;
			private set;
		}
		public static AdvancedSet<GroupOfContacts> Groups
		{
			get;
			private set;
		}

		// Nested Classes
		private class ContactComparer : IComparer<IContact>
		{
			public int Compare(IContact x, IContact y)
			{
				if (x.Protocol != y.Protocol)
					return x.Protocol.CompareTo(y.Protocol);

				return String.Compare(x.Username, y.Username, StringComparison.CurrentCulture);
			}
		}
		private class GroupComparer : IComparer<GroupOfContacts>
		{
			public int Compare(GroupOfContacts x, GroupOfContacts y)
			{
				return x.GroupName.CompareTo(y.GroupName);
			}
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
				foreach (KeyValuePair<String, IContact> contact in e.NewItems)
				{
					string groupname = contact.Value.Group;
					if (String.IsNullOrEmpty(contact.Value.Group))
						groupname = "Friends";

					GroupOfContacts group = Groups.FirstOrDefault(g => g.GroupName == groupname);
					if (group == null)
					{
						group = new GroupOfContacts();
						group.GroupName = groupname;
						Groups.Add(group);
					}
					ContactList.Add(contact.Value);
					group.Contacts.Add(contact.Value);
				}
			}
			if (e.OldItems != null)
			{
				foreach (KeyValuePair<String, IContact> contact in e.OldItems)
				{
					string groupname = contact.Value.Group;
					if (String.IsNullOrEmpty(contact.Value.Group))
						groupname = "Friends";

					GroupOfContacts group = Groups.FirstOrDefault(g => g.GroupName == groupname);
					if (group != null)
					{
						group.Contacts.Remove(contact.Value);
					}
					ContactList.Remove(contact.Value);
				}
			}
		}
	}
}