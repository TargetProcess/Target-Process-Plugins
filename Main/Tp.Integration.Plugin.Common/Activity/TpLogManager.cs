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
	internal class TpLogManager : ILogManager, ILogProvider
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
					? GetLogger(_path, name, _context)
			       	: LogManager.GetLogger(name);
		}

		public IEnumerable<ILog> GetActivityLoggers()
		{
			return GetActivityLoggers(_context);
		}

		public IEnumerable<ILog> GetActivityLoggers(IPluginContext context)
		{
			return ActivityLoggerRegistry.LoggersNames.Select(n => GetLogger(_path, n, context)).Concat(GetLogger(typeof(TpLogManager)));
		}

		private static ILog GetLogger(IActivityLogPathProvider path, string name, IPluginContext context)
		{
			return LogManager.GetLogger(path.GetLogPathFor(context.AccountName.Value, context.ProfileName.Value, name));
		}
	}
}