﻿using System;
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

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ContactListGroup.xaml
	/// </summary>
	public partial class ContactListGroup : UserControl, INotifyPropertyChanged
	{
		public ContactListGroup()
		{
			this.InitializeComponent();

			this.MouseDoubleClick += new MouseButtonEventHandler(ContactListGroup_MouseDoubleClick);
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

		private void AddContacts(IEnumerable contacts)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				foreach (IContact contact in contacts)
				{
					ContactListItem item = new ContactListItem();
					item.DataContext = contact;
					item.ContextMenu = new ContactItemContextMenu(contact);
					item.MouseDoubleClick += new MouseButtonEventHandler(ContactListItem_MouseDoubleClick);
					ContactList.Children.Add(item);
				}
			}));
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