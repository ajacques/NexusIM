using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using InstantMessage;
using System.Windows.Controls;

namespace NexusIM.Converters
{
	[ValueConversion(typeof(BuddyAvatar), typeof(Image))]
	class IMAvatarToImage : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			BuddyAvatar source = value as BuddyAvatar;

			source.

			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
