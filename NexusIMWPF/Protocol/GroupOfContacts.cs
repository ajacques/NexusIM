using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;
using System.ComponentModel;
using System.Collections;

namespace InstantMessage
{
	public class GroupOfContacts : IEnumerable<IMBuddy>, INotifyPropertyChanged
	{


		public IEnumerator<IMBuddy> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
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