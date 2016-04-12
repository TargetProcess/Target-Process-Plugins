using System.Data;
using System.Linq.Dynamic;
using Tp.Core;
using Tp.Core.Internationalization;
using Tp.I18n;

// ReSharper disable CheckNamespace

namespace System
// ReSharper restore CheckNamespace
{
	[LocalizationScope("field-names")]
	public static class DateTimeExtensions
	{
		private static IFormattedMessage NoDate => "No date".Localize();
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

		[SqlFunction("dbo.f_WeekNumber", DbType.UInt32)]
		public static int WeekNumber(this DateTime date)
		{
			return ((int) Math.Floor((date - new DateTime(1990, 1, 1)).TotalDays)) / 7;
		}

		[SqlFunction("dbo.f_Date", DbType.Date)]
		[DynamicExpressionAlias("Day")]
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

		public static DateTime? AddDays(this DateTime? date, double days) => date?.AddDays(days);

		public static DateTime? Subtract(this DateTime? date, TimeSpan timeSpan) => date?.Subtract(timeSpan);

		public static DateTime? TrimMilliseconds(this DateTime? dateTime) => dateTime?.TrimMilliseconds();

		public static DateTime TrimMilliseconds(this DateTime dateTime)
			=> new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

		public static DateTime TrimHours(this DateTime dateTime)
			=> new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);

        public static DateTime TrimTicks(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

        public static IFormattedMessage HumanFormat(this DateTime? dateTime)
		{
			if (dateTime != null)
			{
				if (dateTime.Value == MinDateValue)
					return Past;
				if (dateTime.Value == MaxDateValue)
					return Future;

				var date = dateTime.Value.ToString("MMM d");
				return dateTime.Value.Date == CurrentDate.Value.Date
					? "Today ({date})".Localize(new { date })
					: date.AsLocalized();
			}
			return NoDate;
		}

		public static readonly DateTime MaxDateValue = new DateTime(9999, 12, 31);
		public static readonly DateTime MinDateValue = new DateTime(1753, 1, 1);


		[DynamicExpressionAlias("Year")]
		[SqlFunction("dbo.f_FloorYear", DbType.DateTime)]
		public static DateTime FloorYear(this DateTime date) => new DateTime(date.Year, 1, 1, 0, 0, 0, date.Kind);

		[DynamicExpressionAlias("Year")]
		[SqlFunction("dbo.f_FloorYear", DbType.DateTime)]
		public static DateTime? FloorYear(this DateTime? date) => date.Select(FloorYear);

		[DynamicExpressionAlias("Quarter")]
		[SqlFunction("dbo.f_FloorQuarter", DbType.DateTime)]
		public static DateTime FloorQuarter(this DateTime date)
		{
			return new DateTime(date.Year, (((date.Month - 1) / 3) * 3 + 1), 1, 0, 0, 0, date.Kind);
		}

		[DynamicExpressionAlias("Quarter")]
		[SqlFunction("dbo.f_FloorQuarter", DbType.DateTime)]
		public static DateTime? FloorQuarter(this DateTime? date) => date.Select(FloorQuarter);

		[DynamicExpressionAlias("Month")]
		[SqlFunction("dbo.f_FloorMonth", DbType.DateTime)]
		public static DateTime FloorMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind);

		[DynamicExpressionAlias("Month")]
		[SqlFunction("dbo.f_FloorMonth", DbType.DateTime)]
		public static DateTime? FloorMonth(this DateTime? date) => date.Select(FloorMonth);

		[DynamicExpressionAlias("Week")]
		[SqlFunction("dbo.f_FloorWeek", DbType.DateTime)]
		public static DateTime? FloorWeek(this DateTime? date) => date.Select(FloorWeek);

		[DynamicExpressionAlias("Week")]
		[SqlFunction("dbo.f_FloorWeek", DbType.DateTime)]
		private static DateTime FloorWeek(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind).AddDays(-(int) dateTime.DayOfWeek);
		}
	}
}
