using System;
using System.Globalization;
using System.Windows.Data;
using ImageFilters.Enums;

namespace ImageFilters.Converters
{
    public class RadioBoolToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FilterTypeEnum result;
            Enum.TryParse(parameter.ToString(), out result);
            return value.Equals(result);
                
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

    }
}
