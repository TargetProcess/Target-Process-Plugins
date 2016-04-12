// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
	internal interface ILog4NetFileRepository
	{
		IEnumerable<ActivityLogRecord> GetActivityRecordsFor(Logger logger, ActivityFilter filter);

		void RemoveFoldersFor(IEnumerable<Logger> loggers);

		void RemoveFilesFor(Logger logger);

		bool RecordsExist(Logger logger);
	}
}