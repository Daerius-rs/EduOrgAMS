using System;
using System.Globalization;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Extensions;

namespace EduOrgAMS.Client.Converters
{
    public sealed class GenderToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gender result = Gender.Default;

            if (value is int intValue)
                result = DatabaseManager.Find<Gender>(intValue);
            else if (value is Gender typeValue)
                result = typeValue;

            return result.GetLocalizedName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gender result = Gender.Default;

            if (value is string stringValue)
                result = Gender.Default.ParseLocalizedName(stringValue);

            return result;
        }
    }
}
