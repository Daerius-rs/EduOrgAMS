using System;
using System.Globalization;
using System.Windows.Data;
using EduOrgAMS.Client.Utils;

namespace EduOrgAMS.Client.Converters
{
    public sealed class UnixTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                ulong result = 0;

                if (value is ulong longValue)
                    result = longValue;

                return TimeUtils.ToDateTime(result)
                    .ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return TimeUtils.ToDateTime(0)
                    .ToString(CultureInfo.InvariantCulture);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string result = string.Empty;

                if (value is string stringValue)
                    result = stringValue;

                return TimeUtils.ToUnixTimeStamp(DateTime.Parse(
                    result, CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
                string result = TimeUtils.ToDateTime(0)
                    .ToString(CultureInfo.InvariantCulture);

                return TimeUtils.ToUnixTimeStamp(DateTime.Parse(
                    result, CultureInfo.InvariantCulture));
            }
        }
    }
}
