using System;
using System.Globalization;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Extensions;

namespace EduOrgAMS.Client.Converters
{
    public sealed class RoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role result = Role.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Role>(intValue);
            else if (value is Role typeValue)
                result = typeValue;

            return result.GetLocalizedName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Role result = Role.Default;

            if (value is string stringValue)
                result = Role.Default.ParseLocalizedName(stringValue);

            return result;
        }
    }
}
