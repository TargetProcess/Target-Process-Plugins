// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Activity
{
    public static class DateRangeExtensions
    {
        public static bool IsInRangeIncludingEndDate(this DateRange range, DateTime dateTime)
        {
            return (!range.StartDate.HasValue || range.StartDate.Value < dateTime)
                && (!range.EndDate.HasValue || range.EndDate.Value >= dateTime);
        }
    }
}
