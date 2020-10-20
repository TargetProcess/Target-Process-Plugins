using System;
using System.Data;
using Tp.Components;

namespace Tp.Model.Common.Extensions
{
    public static class ProjectAllocationDatesExtensions
    {
        [SqlFunction("dbo.f_GetAnticipatedEndDate", DbType.DateTime)]
        public static DateTime? GetAnticipatedEndDate(this DateTime? startDate, DateTime? plannedStartDate, DateTime? plannedEndDate)
        {
            if (startDate.HasValue && plannedStartDate.HasValue && plannedEndDate.HasValue)
            {
                var timeSpan = plannedEndDate.Value.EndOfDay() - plannedStartDate.Value.StartOfDay();
                return startDate.Value.StartOfDay().AddDays(timeSpan.TotalDays);
            }

            return null;
        }
    }
}
