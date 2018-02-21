// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.BugTracking.Settings
{
    public interface IBugTrackingConnectionSettingsSource
    {
        string Url { get; }

        string Login { get; }

        string Password { get; }

        string SavedSearches { get; }
    }
}
