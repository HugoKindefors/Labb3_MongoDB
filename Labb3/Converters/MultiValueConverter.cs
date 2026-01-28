using System;
using System.Globalization;
using System.Windows.Data;

namespace Labb3.Converters
{
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return string.Empty;

            if (parameter is string format)
            {
                return string.Format(culture, format, values);
            }

            return string.Join(" ", values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

