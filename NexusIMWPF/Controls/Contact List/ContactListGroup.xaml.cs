using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using NexusIM.Managers;
using System.Collections.Generic;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ContactListGroup.xaml
	/// </summary>
	public partial class ContactListGroup : UserControl, INotifyPropertyChanged, IDisposable
	{
		public ContactListGroup()
		{
			this.InitializeComponent();

			this.MouseDoubleClick += new MouseButtonEventHandler(ContactListGroup_MouseDoubleClick);
		}

		public void Dispose()
		{
			SourceGroup.Contacts.CollectionChanged -= new NotifyCollectionChangedEventHandler(Contacts_CollectionChanged);
		}

		public GroupOfContacts SourceGroup
		{
			get	{
				return (GroupOfContacts)DataContext;
			}
			set	{
				DataContext = value;
				value.Contacts.CollectionChanged += new NotifyCollectionChangedEventHandler(Contacts_CollectionChanged);
				AddContacts(value.Contacts);
			}
		}
		public bool IsExpanded
		{
			get	{
				return mIsExpanded;
			}
			set	{
				if (mIsExpanded != value)
				{
					Storyboard animStory;
					if (value)
						animStory = FindResource("AnimIn") as Storyboard;
					else
						animStory = FindResource("AnimOut") as Storyboard;
					animStory.Begin();

					mIsExpanded = value;

					ContactList.Height = mIsExpanded ? Double.NaN : 0;

					NotifyPropertyChanged("IsExpanded");
				}
			}
		}

		internal void DeselectAllExcept(UIElement exception)
		{
			foreach (UIElement elem in ContactList.Children)
			{
				if (elem is ContactListItem)
				{
					ContactListItem item = elem as ContactListItem;
					if (item.Selected && item != exception)
						item.Deselect();
				}				
			}
		}

		private sealed class UsernameComparer : IComparer<IContact>
		{
			public int Compare(IContact x, IContact y)
			{
				return x.Username.CompareTo(y.Username);
			}
		}
		private sealed class StatusComparer : IComparer<IContact>
		{
			public int Compare(IContact x, IContact y)
			{
				if (x.Status == y.Status)
					return x.Username.CompareTo(y.Username);
				return x.Status.CompareTo(y.Status);
			}
		}

		private void AddContacts(IEnumerable contacts)
		{
			IComparer<IContact> comparer = new UsernameComparer();
			Dispatcher.InvokeIfRequired(() =>
			{
				foreach (IContact contact in contacts)
				{
					ContactListItem item = new ContactListItem();
					item.Contact = contact;
					item.ContextMenu = new ContactItemContextMenu();
					item.MouseDoubleClick += new MouseButtonEventHandler(ContactListItem_MouseDoubleClick);

					int pos;
					for (pos = 0; pos < ContactList.Children.Count; pos++)
					{
						ContactListItem sitem = (ContactListItem)ContactList.Children[pos];
						if (comparer.Compare(contact, sitem.Contact) < 1)
							break;
					}
					ContactList.Children.Insert(pos, item);
				}
			}); // Don't use async because we have a reader lock right now and we can't let it go because another thread might try to write to it
		}

		private void ContactListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			ContactListItem item = sender as ContactListItem;
			IMBuddy contact = item.DataContext as IMBuddy;

			ThreadPool.QueueUserWorkItem((state) => {
				WindowSystem.OpenContactWindow(contact, true);
			});
		}

		private void Contacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				AddContacts(e.NewItems);

			if (e.OldItems != null)
			{
				foreach (IContact contact in e.OldItems)
				{

				}
			}
		}
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		private void ContactListGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Point p = e.GetPosition(ClickArea);

			if (p.Y < ClickArea.ActualHeight)
				IsExpanded = !IsExpanded;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		private bool mIsExpanded;
	}
}