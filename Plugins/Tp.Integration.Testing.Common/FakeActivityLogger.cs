//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.Integration.Testing.Common
{
    public class FakeActivityLogger : PluginActivityLogger
    {
        private readonly ILogProvider _logProvider;

        public List<string> LoggersUsed { get; set; }

        public FakeActivityLogger(ILogProvider logProvider)
            : base(logProvider)
        {
            _logProvider = logProvider;
            LoggersUsed = new List<string>();
        }

        protected override void Log(Action<ILog> action)
        {
            var logs = _logProvider.GetActivityLoggers()
                //.Where(x => ActivityLoggerRegistry.IsKnownLogger(x.Logger.Name))
                .Select(x => x.Logger.Name)
                .ToList();
            LoggersUsed.AddRange(logs);
        }
    }
}
