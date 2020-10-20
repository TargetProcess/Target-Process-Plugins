using System;

namespace Tp.Model.Common.Interfaces
{
    public interface ITimePeriodBase
    {
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }

    public interface IPreviousTimePeriodBase
    {
        DateTime? PreviousStartDate { get; }
        DateTime? PreviousEndDate { get; }
    }

    public interface ILeadCycleTimeSource : ITimePeriodBase
    {
        DateTime? CreateDate { get; set; }
    }

    public interface ITimePeriod : ITimePeriodBase
    {
        bool IsPast { get; }
        int? Duration { get; set; }
    }
}
