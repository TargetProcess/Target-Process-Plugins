// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.BugTracking.ImportToTp
{
    [Serializable]
    public class FailedSyncDate
    {
        public FailedSyncDate()
        {
        }

        public FailedSyncDate(DateTime? failedSyncDate)
        {
            _value = failedSyncDate;
        }

        private readonly DateTime? _value;

        public DateTime? GetValue()
        {
            return _value;
        }
    }
}
