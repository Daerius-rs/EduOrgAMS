using System;

namespace EduOrgAMS.Client.Utils
{
    public static class TimeUtils
    {
        public static DateTime ToDateTime(ulong unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static ulong ToUnixTimeStamp(DateTime time)
        {
            return (ulong)time
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                .TotalSeconds;
        }
    }
}
