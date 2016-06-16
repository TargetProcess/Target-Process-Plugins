using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using Tp.Core;
using Tp.Core.Annotations;

namespace Tp.Utils
{
	public static class DateUtils
	{
		//2013-05-02 00:00:00 - Canonical Time
		public const string DateSqlStringFormat = "yyyy-MM-dd HH:mm:ss";

		private static readonly string[] _formats =
		{
			// these 2 time formats are used in Bugzilla
			"yyyy'-'MM'-'dd HH':'mm",
			"yyyy'-'MM'-'dd HH':'mm':'ss",
			"dd MMM yyyy HH':'mm",
			"dd MMM yyyy HH':'mm':'ss",
			"ddd, dd MMM yyyy HH':'mm",
			"ddd, dd MMM yyyy HH':'mm':'ss",
		};

		private static readonly ILog _log = LogManager.GetLogger(typeof(DateUtils));
		private static readonly StringDictionary _phpDateformat;
		private static IList<DayOfWeek> _workingDays;

		static DateUtils()
		{
			_phpDateformat = new StringDictionary
			{
				{ "d", "j" },
				{ "%d", "j" },
				{ "dd", "d" },
				{ "ddd", "D" },
				{ "dddd", "l" },
				{ "M", "n" },
				{ "%M", "n" },
				{ "MM", "m" },
				{ "MMM", "M" },
				{ "MMMM", "F" },
				{ "&y", "y" },
				{ "yy", "y" },
				{ "yyyy", "Y" }
			};
		}

