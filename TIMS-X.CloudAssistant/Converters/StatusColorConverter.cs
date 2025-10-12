using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TIMS_X.CloudAssistant.Converters
{
	public class StatusColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var enabled = (bool)value;

			return enabled
				? new SolidColorBrush(Colors.Green)
				: new SolidColorBrush(Colors.Red);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}