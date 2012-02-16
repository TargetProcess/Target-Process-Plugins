// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using log4net;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
	internal class Log4NetActivityLog : IActivityLog
	{
		private readonly string _accountName;
		private readonly string _profileName;
		private readonly IDictionary<ActivityType, string> _loggersNames;

		private readonly ILog4NetFileRepository _log4NetFileRepository;
		private readonly IActivityLogPathProvider _path;

		public Log4NetActivityLog(string accountName, string profileName)
		{
			_accountName = accountName;
			_profileName = profileName;
			_log4NetFileRepository = ObjectFactory.GetInstance<ILog4NetFileRepository>();
			_path = ObjectFactory.GetInstance<IActivityLogPathProvider>();

			_loggersNames = new Dictionary<ActivityType, string>
			                	{
			                		{
			                			ActivityType.Errors,
			                			ActivityLoggerRegistry.ErrorLoggerName
			                			},
			                		{
			                			ActivityType.All,
			                			ActivityLoggerRegistry.ActivityLoggerName
			                			}
			                	};
		}

		public virtual ActivityDto GetBy(ActivityFilter filter)
		{
			var logger = GetLogger(filter.Type);

			var records = _log4NetFileRepository.GetActivityRecordsFor(logger, filter);

			return new ActivityDto
			{
				Type = filter.Type,
				Records = records
					.Reverse()
					.ToList()
			};
		}

		public virtual void Remove()
		{
			var loggers = _loggersNames.Select(x => GetLogger(x.Key)).ToList();

			CloseAppendersFor(loggers);
			_log4NetFileRepository.RemoveFoldersFor(loggers);
		}

		public void ClearBy(ActivityFilter filter)
		{
			var logger = GetLogger(filter.Type);

			CloseAppendersFor(logger);
			_log4NetFileRepository.RemoveFilesFor(logger);
		}

		public bool CheckForErrors()
		{
			var logger = GetLogger(ActivityType.Errors);

			return _log4NetFileRepository.RecordsExist(logger);
		}

		private static void CloseAppendersFor(Logger logger)
		{
			CloseAppendersFor(new[] { logger });
		}

		private static void CloseAppendersFor(IEnumerable<Logger> loggers)
		{
			foreach (var logger in loggers)
			{
				logger.CloseNestedAppenders();
			}
		}

		private Logger GetLogger(ActivityType type)
		{
			var loggerName = _loggersNames[type];

			return (Logger)LogManager.GetLogger(_path.GetLogPathFor(_accountName, _profileName, loggerName)).Logger;
		}
	}
}