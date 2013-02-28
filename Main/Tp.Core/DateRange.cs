// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	public class DateRange : IEquatable<DateRange>
	{
		private DateTime? _startDate, _endDate;
		public static readonly DateRange AllDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);

		public DateRange()
			: this(new DateTime?(), new DateTime?())
		{
		}

		public DateRange(DateTime? startDate) : this(startDate, startDate)
		{
		}

		public DateRange(DateTime? startDate, DateTime? endDate)
		{
			AssertStartDateFollowsEndDate(startDate, endDate);
			_startDate = startDate;
			_endDate = endDate;
		}

		public TimeSpan? TimeSpan
		{
			get { return _endDate - _startDate; }
		}

		public DateTime? StartDate
		{
			get { return _startDate; }
			set
			{
				AssertStartDateFollowsEndDate(value, _endDate);
				_startDate = value;
			}
		}

		public DateTime? EndDate
		{
			get { return _endDate; }
			set
			{
				AssertStartDateFollowsEndDate(_startDate, value);
				_endDate = value;
			}
		}

		public bool IsInFuture
		{
			get { return StartDate.GetValueOrDefault().Date > CurrentDate.Value.Date; }
		}

		public static DateRange Empty
		{
			get { return new DateRange(null, null); }
		}

		[Annotations.AssertionMethod]
		private static void AssertStartDateFollowsEndDate(DateTime? startDate,
		                                                  DateTime? endDate)
		{
			if ((startDate.HasValue && endDate.HasValue) &&
			    (endDate.Value < startDate.Value))
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

		public static DateRange Create(DateTime startDate, int duration)
		{
			return new DateRange(startDate, startDate.AddDays(duration - 1));
		}
	}
}