using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Converters
{
    public sealed class MinimumAccessRoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role result = Role.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Role>(intValue);
            else if (value is Role typeValue)
                result = typeValue;

            Role resultParameter = Role.Default;

            if (value is int parameterIntValue)
                resultParameter = DatabaseManager.Find<Role>(parameterIntValue);
            else if (value is string parameterStringValue)
                resultParameter = DatabaseManager.CreateContext().Roles.FirstOrDefault(role => role.Name == parameterStringValue);

            if (resultParameter == null)
                return Visibility.Visible;

            if (result == null)
            {
                if (resultParameter.Id == Role.Default.Id)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }

            if (result.IsAdmin)
                return Visibility.Visible;

            if (result.Id >= resultParameter.Id)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
