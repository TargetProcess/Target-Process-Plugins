using System;
using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
    public interface ITimePointsReadOnly
    {
        string Dump();
        IReadOnlyList<TimePoint> Points { get; }
        TimeSpan GetFullSpan();
    }

    public interface ITimePoints : ITimePointsReadOnly, ITimePointsFork
    {
        TimePoint Add(TimePoint point);
        TimePoint AddUtcNow(string name);
    }
}
