using System.Data;
using System.Globalization;
using System.Linq.Dynamic;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Internationalization;
using Tp.I18n;

// ReSharper disable CheckNamespace

namespace System
// ReSharper restore CheckNamespace
{
    [LocalizationScope("field-names")]
    public static class DateTimeExtensions
    {
        public static IFormattedMessage NoDate => "No Date".Localize();
        public static IFormattedMessage Future => "Future".Localize();
        public static IFormattedMessage Past => "Past".Localize();
        private const string FORMATE_DATE_STRING = "dd MMM";

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return dateTime;
            }
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        public static string GetDateName(this DateTime date) => date.ToString(FORMATE_DATE_STRING);

        public static DateTime GetWeekFirstDay(this DateTime date)
        {
            int sundayDifference = -(int) date.DayOfWeek;
            return date.AddDays(sundayDifference).Date;
        }

        public static DateTime GetWeekLastDay(this DateTime date)
        {
            int saturdayDifference = 6 - (int) date.DayOfWeek;
            return date.AddDays(saturdayDifference).Date;
        }

        public static string GetPartWeekName(this DateTime weekStartDate, DateTime weekEndDate)
            => $"{weekStartDate.ToString(FORMATE_DATE_STRING)} - {weekEndDate.ToString(FORMATE_DATE_STRING)}";

        public static string GetWeekName(this DateTime date) => GetPartWeekName(date.GetWeekFirstDay(), date.GetWeekLastDay());

        public static long ToJavascriptDate(this DateTime date)
        {
            return Convert.ToInt64(
                date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        public static TimeSpan Days(this int days)
        {
            return TimeSpan.FromDays(days);
        }

        public static DateTime SubtractDays(this DateTime dateTime, int days)
        {
            return dateTime.Subtract(TimeSpan.FromDays(days));
        }

        /// <summary>
        /// This is not ISO 8601 compliant. See WeekNumberIso() method.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [PublicApiMethod]
        [SqlFunction("dbo.f_WeekNumber", DbType.UInt32)]
        public static int WeekNumber(this DateTime date)
        {
            return ((int) Math.Floor((date - new DateTime(1990, 1, 1)).TotalDays)) / 7;
        }

        [SqlFunction("dbo.f_Date", DbType.Date)]
        [DynamicExpressionAlias("Day")]
        [PublicApiMethod]
        public static DateTime? Date(this DateTime? date) => date.Select(x => x.Date);

        [SqlFunction("dbo.f_ForecastEndDate", DbType.Date)]
        public static DateTime? ForecastEndDate(this DateTime? startDate, DateTime now, decimal? progress)
        {
            if (progress == null || progress.Value <= 0.01m || progress.Value >= 1m || startDate == null || startDate > now)
            {
                return null;
            }
            if (progress <= 0.1m)
            {
                return null;
            }
            TimeSpan passedDuration = now - startDate.Value;
            if (passedDuration.TotalDays <= 7)
            {
                return null;
            }
            decimal stepInMs = (decimal) (passedDuration.TotalMilliseconds * 0.01) / progress.Value;
            decimal forecastDurationInMs = stepInMs * (1 - progress.Value) * 100;
            TimeSpan forecastDuration = TimeSpan.FromMilliseconds((long) Math.Ceiling(forecastDurationInMs));
            DateTime predictedEndDate = now + forecastDuration;
            return predictedEndDate;
        }

        [SqlDateFunction("dbo.f_DateDiffInSeconds", DbType.Int32)]
        public static int DateDiffInSeconds(this DateTime? date, DateTime? otherDate)
        {
            return (int) (otherDate - date).GetValueOrDefault().TotalSeconds;
        }

        [PublicApiMethod("Used in CCFs")]
        public static DateTime? AddDays(this DateTime? date, double days) => date?.AddDays(days);

        public static DateTime? Subtract(this DateTime? date, TimeSpan timeSpan) => date?.Subtract(timeSpan);

        public static DateTime? TrimMilliseconds(this DateTime? dateTime) => dateTime?.TrimMilliseconds();

        public static DateTime TrimMilliseconds(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

        public static DateTime TrimHours(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);

        public static DateTime TrimTicks(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond,
                dateTime.Kind);

        [PublicApiMethod]
        public static IFormattedMessage HumanFormat(this DateTime? dateTime) => HumanFormatExt(dateTime, format: null);

        public const string YearFormatId = "year";
        public const string HalfYearFormatId = "halfYear";
        public const string QuarterFormatId = "quarter";
        public const string MonthFormatId = "month";
        public const string WeekFormatId = "week";

        public static IFormattedMessage HumanFormatExt(this DateTime date, [CanBeNull] string format) => HumanFormatExt((DateTime?)date, format);

        [PublicApiMethod]
        public static IFormattedMessage HumanFormatExt(this DateTime? date, [CanBeNull] string format)
        {
            if (date == null) return NoDate;
            if (date.Value == MinDateValue) return Past;
            if (date.Value == MaxDateValue) return Future;

            switch (format)
            {
                case null:
                {
                    var dateStr = date.Value.ToString("MMM d");
                    return date.Value.Date == CurrentDate.Value.Date
                        ? "Today ({date})".Localize(new { date = dateStr })
                        : dateStr.AsLocalized();
                }
                case YearFormatId:
                {
                    var dateStr = date.Value.ToString("Y yyyy");
                    return date.FloorYear() == CurrentDate.Value.FloorYear()
                        ? "Current ({date})".Localize(new { date = dateStr })
                        : dateStr.AsLocalized();
                }
                case HalfYearFormatId:
                {
                    var dateStr = $"H{(date.Value.Month <= 6 ? 1 : 2)} {date.Value.Year}";
                    return date.FloorHalfYear() == CurrentDate.Value.FloorHalfYear()
                        ? "Current ({date})".Localize(new { date = dateStr })
                        : dateStr.AsLocalized();
                }
                case QuarterFormatId:
                {
                    var dateStr = $"Q{date.Value.Quarter()} {date.Value.Year}";
                    return date.FloorQuarter() == CurrentDate.Value.FloorQuarter()
                        ? "Current ({date})".Localize(new { date = dateStr })
                        : dateStr.AsLocalized();
                }
                case MonthFormatId:
                {
                    var dateStr = date.Value.ToString("MMMM");
                    if (date.Value.Month == 1)
                    {
                        dateStr = dateStr + date.Value.ToString(" yyyy");
                    }
                    return date.FloorMonth() == CurrentDate.Value.FloorMonth()
                        ? "Current ({date})".Localize(new { date = dateStr })
                        : dateStr.AsLocalized();
                }
                case WeekFormatId:
                {
                    var dateStr = "W{weekNumber} ({weekStart} - {weekEnd})".Localize(new
                    {
                        weekNumber = date.Value.WeekNumberIso(),
                        weekStart = date.Value.FloorWeekToMonday().ToString("MMM d"),
                        weekEnd = date.Value.FloorWeekToMonday().AddDays(6).ToString("MMM d")
                    });
                    return date.FloorWeekToMonday() == CurrentDate.Value.FloorWeekToMonday()
                        ? "Current ({date})".Localize(new { date = dateStr.Value })
                        : dateStr;
                }
                default: throw new ArgumentOutOfRangeException(nameof(format));
            }
        }

        public static DateTime AddWeeks(this DateTime date, int weeks) => date.AddDays(weeks * 7);

        public static DateTime AddQuarters(this DateTime date, int quarters) => date.AddMonths(quarters * 3);
        public static int Quarter(this DateTime date) => (date.Month - 1)  / 3 + 1;

        public static readonly DateTime MaxDateValue = new DateTime(9999, 12, 31);
        public static readonly DateTime MinDateValue = new DateTime(1753, 1, 1);

        [PublicApiMethod]
        [DynamicExpressionAlias("Year")]
        [SqlFunction("dbo.f_FloorYear", DbType.DateTime)]
        public static DateTime FloorYear(this DateTime date) => new DateTime(date.Year, 1, 1, 0, 0, 0, date.Kind);

        [PublicApiMethod]
        [DynamicExpressionAlias("Year")]
        [SqlFunction("dbo.f_FloorYear", DbType.DateTime)]
        public static DateTime? FloorYear(this DateTime? date) => date.Select(FloorYear);

        [PublicApiMethod]
        [DynamicExpressionAlias("HalfYear")]
        [SqlFunction("dbo.f_FloorHalfYear", DbType.DateTime)]
        public static DateTime FloorHalfYear(this DateTime date) => new DateTime(date.Year, date.Month <= 6 ? 1 : 7, 1, 0, 0, 0, date.Kind);

        [PublicApiMethod]
        [DynamicExpressionAlias("HalfYear")]
        [SqlFunction("dbo.f_FloorHalfYear", DbType.DateTime)]
        public static DateTime? FloorHalfYear(this DateTime? date) => date.Select(FloorHalfYear);

        [PublicApiMethod]
        [DynamicExpressionAlias("Quarter")]
        [SqlFunction("dbo.f_FloorQuarter", DbType.DateTime)]
        public static DateTime FloorQuarter(this DateTime date)
        {
            return new DateTime(date.Year, (((date.Month - 1) / 3) * 3 + 1), 1, 0, 0, 0, date.Kind);
        }

        [PublicApiMethod]
        [DynamicExpressionAlias("Quarter")]
        [SqlFunction("dbo.f_FloorQuarter", DbType.DateTime)]
        public static DateTime? FloorQuarter(this DateTime? date) => date.Select(FloorQuarter);

        [PublicApiMethod]
        [DynamicExpressionAlias("Month")]
        [SqlFunction("dbo.f_FloorMonth", DbType.DateTime)]
        public static DateTime FloorMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind);

        [PublicApiMethod]
        [DynamicExpressionAlias("Month")]
        [SqlFunction("dbo.f_FloorMonth", DbType.DateTime)]
        public static DateTime? FloorMonth(this DateTime? date) => date.Select(FloorMonth);

        /// <summary>
        /// This method assume Sunday as first day of the week. See FloorWeekToMonday().
        /// </summary>
        [PublicApiMethod]
        [DynamicExpressionAlias("Week")]
        [SqlFunction("dbo.f_FloorWeek", DbType.DateTime)]
        public static DateTime? FloorWeek(this DateTime? date) => date.Select(FloorWeek);

        /// <summary>
        /// This method assume Sunday as first day of the week. See FloorWeekToMonday().
        /// </summary>
        [PublicApiMethod]
        [DynamicExpressionAlias("Week")]
        [SqlFunction("dbo.f_FloorWeek", DbType.DateTime)]
        public static DateTime FloorWeek(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind).AddDays(-(int) dateTime.DayOfWeek);
        }

        /// <summary>
        /// This method assume Monday as first day of the week. See FloorWeek().
        /// </summary>
        [PublicApiMethod]
        [DynamicExpressionAlias("WeekToMonday")]
        [SqlFunction("dbo.f_FloorWeekToMonday", DbType.DateTime)]
        public static DateTime? FloorWeekToMonday(this DateTime? dateTime) => dateTime.Select(FloorWeekToMonday);

        /// <summary>
        /// This method assume Monday as first day of the week. See FloorWeek().
        /// </summary>
        [PublicApiMethod]
        [DynamicExpressionAlias("WeekToMonday")]
        [SqlFunction("dbo.f_FloorWeekToMonday", DbType.DateTime)]
        public static DateTime FloorWeekToMonday(this DateTime dateTime)
        {
            var dayOfWeek = dateTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) dateTime.DayOfWeek;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind).AddDays(-(dayOfWeek - 1));
        }

        private const int WeeksInShortYear = 52;
        private const int WeeksInLongYear = 53;

        /// <summary>
        /// Returns week number according to ISO 8601 standard.
        /// This presumes that weeks start with Monday.
        /// Week 1 is the 1st week of the year with a Thursday in it.
        /// See https://en.wikipedia.org/wiki/ISO_week_date#Calculating_the_week_number_of_a_given_date
        /// Also see https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Globalization/ISOWeek.cs
        /// and https://gitlab.tpondemand.net/frontend/targetprocess-frontend/blob/develop/Code/Main/Tp.Web/JavaScript/tau/scripts/tau/utils/utils.date.js
        /// </summary>
        public static int WeekNumberIso(this DateTime date)
        {
            var dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) date.DayOfWeek;
            var weekNumber = (date.DayOfYear - dayOfWeek + 10) / 7;
            if (weekNumber == 0)
            {
                // If the week number equals 0, it means that the given date belongs to the last week of preceding year.
                return GetWeeksInYear(date.Year - 1);
            }

            if (weekNumber == WeeksInLongYear && GetWeeksInYear(date.Year) == WeeksInShortYear)
            {
                // If a week number of 53 is obtained, one must check that the date is not actually in week 1 of the following year.
                return 1;
            }

            return weekNumber;
        }

        // Algorithm from https://en.wikipedia.org/wiki/ISO_week_date#Weeks_per_year
        private static int GetWeeksInYear(int year)
        {
            int P(int y) => (y + (y / 4) - (y / 100) + (y / 400)) % 7;

            return P(year) == 4 || P(year - 1) == 3 ? WeeksInLongYear : WeeksInShortYear;
        }
    }
}
