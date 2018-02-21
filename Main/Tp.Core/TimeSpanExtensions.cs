using System.Linq.Dynamic;

namespace System
{
    public static class TimeSpanExtensions
    {
        [PublicApiMethod]
        public static double? TotalDays(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalDays);
        }

        [PublicApiMethod]
        public static double? TotalHours(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalHours);
        }

        [PublicApiMethod]
        public static double? TotalMilliseconds(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalMilliseconds);
        }

        [PublicApiMethod]
        public static double? TotalMinutes(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalMinutes);
        }

        [PublicApiMethod]
        public static double? TotalSeconds(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalSeconds);
        }

        [PublicApiMethod]
        public static double? TotalMonths(this TimeSpan? timeSpan)
        {
            return timeSpan.Select(x => x.TotalDays / 30);
        }
    }
}
