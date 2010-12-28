using System;
using System.Windows.Data;
using NexusPhone.UserInterface;
using System.Linq;
using System.Collections;

namespace NexusPhone.Converters
{
	public class MessageToUIMessage : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((IEnumerable)value).Cast<IMMessage>().Select(im => new UIChatMessage() { DataContext = im });
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
