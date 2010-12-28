using System;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace NexusPhone.Converters
{
	public class FontShrinkerConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return 56;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
