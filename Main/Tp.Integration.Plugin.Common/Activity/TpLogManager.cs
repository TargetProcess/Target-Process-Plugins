// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class TpLogManager : ILogManager
	{
		private readonly IActivityLogPathProvider _path;
		private readonly IPluginContext _context;

		public TpLogManager(IActivityLogPathProvider path, IPluginContext context)
		{
			_path = path;
			_context = context;
		}

		public ILog GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}

		public ILog GetLogger(string name)
		{
			return ActivityLoggerRegistry.IsKnownLogger(name)
			       	? LogManager.GetLogger(_path.GetLogPathFor(_context.AccountName.Value, _context.ProfileName.Value, name))
			       	: LogManager.GetLogger(name);
		}

		public IEnumerable<ILog> GetActivityLoggers()
		{
			return ActivityLoggerRegistry.LoggersNames.Select(GetLogger)
				.Concat(GetLogger(typeof(TpLogManager)));
		}
	}
}