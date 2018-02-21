// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla
{
    public static class BugzillaInfo
    {
        public static string[] SupportedVersions => new[]
        {
            "3.4",
            "3.6",
            "4.0",
            "4.2",
            "4.4",
            "5.0",
            "5.1"
        };

        public static string SupportedCgiScriptVersion => "2";
    }
}
