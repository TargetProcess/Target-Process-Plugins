using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tp.Core.Diagnostics.Time
{
    public class TimePoints : ITimePoints
    {
        public static readonly ITimePointsReadOnly Empty = new TimePoints();

        private readonly List<TimePoint> _timepoints;

        public TimePoints()
        {
            _timepoints = new List<TimePoint>();
        }

        private TimePoints(TimePoints other)
        {
            _timepoints = new List<TimePoint>(other._timepoints);
        }

        public IReadOnlyList<TimePoint> Points => _timepoints;

        public TimePoint Add(TimePoint timepoint)
        {
            _timepoints.Add(timepoint);
            return timepoint;
        }

        public TimePoint AddUtcNow(string name)
        {
            return Add(TimePoint.UtcNow(name));
        }

        public string Dump()
        {
            var distances = CalculateDistances();
            return distances.Aggregate(new StringBuilder(), (acc, distance) => acc.AppendLine(distance.ToString())).ToString();
        }

        public ITimePoints Fork()
        {
            return new TimePoints(this);
        }

        public TimeSpan GetFullSpan()
        {
            var ordered = _timepoints.ToList().OrderBy(t => t).ToList();
            if (ordered.Empty())
            {
                return new TimeSpan();
            }
            return ordered.Last().Timestamp - ordered.First().Timestamp;
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
                if (obj.GetType() != typeof(TimePointsDistance)) return false;
                return Equals((TimePointsDistance) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (Start != null ? Start.GetHashCode() : 0);
                    result = (result * 397) ^ (End != null ? End.GetHashCode() : 0);
                    result = (result * 397) ^ Length.GetHashCode();
                    return result;
                }
            }

            public override string ToString()
            {
                return "{0} - {1}, ms :{2}".Fmt(Start, End, Length.TotalMilliseconds);
            }
        }

        public struct Range
        {
            public TimePoint Start { get; set; }
            public TimePoint End { get; set; }
        }

        public static Maybe<Range> FindRange(IReadOnlyCollection<TimePoint> timepoints, string beginEventName, string endEventName)
        {
            var start = timepoints.FirstOrNothing(x => x.Name == beginEventName);
            if (start.HasValue)
            {
                var finish = timepoints.FirstOrNothing(x => x.Name == endEventName);
                return finish.Select(x => new Range { Start = start.Value, End = x });
            }
            return Maybe.Nothing;
        }
    }
}
