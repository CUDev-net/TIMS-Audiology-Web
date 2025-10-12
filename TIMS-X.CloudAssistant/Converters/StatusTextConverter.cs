using System;
using System.Globalization;
using System.Windows.Data;

namespace TIMS_X.CloudAssistant.Converters
{
	public class StatusTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var running = (bool)value;

			return running
				? "Running"
				: "Off";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}