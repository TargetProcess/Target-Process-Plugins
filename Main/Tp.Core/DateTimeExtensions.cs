//  
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Data;
using Tp.Core;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class DateTimeExtensions
	{
		public const string NO_DATE = "No date";
		public const string FUTURE = "Future";
		public const string PAST = "Past";
		private const string FORMATE_DATE_STRING = "dd MMM";

		public static string GetDateName(this DateTime date)
		{
			return date.ToString(FORMATE_DATE_STRING);
		}

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
		{
			return String.Format("{0} - {1}", weekStartDate.ToString(FORMATE_DATE_STRING),
													 weekEndDate.ToString(FORMATE_DATE_STRING));
		}

		public static string GetWeekName(this DateTime date)
		{
			return GetPartWeekName(date.GetWeekFirstDay(), date.GetWeekLastDay());
		}

		public static string ToJavascriptDate(this DateTime date)
		{
			var milliseconds = Convert.ToInt64(
				date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
			return "new Date({0})".Fmt(milliseconds);
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
			return ((int)Math.Floor((date - new DateTime(1990, 1, 1)).TotalDays)) / 7;
		}

		[SqlFunction("dbo.f_Date", DbType.Date)]
		public static DateTime? Date(this DateTime? date)
		{
			return date.Map(x => x.Date);
		}

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
			decimal stepInMs = (decimal)(passedDuration.TotalMilliseconds * 0.01) / progress.Value;
			decimal forecastDurationInMs = stepInMs * (1 - progress.Value) * 100;
			TimeSpan forecastDuration = TimeSpan.FromMilliseconds((long)Math.Ceiling(forecastDurationInMs));
			DateTime predictedEndDate = now + forecastDuration;
			return predictedEndDate;
		}

		[SqlDateFunction("dbo.f_DateDiffInSeconds", DbType.Int32)]
		public static int DateDiffInSeconds(this DateTime? date, DateTime? otherDate)
		{
			return (int) (otherDate - date).GetValueOrDefault().TotalSeconds;
		}

		public static DateTime? AddDays(this DateTime? date, double days)
		{
			return date.Map(x => x.AddDays(days));
		}

		public static DateTime? Subtract(this DateTime? date, TimeSpan timeSpan)
		{
			return date.Map(x => x.Subtract(timeSpan));
		}

		public static DateTime? TrimMilliseconds(this DateTime? dateTime)
		{
			return dateTime.HasValue ? (DateTime?) TrimMilliseconds(dateTime.Value) : null;
		}

		public static DateTime TrimMilliseconds(this DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
		}

		public static DateTime TrimHours(this DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
		}

		public static string HumanFormat(this DateTime? dateTime)
		{
			if (dateTime != null)
			{
				if (dateTime.Value == MinDateValue)
					return PAST;
				if (dateTime.Value == MaxDateValue)
					return FUTURE;

				return ((dateTime.Value.Date == CurrentDate.Value.Date) ? "Today ({0})" : "{0}").Fmt(dateTime.Value.ToString("MMM d"));
			}
			return NO_DATE;
		}

		public static readonly DateTime MaxDateValue = new DateTime(9999,12,31);
		public static readonly DateTime MinDateValue = new DateTime(1753,1,1);
	}
}