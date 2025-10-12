using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TIMS_X.CloudAssistant.Converters
{
	public class BoolVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isVisible = (bool)value;

			return isVisible
				? Visibility.Visible
				: Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}