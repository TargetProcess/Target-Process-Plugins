using System;

namespace hOOt
{
    internal static class FastDateTime
    {
        public static TimeSpan LocalUtcOffset;

        public static DateTime Now
        {
            get { return DateTime.UtcNow + LocalUtcOffset; }
        }

        static FastDateTime()
        {
            LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        }
    }
}
