using System;
using System.Globalization;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Converters
{
    public sealed class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role result = Role.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Role>(intValue);
            else if (value is Role typeValue)
                result = typeValue;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;

            if (value is int intValue)
                result = intValue;
            else if (value is Role typeValue)
                result = typeValue.Id;

            return result;
        }
    }
}
