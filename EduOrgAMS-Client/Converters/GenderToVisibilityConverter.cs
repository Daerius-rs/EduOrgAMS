using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Converters
{
    public sealed class GenderToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gender result = Gender.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Gender>(intValue);
            else if (value is Gender typeValue)
                result = typeValue;

            return result != Gender.Default
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