		public static bool Is12HoursFormat
		{
			get
			{
				return !String.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator) ||
					!String.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator);
			}
		}

		public static string TimePattern
		{
			get { return Is12HoursFormat ? "g:i A" : "H:i"; }
		}

		public static IList<DayOfWeek> WorkingDays
		{
			get
			{
				if (_workingDays == null)
				{
					SetWorkingDays(String.Empty);
				}
				return _workingDays;
			}
		}

		public static string ShortDateExtJsPattern
		{
			get
			{
				var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
				var separatorToken = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;
				var patternParts = pattern.Split(new[] { separatorToken }, StringSplitOptions.RemoveEmptyEntries);
				var resultPattern = String.Empty;
				for (var i = 0; i < patternParts.Length; i++)
				{
					if (_phpDateformat.ContainsKey(patternParts[i]))
						resultPattern += _phpDateformat[patternParts[i]];
					else
						resultPattern += patternParts[i];

					if (i < patternParts.Length - 1)
						resultPattern += separatorToken;
				}
				return resultPattern;
			}
		}

		public static DateTime[] GetDays(DateTime start, DateTime end)
		{
			var firstDay = start.Date;
			var lastDay = end.Date;

			if (firstDay > lastDay)
				throw new ArgumentException("start date must not be later than end date");

			var days = new List<DateTime>();
			for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
			{
				days.Add(day);
			}
			return days.ToArray();
		}

		public static DateTime[] GetWeeks(DateTime start, DateTime end)
		{
			var firstMonday = FindMonday(start);
			var lastMonday = FindMonday(end);

			if (firstMonday > lastMonday)
				throw new ArgumentException("start date must not be later than end date");

			var weeks = new List<DateTime>();
			while (firstMonday.Date <= lastMonday.Date)
			{
				weeks.Add(firstMonday);
				firstMonday = firstMonday.AddDays(7);
			}
			return weeks.ToArray();
		}

		public static DateTime[] GetMonths(DateTime start, DateTime end)
		{
			var firstMonth = new DateTime(start.Year, start.Month, 1).AddMonths(-1);
			var lastMonth = new DateTime(end.Year, end.Month, 1).AddMonths(2);

			if (firstMonth > lastMonth)
				throw new ArgumentException("start date must not be later than end date");

			var months = new List<DateTime>();
			while (firstMonth.Date <= lastMonth.Date)
			{
				months.Add(firstMonth);
				firstMonth = firstMonth.AddMonths(1);
			}
			return months.ToArray();
		}

		private static DateTime FindMonday(DateTime date)
		{
			while (date.DayOfWeek != DayOfWeek.Monday)
			{
				date = date.AddDays(-1);
			}
			return date.Date;
		}

		public static string GetTimeLag(this DateTime current, DateTime previousDate)
		{
			var span = current - previousDate;

			var lag = new StringBuilder();
			if (span.Days > 0)
			{
				lag.AppendFormat("{0} {1}", span.Days, span.Days > 1 ? "days" : "day");
			}
			else if (span.Hours > 0)
			{
				lag.AppendFormat("{0} {1}", span.Hours, span.Hours > 1 ? "hours" : "hour");
			}
			else if (span.Minutes > 0)
			{
				lag.AppendFormat("{0} {1}", span.Minutes, span.Minutes > 1 ? "minutes" : "minute");
			}
			else
			{
				return "Now";
			}
			lag.Append(" ago");
			return lag.ToString();
		}

		public static int DaysToCompletion(DateTime? baseDate)
		{
			if (baseDate == null)
				return Int32.MinValue;

			var days = baseDate.Value.Date - DateTime.Today;
			return days.Days;
		}

		public static int DaysPassed(DateTime? baseDate)
		{
			var today = DateTime.Today;

			return DaysPassed(baseDate, today);
		}

		public static int DaysPassed(DateTime? baseDate, DateTime today)
		{
			if (baseDate == null)
				return Int32.MinValue;

			var days = today - baseDate.Value.Date;

			return days.Days;
		}

		public static int GetWorkingDaysLeftFromCurrentDate(DateTime? startDate, DateTime? endDate)
		{
			DateTime fromDate = CurrentDate.Value < startDate.GetValueOrDefault()
				? startDate.GetValueOrDefault()
				: CurrentDate.Value;

			if (endDate.HasValue && fromDate.Date <= endDate.Value.Date)
			{
				return GetWorkingDaysCount(fromDate, endDate.Value);
			}

			return 0;
		}

		public static int GetWorkingDaysCount(DateRange dateRange)
		{
			return !dateRange.Equals(DateRange.Empty)
				? GetWorkingDaysCount(dateRange.StartDate.Value, dateRange.EndDate.Value)
				: 0;
		}

		public static int GetWorkingDaysCount(DateTime startDate, DateTime endDate)
		{
			startDate = startDate.Date;
			endDate = endDate.Date;

			if (endDate < startDate)
				throw new ArgumentException("'endDate' must be greater than or equals to 'startDate'");

			var duration = (endDate - startDate).Days + 1;
			var workingDays = 0;
			for (var i = 0; i < duration; i++)
			{
				var date = startDate.AddDays(i);
				if (IsWorkingDay(date)) workingDays++;
			}
			return workingDays;
		}

		/// <summary>
		/// Check if date is a working day
		/// </summary>
		/// <remarks>The day of the week is checked at the moment. The holidays calendar can be added later</remarks>
		/// <param name="date">The date to be checked</param>
		/// <returns>If the date is a working day.</returns>
		public static bool IsWorkingDay(this DateTime date)
		{
			return WorkingDays.Contains(date.DayOfWeek);
		}

		public static void SetWorkingDays(string notWorkingDays)
		{
			_workingDays = new List<DayOfWeek>(
				new[]
				{
					DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
					DayOfWeek.Saturday
					, DayOfWeek.Sunday
				});
			if (!String.IsNullOrEmpty(notWorkingDays))
			{
				var days = notWorkingDays.Split(new[] { ',' });

				foreach (var dayStr in days)
				{
					try
					{
						var day = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), dayStr);
						_workingDays.Remove(day);
					}
					catch
					{
						_log.ErrorFormat("Can't parse 'notWorkingDays' parameter. '{0}' is not valid DayOfWeek", dayStr);
					}
				}
			}
			else
			{
				_workingDays.Remove(DayOfWeek.Saturday);
				_workingDays.Remove(DayOfWeek.Sunday);
			}
		}

		public static DateTime ParseFromUniversalTime(this string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var zzz = 0; // time zone offset stored as HH * 100 + MM

			var r = new Regex(@"(?<dateTime>.*?)(?<offset>([+-]\d\d\d\d)|[a-z]+)\s*$", RegexOptions.IgnoreCase);
			var m = r.Match(input);
			if (m.Success)
			{
				input = m.Groups["dateTime"].Value.Trim();
				var zone = m.Groups["offset"].Value;
				switch (zone.ToLower())
				{
					case "brst":
						zzz = -0200;
						break; // Brazil Summer Time (East Daylight)
					case "adt":
						zzz = -0300;
						break; // Atlantic Daylight
					case "edt":
						zzz = -0400;
						break; // Eastern Daylight
					case "cdt":
						zzz = -0500;
						break; // Central Daylight
					case "mdt":
						zzz = -0600;
						break; // Mountain Daylight
					case "pdt":
						zzz = -0700;
						break; // Pacific Daylight
					case "ydt":
						zzz = -0800;
						break; // Yukon Daylight
					case "hdt":
						zzz = -0900;
						break; // Hawaii Daylight
					case "bst":
						zzz = +0100;
						break; // British Summer
					case "mest":
						zzz = +0200;
						break; // Middle European Summer
					case "sst":
						zzz = +0200;
						break; // Swedish Summer
					case "fst":
						zzz = +0200;
						break; // French Summer
					case "cest":
						zzz = +0200;
						break; // Central European Daylight
					case "eest":
						zzz = +0300;
						break; // Eastern European Summer
					case "wadt":
						zzz = +0800;
						break; // West Australian Daylight
					case "kdt":
						zzz = +1000;
						break; // Korean Daylight
					case "eadt":
						zzz = +1100;
						break; // Eastern Australian Daylight
					case "nzd":
						zzz = +1300;
						break; // New Zealand Daylight
					case "nzdt":
						zzz = +1300;
						break; // New Zealand Daylight
					case "gmt":
						zzz = 000;
						break; // Greenwich Mean
					case "ut":
						zzz = 000;
						break; // Universal (Coordinated)
					case "utc":
						zzz = 000;
						break;
					case "wet":
						zzz = 000;
						break; // Western European
					case "wat":
						zzz = -0100;
						break; // West Africa
					case "at":
						zzz = -0200;
						break; // Azores
					case "fnt":
						zzz = -0200;
						break; // Brazil Time (Extreme East - Fernando Noronha)
					case "brt":
						zzz = -0300;
						break; // Brazil Time (East Standard - Brasilia)
					case "mnt":
						zzz = -0400;
						break; // Brazil Time (West Standard - Manaus)
					case "ewt":
						zzz = -0400;
						break; // U.S. Eastern War Time
					case "ast":
						zzz = -0400;
						break; // Atlantic Standard
					case "est":
						zzz = -0500;
						break; // Eastern Standard
					case "act":
						zzz = -0500;
						break; // Brazil Time (Extreme West - Acre)
					case "cst":
						zzz = -0600;
						break; // Central Standard
					case "mst":
						zzz = -0700;
						break; // Mountain Standard
					case "pst":
						zzz = -0800;
						break; // Pacific Standard
					case "yst":
						zzz = -0900;
						break; // Yukon Standard
					case "hst":
						zzz = -1000;
						break; // Hawaii Standard
					case "cat":
						zzz = -1000;
						break; // Central Alaska
					case "ahst":
						zzz = -1000;
						break; // Alaska-Hawaii Standard
					case "nt":
						zzz = -1100;
						break; // Nome
					case "idlw":
						zzz = -1200;
						break; // International Date Line West
					case "cet":
						zzz = +0100;
						break; // Central European
					case "mez":
						zzz = +0100;
						break; // Central European (German)
					case "ect":
						zzz = +0100;
						break; // Central European (French)
					case "met":
						zzz = +0100;
						break; // Middle European
					case "mewt":
						zzz = +0100;
						break; // Middle European Winter
					case "swt":
						zzz = +0100;
						break; // Swedish Winter
					case "set":
						zzz = +0100;
						break; // Seychelles
					case "fwt":
						zzz = +0100;
						break; // French Winter
					case "eet":
						zzz = +0200;
						break; // Eastern Europe, USSR Zone 1
					case "ukr":
						zzz = +0200;
						break; // Ukraine
					case "bt":
						zzz = +0300;
						break; // Baghdad, USSR Zone 2
					case "wst":
						zzz = +0800;
						break; // West Australian Standard
					case "hkt":
						zzz = +0800;
						break; // Hong Kong
					case "cct":
						zzz = +0800;
						break; // China Coast, USSR Zone 7
					case "jst":
						zzz = +0900;
						break; // Japan Standard, USSR Zone 8
					case "kst":
						zzz = +0900;
						break; // Korean Standard
					case "east":
						zzz = +1000;
						break; // Eastern Australian Standard
					case "gst":
						zzz = +1000;
						break; // Guam Standard, USSR Zone 9
					case "nzt":
						zzz = +1200;
						break; // New Zealand
					case "nzst":
						zzz = +1200;
						break; // New Zealand Standard
					case "idle":
						zzz = +1200;
						break; // International Date Line East

					default:
					{
						if (
							!Int32.TryParse(zone, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out zzz))
						{
							zzz = 0;
						}
						break;
					}
				}
			}

			var time = DateTime.ParseExact(input, _formats, CultureInfo.InvariantCulture,
				DateTimeStyles.AllowInnerWhite);

			var offset = new TimeSpan(zzz / 100, zzz % 100, 0);
			var dateTime = time.Subtract(offset).ToLocalTime();
			return dateTime;
		}

		public static int GetNumberOfWorkingDaysInclusive(DateTime startDate, DateTime endDate)
		{
			if (startDate.Date > endDate.Date)
			{
				return 0;
			}
			var result = 0;
			var currentDate = startDate.Date;
			while (currentDate <= endDate.Date)
			{
				if (currentDate.IsWorkingDay())
				{
					result++;
				}
				currentDate = currentDate.AddDays(1);
			}
			return result;
		}

		public static DateTime? Max(DateTime? dateTime1, DateTime? dateTime2)
		{
			if (dateTime1 == null || dateTime2 == null)
			{
				return null;
			}
			return dateTime1 > dateTime2 ? dateTime1 : dateTime2;
		}

		[NotNull, Pure]
		public static string ToSqlString(this DateTime date)
		{
			return date.ToString(DateSqlStringFormat);
		}

		[NotNull, Pure]
		public static string ToSqlString([CanBeNull] this DateTime? date)
		{
			return date == null ? "null" : date.Value.ToSqlString();
		}

		[Pure]
		public static DateTime FromSqlString([NotNull] string str)
		{
			return DateTime.ParseExact(str, DateSqlStringFormat, CultureInfo.InvariantCulture);
		}
	}
}
