using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tp.Core.Diagnostics.Time
{
	public class TimePoints : TimePointsBase
	{
		private readonly List<TimePoint> _timepoints;
		public TimePoints()
		{
			_timepoints = new List<TimePoint>();
		}

		private TimePoints(TimePoints other)
		{
			_timepoints = new List<TimePoint>(other._timepoints);
		}

		public override IEnumerable<TimePoint> Points
		{
			get { return _timepoints; }
		}

		public override void Add(TimePoint timestamp)
		{
			_timepoints.Add(timestamp);
		}

		public override void AddUtcNow(string name)
		{
			Add(TimePoint.UtcNow(name));
		}

		public override string Dump()
		{
			var distances = CalculateDistances();
			return distances.Aggregate(new StringBuilder(), (acc, distance) => acc.AppendLine(distance.ToString())).ToString();
		}
		
		public override ITimePoints CreateFork()
		{
			return new TimePoints(this);
		}

		public IEnumerable<TimePointsDistance> CalculateDistances()
		{
			var list = _timepoints.ToList();
			var ordered = list.OrderBy(t => t).ToList();
			var distances = ordered
						.Skip(1)
						.Zip(ordered, (next, prev) => new TimePointsDistance
						{
							Start = prev.Name,
							End = next.Name,
							Length = (next.Timestamp - prev.Timestamp)
						})
						.ToList();
			distances.Add(new TimePointsDistance
			{
				Start = ordered.First().Name,
				End = ordered.Last().Name,
				Length = ordered.Last().Timestamp - ordered.First().Timestamp
			});
			return distances;
		}
		
		public struct TimePointsDistance : IEquatable<TimePointsDistance>
		{
			public string Start { get; set; }
			public string End { get; set; }
			public TimeSpan Length { get; set; }

			public bool Equals(TimePointsDistance other)
			{
				return Equals(other.Start, Start) && Equals(other.End, End) && other.Length.Equals(Length);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (obj.GetType() != typeof (TimePointsDistance)) return false;
				return Equals((TimePointsDistance) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int result = (Start != null ? Start.GetHashCode() : 0);
					result = (result*397) ^ (End != null ? End.GetHashCode() : 0);
					result = (result*397) ^ Length.GetHashCode();
					return result;
				}
			}

			public override string ToString()
			{
				return "{0} - {1}, ms :{2}".Fmt(Start, End, Length.TotalMilliseconds);
			}
		}
	}
}