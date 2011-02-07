using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using NexusIM.Controls;
using System.Collections;

namespace NexusIM.Converters
{
	public class ContactToUIContact : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerator iterator = ((IEnumerable)value).GetEnumerator();
			ArrayList list = new ArrayList();

			while (iterator.MoveNext())
			{
				ContactListItem item = new ContactListItem();
				item.DataContext = iterator.Current;

				list.Add(item);
			}

			return list;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
