using System;
using System.Windows.Data;
using System.Collections;
using System.Linq;

namespace NexusPhone.Converters
{
	public class EnumerableCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is IEnumerable))
				throw new NotSupportedException();

			IEnumerable col = value as IEnumerable;
			return col.Cast<object>().Count().ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
