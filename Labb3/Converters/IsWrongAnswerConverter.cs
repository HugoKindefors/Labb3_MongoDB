using System;
using System.Globalization;
using System.Windows.Data;

namespace Labb3.Converters
{
    public class IsWrongAnswerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3) return false;
            
            if (values[0] is bool isRevealed &&
                values[1] is int selectedIndex &&
                values[2] is int correctIndex &&
                parameter is string answerIndexStr &&
                int.TryParse(answerIndexStr, out int answerIndex))
            {
                return isRevealed && selectedIndex == answerIndex && correctIndex != answerIndex;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

