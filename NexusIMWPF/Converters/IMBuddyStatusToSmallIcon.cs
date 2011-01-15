using System;
using System.Globalization;
using System.Windows.Data;
using InstantMessage;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NexusIM.Converters
{
	[ValueConversion(typeof(IMBuddyStatus), typeof(ImageSource))]
	public class IMBuddyStatusToSmallIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IMBuddyStatus status = (IMBuddyStatus)value;
			Uri source;

			switch (status)
			{
				case IMBuddyStatus.Available:
					source = new Uri("/NexusIMWPF;component/Resources/available_icon.ico", UriKind.Relative);
					break;
				default:
					source = new Uri("/NexusIMWPF;component/Resources/offline_icon.ico", UriKind.Relative);
					break;
			}

			return new BitmapImage(source);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
