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

                return TimeUtils.ToDateTime(
                        result <= 0
                            ? 0
                            : result)
                    .ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return TimeUtils.ToDateTime(0)
                    .ToString(CultureInfo.CurrentCulture);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
