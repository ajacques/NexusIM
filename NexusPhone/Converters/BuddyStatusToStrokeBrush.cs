using System;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Linq;

namespace NexusPhone.Converters
{
	public class BuddyStatusToStrokeBrush : IValueConverter
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

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 220, 247, 253)});
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 133, 225, 57), Offset = 0.5});
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 220, 247, 203), Offset = 1});

			return col;
		}
		private GradientStopCollection GenerateAwayStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 248, 210, 114)});
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 255, 222, 78), Offset = 0.5 });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 248, 210, 114), Offset = 1 });

			return col;
		}
		private GradientStopCollection GenerateBusyStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 215, 126, 117) });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 194, 53, 43), Offset = 0.5 });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 215, 126, 117), Offset = 1 });

			return col;
		}
		private GradientStopCollection GenerateOfflineStops()
		{
			GradientStopCollection col = new GradientStopCollection();

			col.Add(new GradientStop() { Color = Color.FromArgb(255, 235, 242, 243) });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 217, 229, 232), Offset = 0.5 });
			col.Add(new GradientStop() { Color = Color.FromArgb(255, 235, 242, 243), Offset = 1 });

			return col;
		}
	}
}