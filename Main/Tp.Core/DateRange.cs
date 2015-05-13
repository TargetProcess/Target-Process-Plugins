// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public class DateRange : IEquatable<DateRange>
	{
		private DateTime? _startDate, _endDate;

		// TODO: remove the flag
		private readonly bool _validatePeriod;

		public static readonly DateRange AllDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
		public static readonly DateRange Empty = new DateRange(null, null);

		public DateRange() : this(null, null)
		{
		}

		public DateRange(DateTime? startDate, bool validatePeriod = true)
			: this(startDate, startDate, validatePeriod)
		{
		}

		public DateRange(DateTime? startDate, DateTime? endDate, bool validatePeriod = true)
		{
			_validatePeriod = validatePeriod;
			_startDate = startDate;
			_endDate = endDate;
			AssertDateRangeIsValid();
		}

		public TimeSpan? TimeSpan
		{
			get { return _endDate - _startDate; }
		}

		public bool IsOpened
		{
			get { return TimeSpan == null; }
		}

		public DateTime? StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				AssertDateRangeIsValid();
			}
		}

		public DateTime? EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				AssertDateRangeIsValid();
			}
		}

		public bool IsInFuture
		{
			get { return StartDate.GetValueOrDefault().Date > CurrentDate.Value.Date; }
		}

		[AssertionMethod]
		private void AssertDateRangeIsValid()
		{
			if (_validatePeriod && !IsValidRange(StartDate, EndDate))
			{
				throw new InvalidDateRangeException("Start Date must be less than or equal to End Date");
			}
		}

		public DateRange GetIntersection(DateRange other)
		{
			if (!Intersects(other))
			{
				throw new InvalidOperationException("DateRanges do not intersect");
			}
			return new DateRange(GetLaterStartDate(other.StartDate), GetEarlierEndDate(other.EndDate));
		}

		public DateTime? CutDate(DateTime? date)
		{
			return GetEarlierEndDate(GetLaterStartDate(date));
		}

		private DateTime? GetLaterStartDate(DateTime? other)
		{
			return Nullable.Compare(_startDate, other) >= 0 ? _startDate : other;
		}

		private DateTime? GetEarlierEndDate(DateTime? other)
		{
			//!endDate.HasValue == +infinity, not negative infinity
			//as is the case with !startDate.HasValue
			if (Nullable.Compare(_endDate, other) == 0)
			{
				return other;
			}
			if (_endDate.HasValue && !other.HasValue)
			{
				return _endDate;
			}
			if (!_endDate.HasValue && other.HasValue)
			{
				return other;
			}
			return (Nullable.Compare(_endDate, other) >= 0) ? other : _endDate;
		}

		public bool Contains(DateTime? other)
		{
			return Intersects(new DateRange(other, other));
		}

		public DateRangeContainsResult Contains(DateRange dateRange)
		{
			var result = new DateRangeContainsResult
			{
				IncludeStartDate = true,
				IncludeEndDate = true
			};
			if ((StartDate != null && StartDate > dateRange.StartDate) || (EndDate != null && EndDate < dateRange.StartDate))
			{
				result.IncludeStartDate = false;
			}
			if ((EndDate != null && EndDate < dateRange.EndDate) || (StartDate != null && StartDate > dateRange.EndDate))
			{
				result.IncludeEndDate = false;
			}
			return result;
		}

		public bool Intersects(DateRange other)
		{
			return (!StartDate.HasValue || !other.EndDate.HasValue || StartDate <= other.EndDate) &&
			       (!EndDate.HasValue || !other.StartDate.HasValue || EndDate >= other.StartDate);
		}

		public bool Equals(DateRange other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._startDate.Equals(_startDate) && other._endDate.Equals(_endDate);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (DateRange)) return false;
			return Equals((DateRange) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_startDate.HasValue ? _startDate.Value.GetHashCode() : 0)*397) ^
				       (_endDate.HasValue ? _endDate.Value.GetHashCode() : 0);
			}
		}

		public override string ToString()
		{
			return "{0} - {1}".Fmt(StartDate != null ? StartDate.ToString() : "null", EndDate != null ? EndDate.ToString() : "null");
		}

		public static DateRange Create(DateTime startDate, int duration)
		{
			return new DateRange(startDate, startDate.AddDays(duration - 1));
		}

		public static bool IsValidRange(DateTime? startDate, DateTime? endDate)
		{
			return !startDate.HasValue || !endDate.HasValue || startDate.Value <= endDate.Value;
		}
	}
}