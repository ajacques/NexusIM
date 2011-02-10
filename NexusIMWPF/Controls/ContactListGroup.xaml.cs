using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using InstantMessage;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.Windows.Media;

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

					NotifyPropertyChanged("IsExpanded");
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
					ContactList.Children.Add(item);
				}
			}));	
		}

		private void Contacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			AddContacts(e.NewItems);			
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;

			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		private bool mIsExpanded;
	}
}