using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;
using System.Windows.Controls;
using InstantMessage;

namespace NexusIM.Converters
{
	public class ProtocolToComboBox : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new ArrayList();

			IList<IMProtocolExtraData> source = (IList<IMProtocolExtraData>)value;

			ArrayList output = new ArrayList();

			foreach (IMProtocolExtraData protocol in source.Where(i => i.Enabled))
			{
				ListViewItem item = new ListViewItem();
				item.Content = protocol.Protocol.Username;
				item.Tag = protocol;

				output.Add(item);
			}

			return output;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
