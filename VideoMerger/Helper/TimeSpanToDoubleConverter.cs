using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoMerger.Helper
{
    internal class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
                return ((TimeSpan)value).TotalMilliseconds;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && d > 0)
                return TimeSpan.FromMilliseconds(d);
            return DependencyProperty.UnsetValue;
        }
    }
}
