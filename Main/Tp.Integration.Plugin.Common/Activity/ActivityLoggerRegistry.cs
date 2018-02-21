// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Integration.Plugin.Common.Activity
{
    public class ActivityLoggerRegistry
    {
        public const string LoggerPostfix = "TpInternal";
        public const string ActivityLoggerName = "PluginActivityLogger";
        public const string ErrorLoggerName = "PluginErrorLogger";

        public static IEnumerable<string> LoggersNames => new[] { ActivityLoggerName, ErrorLoggerName };

        public static bool IsKnownLogger(string name)
        {
            return LoggersNames.Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
