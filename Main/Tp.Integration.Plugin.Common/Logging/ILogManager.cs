// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using log4net;

namespace Tp.Integration.Plugin.Common.Logging
{
    /// <summary>
    /// Provides access to default logger. Injected into StructureMap container.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Gets logger by type
        /// </summary>
        /// <param name="type">Type for log4net logger</param>
        /// <returns></returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Get logger by name
        /// </summary>
        /// <param name="name">Log4net logger name</param>
        /// <returns></returns>
        ILog GetLogger(string name);
    }
}
