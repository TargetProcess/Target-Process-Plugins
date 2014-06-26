// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using System.Reflection;
using StructureMap;
using Tp.Core;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
	internal class ActivityLoggerFactory : ILoggerFactory
	{
		private readonly ILoggerFactory _factory;
		private readonly Locker _locker;
		private readonly IActivityLogPathProvider _path;

		public ActivityLoggerFactory(ILoggerFactory factory, Locker locker)
		{
			_factory = factory;
			_locker = locker;
			_path = ObjectFactory.GetInstance<IActivityLogPathProvider>();
		}

		public Logger CreateLogger(string name)
		{
			var loggerName = _path.GetFileNameFrom(name);
			var accountName = _path.GetAccountNameFrom(name);
			var profileName = _path.GetProfileNameFrom(name);

			if (ActivityLoggerRegistry.IsKnownLogger(loggerName) && !string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(profileName))
			{
				var baseLogger = LoggerManager.GetLogger(Assembly.GetCallingAssembly(), loggerName) as Logger;
				if (baseLogger != null)
				{
					return GetLoggerFrom(baseLogger, name, accountName, profileName);
				}
			}

			return _factory.CreateLogger(name);
		}

		private Logger GetLoggerFrom(Logger logger, string fullName, string accountName, string profileName)
		{
			var result = new LoggerImpl(fullName)
			             	{
			             		Level = logger.Level,
			             		Additivity = logger.Additivity,
			             	};

			foreach (var appender in logger.Hierarchy.GetAppenders()
				.OfType<PluginRollingFileAppender>()
				.Where(x => x.Name.StartsWith(logger.Name))
				.Select(x => new PluginRollingFileAppender(x, _path, _locker, accountName, profileName)))
			{
				appender.ActivateOptions();
				result.AddAppender(appender);
			}

			return result;
		}
	}

	internal class LoggerImpl : Logger
	{
		public LoggerImpl(string name) : base(name)
		{
		}
	}
}