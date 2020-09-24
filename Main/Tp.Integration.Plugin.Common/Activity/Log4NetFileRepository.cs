// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Utils.Csv;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
    internal class Log4NetFileRepository : ILog4NetFileRepository
    {
        private readonly Locker _locker;
        private readonly ILogManager _logManager;

        public Log4NetFileRepository(ILogManager logManager, Locker locker)
        {
            _logManager = logManager;
            _locker = locker;
        }

        #region Get Records By

        public virtual IEnumerable<ActivityLogRecord> GetActivityRecordsFor(Logger logger, ActivityFilter filter)
        {
            IEnumerable<string> filesList = GetSuitableFiles(logger, filter);

            var result = new List<ActivityLogRecord>();

            foreach (string file in filesList)
            {
                result.AddRange(GetRecordsFromFile(filter, file));
            }

            return result;
        }

        protected virtual IEnumerable<string> GetSuitableFiles(Logger logger, ActivityFilter filter)
        {
            string[] files = GetFilesForLogger(logger);
            int[] suitableItems = FilterFileNames(files, filter);

            if (!suitableItems.Any())
            {
                return Enumerable.Empty<string>();
            }

            int firstItemIndex = suitableItems.Min(x => x);
            int lastItemIndex = suitableItems.Max(x => x) + 1;

            var filesList = new List<string>();
            if (!filter.DateRange.StartDate.HasValue)
            {
                filesList.Add(files[firstItemIndex]);
            }
            else
            {
                filesList.AddRange(files.Skip(firstItemIndex).Take(lastItemIndex - firstItemIndex + 1));
            }

            return filesList;
        }

        protected virtual string[] GetFilesForLogger(Logger logger)
        {
            string logFileName = GetLogFilePathFromLogger(logger);

            if (string.IsNullOrEmpty(logFileName))
            {
                return Enumerable.Empty<string>().ToArray();
            }

            var directory = new DirectoryInfo(logFileName.GetDirectoryName());

            if (!directory.Exists)
            {
                return Enumerable.Empty<string>().ToArray();
            }

            return new[] { logFileName }
                .Concat(directory
                    .GetFiles(ActivityLogFile.GetWildcartPatternFor(logFileName))
                    .Select(x => x.FullName))
                .OrderBy(ActivityLogFile.GetOrder)
                .ToArray();
        }

        protected virtual string GetLogFilePathFromLogger(Logger logger)
        {
            PluginRollingFileAppender appender = logger?.Appenders.OfType<PluginRollingFileAppender>().FirstOrDefault();
            return appender != null ? appender.File : String.Empty;
        }

        private static int[] FilterFileNames(IEnumerable<string> files, ActivityFilter filter)
        {
            return files.Select((x, i) => new { Item = x, Index = i })
                .Where(x => filter.DateRange.IsInRangeIncludingEndDate(ActivityLogFile.GetDate(x.Item)))
                .Select(x => x.Index)
                .ToArray();
        }

        protected virtual IEnumerable<ActivityLogRecord> GetRecordsFromFile(ActivityFilter filter, string file)
        {
            return GetFromLog(file, Enumerable.Empty<ActivityLogRecord>(),
                (csvReader, defaultValue) =>
                {
                    var records = new List<ActivityLogRecord>();
                    while (csvReader.Read())
                    {
                        var record = GetActivityRecord(csvReader);
                        if (filter.DateRange.IsInRangeIncludingEndDate(record.DateTime))
                        {
                            records.Add(record);
                        }
                    }

                    return records;
                });
        }

        private static ActivityLogRecord GetActivityRecord(CsvReader csvReader)
        {
            return new ActivityLogRecord
            {
                DateTime = DateTime.ParseExact(csvReader[0], CsvLayout.DATE_TIME_FORMAT, CultureInfo.InvariantCulture),
                Level = csvReader[1],
                Message = csvReader[2],
                Details = csvReader[3]
            };
        }

        #endregion

        #region Remove Folders For

        public virtual void RemoveFoldersFor(IEnumerable<Logger> loggers)
        {
            IEnumerable<string> folders = loggers
                .SelectMany(GetLoggerFolders)
                .Distinct();

            RemoveFolders(folders);
        }

        protected virtual IEnumerable<string> GetLoggerFolders(IAppenderAttachable logger)
        {
            return logger.Appenders.OfType<PluginRollingFileAppender>().Select(x => x.File.GetDirectoryName());
        }

        protected virtual void RemoveFolders(IEnumerable<string> folders)
        {
            try
            {
                foreach (string folder in folders.Where(Directory.Exists))
                {
                    Directory.Delete(folder, true);
                }
            }
            catch (IOException e)
            {
                _logManager.GetLogger(GetType()).Error(
                    $"Failed to delete activity log. Folder : {string.Join(",", folders.ToArray())}. Reason : ", e);
            }
        }

        protected virtual IEnumerable<string> GetLoggerFiles(IAppenderAttachable logger)
        {
            return logger.Appenders.OfType<PluginRollingFileAppender>().Select(x => x.File);
        }

        #endregion

        #region Remove Files For

        public void RemoveFilesFor(Logger logger)
        {
            IEnumerable<string> files = GetLoggerFiles(logger)
                .SelectMany(GetFilesFromFolderByPattern)
                .Distinct();

            RemoveFiles(files);
        }

        protected virtual IEnumerable<string> GetFilesFromFolderByPattern(string fileName)
        {
            string folder = fileName.GetDirectoryName();
            string pattern = fileName.GetFileNameWithoutExtension() + "*";

            var directory = new DirectoryInfo(folder);
            if (directory.Exists)
            {
                return directory.GetFiles(pattern).Select(x => x.FullName);
            }

            return Enumerable.Empty<string>();
        }

        protected virtual void RemoveFiles(IEnumerable<string> files)
        {
            foreach (string file in files.Where(File.Exists))
            {
                File.Delete(file);
            }
        }

        #endregion

        public ActivityLogRecord GetLatestRecord(Logger logger)
        {
            var filter = new ActivityFilter { Type = ActivityType.Errors, DateRange = DateRange.Empty };
            return (from lf in GetLoggerFiles(logger)
                    where File.Exists(lf) && !new FileInfo(lf).Name.StartsWith("json.", StringComparison.OrdinalIgnoreCase)
                    select GetFromLog<ActivityLogRecord>(lf, null, (reader, _) => GetRecordsFromFile(filter, lf).Reverse().FirstOrDefault()))
                .FirstOrDefault(record => record != null);
        }

        private T GetFromLog<T>(string fileName, T defaultValue, Func<CsvReader, T, T> func)
        {
            if (!File.Exists(fileName))
            {
                return defaultValue;
            }

            var result = _locker.TryAcquireLockAndExecute(fileName, () =>
            {
                using (var reader = new StreamReader(fileName))
                using (var csvReader = new CsvReader(reader, CsvLayout.DELIMITER))
                {
                    return func(csvReader, defaultValue);
                }
            });

            return result.HasValue ? result.Value : defaultValue;
        }
    }
}
