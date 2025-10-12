using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TIMS_X.CloudAssistant.Converters
{
	public class InactiveForegroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value) return new SolidColorBrush(Color.FromRgb(176, 176, 176));
			return new SolidColorBrush(Color.FromRgb(221, 221, 221));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}