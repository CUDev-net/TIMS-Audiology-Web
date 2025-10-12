using System;
using System.Globalization;
using System.Windows.Data;

namespace TIMS_X.CloudAssistant.Converters
{
	public class NoahButtonTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value
				? "Use Noah Database"
				: "Use TIMS Database";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}