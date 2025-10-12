using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TIMS_X.CloudAssistant.Converters
{
	public class BoolColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isSelected = (bool)value;

			return isSelected
				? new SolidColorBrush(Colors.WhiteSmoke)
				: new SolidColorBrush(Color.FromRgb(143, 148, 155));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}