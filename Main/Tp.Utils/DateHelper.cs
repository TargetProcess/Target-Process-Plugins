using System;
using System.Collections.Generic;

namespace Tp.Components
{
	public static class DateHelper
	{
		public static DateTime StartOfDay(this DateTime processDate)
		{
			return processDate.Date;
		}

		public static DateTime StartOfDay(this DateTime? processDate)
		{
			return !processDate.HasValue ? DateTime.MinValue : processDate.Value.StartOfDay();
		}

		public static DateTime EndOfDay(this DateTime date)
		{
			return date.Date.AddDays(1).Date.AddSeconds(-1);
		}

		public static DateTime GetMonthFirstDate(this DateTime processDate)
		{
			return new DateTime(processDate.Year, processDate.Month, 1);
		}

		public static bool SameMonthAs(this DateTime processDate, DateTime processDate1)
		{
			if (processDate.Year != processDate1.Year)
				return false;

			return processDate.Month == processDate1.Month;
		}

		public static bool SameWeekAs(this DateTime processDate, DateTime processDate1)
		{
			if (!processDate.SameMonthAs(processDate1))
				return false;

			return processDate.DayOfWeek == processDate1.DayOfWeek;
		}


		public static DateTime GetMonthEndDate(this DateTime date)
		{
			var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
			return new DateTime(date.Year, date.Month, daysInMonth);
		}


		public static DateTime EndOfDay(this DateTime? date)
		{
			return !date.HasValue ? DateTime.MinValue : date.Value.EndOfDay();
		}

		public static List<DateTime> GetDatesByMinAndMax(DateTime minDate, DateTime maxDate, int countOfDates)
		{
			var minTimeSpan = new TimeSpan(minDate.Date.Ticks);
			var maxTimeSpan = new TimeSpan(maxDate.Date.Ticks);

			var diff = maxTimeSpan - minTimeSpan;

			if (diff.TotalDays < countOfDates)
			{
				maxDate = maxDate.AddDays(countOfDates - diff.TotalDays);
			}

			minTimeSpan = new TimeSpan(minDate.Ticks);
			maxTimeSpan = new TimeSpan(maxDate.Ticks);

			diff = maxTimeSpan - minTimeSpan;

			var period = diff.TotalDays / (countOfDates - 1);

			var reportDates = new List<DateTime> { minDate.Date };

			var date = minDate.Date;
			for (var i = 1; i < (countOfDates - 1); i++)
			{
				reportDates.Add(date.AddDays(period * i).Date);
			}

			reportDates.Add(maxDate.Date);

			return reportDates;
		}

		public static DateTime ParseDate(this string date, DateTime today)
		{
			if (String.Compare(date, "before yesterday", true) == 0)
			{
				return today.AddDays(-2);
			}
			if (String.Compare(date, "yesterday", true) == 0)
			{
				return today.AddDays(-1);
			}
			if (String.Compare(date, "today", true) == 0)
			{
				return today.AddDays(0);
			}
			if (String.Compare(date, "tomorrow", true) == 0)
			{
				return today.AddDays(1);
			}
			if (String.Compare(date, "after tomorrow", true) == 0)
			{
				return today.AddDays(2);
			}
			throw new ArgumentException("date");
		}
	}
}
