// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Logging;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
    public class ActivityDtoMock
    {
        public ActivityDto ActivityDto { get; set; }

        public string FileName { get; set; }
    }

    internal class Log4NetFileRepositoryMock : Log4NetFileRepository
    {
        #region Inner storage

        private static DateTime _now = CurrentDate.Value;

        private readonly IList<ActivityDtoMock> _activityInnerStorage = new List<ActivityDtoMock>
        {
            new ActivityDtoMock
            {
                FileName = "Activity.txt",
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime = _now,
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            },
            new ActivityDtoMock
            {
                FileName = ComposeLogFileName("Activity", 1),
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime =
                                        _now.AddMonths(-1),
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            },
            new ActivityDtoMock
            {
                FileName = ComposeLogFileName("Activity", 2),
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime =
                                        _now.AddMonths(-2),
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            }
        };

        private readonly IList<ActivityDtoMock> _errorInnerStorage = new List<ActivityDtoMock>
        {
            new ActivityDtoMock
            {
                FileName = "Error.txt",
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime = _now,
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            },
            new ActivityDtoMock
            {
                FileName = ComposeLogFileName("Error", 1),
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime =
                                        _now.AddMonths(-1),
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            },
            new ActivityDtoMock
            {
                FileName = ComposeLogFileName("Error", 2),
                ActivityDto = new ActivityDto
                {
                    Records =
                        Enumerable.Range(0, 3).Select(
                            x =>
                                new ActivityLogRecord
                                {
                                    DateTime =
                                        _now.AddMonths(-2),
                                    Level = "ERROR",
                                    Message =
                                        "This is test message"
                                })
                }
            }
        };

        #endregion

        private readonly List<string> _removedFiles;

        private readonly List<string> _removedFolders;

        public int Page { get; set; }

        public int NumberOfRecords { get; set; }

        public DateTime? Date { get; set; }

        public List<string> RemovedFolders => _removedFolders;

        public List<string> RemovedFiles => _removedFiles;

        public Log4NetFileRepositoryMock(ILogManager logManager, Locker locker) : base(logManager, locker)
        {
            _removedFiles = new List<string>();
            _removedFolders = new List<string>();
        }

        public override IEnumerable<ActivityLogRecord> GetActivityRecordsFor(Logger logger, ActivityFilter filter)
        {
            var filesList = GetSuitableFiles(logger, filter);

            var result = new List<ActivityLogRecord>();

            var store = GetCurrentStorage(logger);

            foreach (var file in filesList)
            {
                result.AddRange(store.Where(x => x.FileName == file).SelectMany(x => x.ActivityDto.Records));
            }

            return result;
        }

        protected override string[] GetFilesForLogger(Logger logger)
        {
            var storage = GetCurrentStorage(logger);

            return storage.Select(s => s.FileName).ToArray();
        }

        private IEnumerable<ActivityDtoMock> GetCurrentStorage(Logger logger)
        {
            return logger.Name.Contains("Error") ? _errorInnerStorage : _activityInnerStorage;
        }

        protected override IEnumerable<string> GetFilesFromFolderByPattern(string fileName)
        {
            return Enumerable.Range(0, 3).Select(x => x == 0 ? fileName : fileName + "." + x);
        }

        protected override void RemoveFolders(IEnumerable<string> folders)
        {
            _removedFolders.AddRange(folders);
        }

        protected override void RemoveFiles(IEnumerable<string> files)
        {
            _removedFiles.AddRange(files);
        }

        private static string ComposeLogFileName(string baseFileName, int number)
        {
            return $"{baseFileName}.{_now.AddMonths(-number).AddSeconds(-1).ToString(ActivityLogFile.FileDatePattern)}.txt.{number}";
        }
    }
}
