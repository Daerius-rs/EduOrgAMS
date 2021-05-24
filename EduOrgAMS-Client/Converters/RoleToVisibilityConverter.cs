using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Converters
{
    public sealed class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role result = Role.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Role>(intValue);
            else if (value is Role typeValue)
                result = typeValue;

            return result != Role.Default
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
