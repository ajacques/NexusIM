using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using InstantMessage;

namespace NexusIM.Converters
{
	public sealed class GeneralIMStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IMStatus status = (IMStatus)value;

			switch (status)
			{
				case IMStatus.Available:
					return AvailableBrush;
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private Brush AvailableBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xC1, 0xFF, 0x99));
		private Brush BusyBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xFF, 0xC9, 0xC9));
		private Brush AwayBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xFF, 0xED, 0x99));
		private Brush OfflineBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xDE, 0xDE, 0xDE));
	}
}
