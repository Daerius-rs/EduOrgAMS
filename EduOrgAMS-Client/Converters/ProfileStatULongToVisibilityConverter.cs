using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EduOrgAMS.Client.Converters
{
    public sealed class ProfileStatULongToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ulong result = 0;

            if (value is ulong intValue)
                result = intValue;

            return result != 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
