using System;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Linq;

namespace NexusPhone.Converters
{
	public class BuddyStatusToFillBrush : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is IMBuddyStatus))
				throw new NotSupportedException();

			if (targetType != typeof(Brush))
				throw new NotSupportedException();

			IMBuddyStatus status = (IMBuddyStatus)value;
			GradientBrush brush = new LinearGradientBrush();

			switch (status)
			{
				case IMBuddyStatus.Available:
					brush.GradientStops = GenerateAvailableStops();
					break;
				case IMBuddyStatus.Away:
					brush.GradientStops = GenerateAwayStops();
					break;
				case IMBuddyStatus.Busy:
					brush.GradientStops = GenerateBusyStops();
					break;
				case IMBuddyStatus.Offline:
					brush.GradientStops = GenerateOfflineStops();
					break;
			}
			
			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private GradientStopCollection GenerateAvailableStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 0x98, 0xdc, 0x89)});
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 0x1c, 0xb1, 0x04), Offset = 0.5});
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 0x7d, 0xd7, 0x59), Offset = 1});

			return col;
		}
		private GradientStopCollection GenerateAwayStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 238, 165) });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 222, 78), Offset = 0.5 });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 238, 165), Offset = 1 });

			return col;
		}
		private GradientStopCollection GenerateBusyStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 194, 58, 43) });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 236, 115, 86), Offset = 1 });

			return col;
		}
		private GradientStopCollection GenerateOfflineStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 215, 228, 231) });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 243, 247, 248), Offset = 1 });

			return col;
		}
	}
}