using System;
using System.ComponentModel;

namespace Tp.Core
{
	[Description("Indexes are zero-based")]
	public struct Range : IEquatable<Range>
	{
		private readonly int _startInclusive;
		private readonly int _endExclusive;
		private Range(int startInclusive, int endExclusive)
		{
			_startInclusive = startInclusive;
			_endExclusive = endExclusive;
		}

		public int StartInclusive
		{
			get { return _startInclusive; }
		}

		public int EndExclusive
		{
			get { return _endExclusive; }
		}

		public int Skip
		{
			get { return _startInclusive; }
		}

		public int Take
		{
			get { return Length; }
		}

		public int Length
		{
			get { return EndExclusive - StartInclusive; }
		}

		public bool IsEmpty
		{
			get { return Length == 0; }
		}
		
		public static bool operator ==(Range self, Range other)
		{
			return self.Equals(other);
		}

		public static bool operator !=(Range self, Range other)
		{
			return !self.Equals(other);
		}

		public override int GetHashCode()
		{
			return Length.GetHashCode() ^ StartInclusive.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Range && Equals((Range) obj);
		}

		public bool Equals(Range other)
		{
			return StartInclusive == other.StartInclusive && EndExclusive == other.EndExclusive;
		}

		public static Range Absolute(int startInclusive, int endExclusive)
		{
			return new Range(startInclusive, endExclusive);
		}

		public static Range Relative(int skip, int take)
		{
			return new Range(skip, skip + take);
		}

	}
}
