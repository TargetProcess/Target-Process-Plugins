using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics.Time
{
    public interface ITimePointsReadOnly
    {
        string Dump();
        IReadOnlyList<TimePoint> Points { get; }
        TimeSpan GetFullSpan();
    }

    public interface ITimePoints : ITimePointsReadOnly, ITimePointsForkable
    {
        TimePoint Add(TimePoint point);
        TimePoint AddUtcNow(string name);
    }

    public static class TimePointsExtensions
    {
        public static TimePoint? GetByName(this ITimePointsReadOnly timePoints, string name)
        {
            return timePoints.Points.FirstOrDefault(x => x.Name == name);
        }
    }
}
