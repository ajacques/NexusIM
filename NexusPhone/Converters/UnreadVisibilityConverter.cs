using System;
using System.Windows.Data;
using System.Windows;
using System.Collections;
using System.Linq;

namespace NexusPhone.Converters
{
	public class GenericVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int)
				return (int)value >= 1 ? Visibility.Visible : Visibility.Collapsed;
			if (value is string)
				return String.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
			else if (value == null)
				return Visibility.Collapsed;
			else if (value is IEnumerable)
				return ((IEnumerable)value).Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
			else
				throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
