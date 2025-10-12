using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TIMS_X.CloudAssistant.Converters
{
	public class LoadTimeColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var loadTime = (double)value;
			const double threshold = 10.0;
			return loadTime < threshold
				? new SolidColorBrush(Colors.Green)
				: new SolidColorBrush(Colors.Red);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}