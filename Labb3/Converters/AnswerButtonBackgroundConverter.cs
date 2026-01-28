using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Labb3.Converters
{
    public class AnswerButtonBackgroundConverter : IMultiValueConverter
    {
        private static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return DefaultBrush;

            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return DefaultBrush;

            if (values[0] is not bool isAnswerRevealed ||
                values[1] is not int correctIndex ||
                parameter == null ||
                !int.TryParse(parameter.ToString(), out int buttonIndex))
            {
                return DefaultBrush;
            }

            return isAnswerRevealed && correctIndex == buttonIndex ? GreenBrush : DefaultBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

