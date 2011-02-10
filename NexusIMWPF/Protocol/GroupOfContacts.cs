using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace InstantMessage
{
	public class GroupOfContacts : INotifyPropertyChanged
	{
		public GroupOfContacts()
		{
			Contacts = new ObservableCollection<IContact>();
		}

		public ObservableCollection<IContact> Contacts
		{
			get;
			private set;
		}
		public string GroupName
		{
			get	{
				return mGroupName;
			}
			set	{
				if (mGroupName != value)
				{
					mGroupName = value;

					NotifyPropertyChanged("GroupName");
				}
			}
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private string mGroupName;
	}
}