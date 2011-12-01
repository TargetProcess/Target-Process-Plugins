//  
// Copyright (c) 2005-2009 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	public static class DateTimeExtensions
	{
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
			return "new Date({0})".Fmt(date.Subtract(new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc)).TotalMilliseconds);
		}
	}
}