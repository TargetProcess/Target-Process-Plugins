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
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class PluginRollingFileAppender : FileAppender
	{
		#region Fields

		private readonly string _accountName;
		private readonly string _profileName;
		private static readonly DateTime _date1970 = new DateTime(1970, 1, 1);
		private readonly RollingFileAppender.IDateTime _dateTime;
		private string _scheduledFilename;
		private DateTime _nextCheck = DateTime.MaxValue;
		private long _maxFileSize = 10485760L;
		private int _maxSizeRollBackups;
		private int _curSizeRollBackups;
		private int _countDirection = -1;
		private RollingFileAppender.RollingMode _rollingStyle = RollingFileAppender.RollingMode.Composite;
		private bool _rollDate = true;
		private bool _rollSize = true;
		private bool _staticLogFileName = true;
		private DateTime _now;
		private RollPoint _rollPoint;
		private string _baseFileName;

		#endregion

		#region Properties
		
		protected bool IsTemplate
		{
			get { return string.IsNullOrEmpty(_accountName) || string.IsNullOrEmpty(_profileName); }
		}

		public bool Closed { get; set; }

		public string Folder { get; set; }

		public string DatePattern { get; set; }

		public int MaxSizeRollBackups
		{
			get { return _maxSizeRollBackups; }
			set { _maxSizeRollBackups = value; }
		}

		public long MaxFileSize
		{
			get { return _maxFileSize; }
			set { _maxFileSize = value; }
		}

		public string MaximumFileSize
		{
			get { return _maxFileSize.ToString(NumberFormatInfo.InvariantInfo); }
			set { _maxFileSize = OptionConverter.ToFileSize(value, _maxFileSize + 1L); }
		}

		public int CountDirection
		{
			get { return _countDirection; }
			set { _countDirection = value; }
		}

		public RollingFileAppender.RollingMode RollingStyle
		{
			get { return _rollingStyle; }
			set
			{
				_rollingStyle = value;
				switch (_rollingStyle)
				{
					case RollingFileAppender.RollingMode.Once:
						_rollDate = false;
						_rollSize = false;
						AppendToFile = false;
						break;
					case RollingFileAppender.RollingMode.Size:
						_rollDate = false;
						_rollSize = true;
						break;
					case RollingFileAppender.RollingMode.Date:
						_rollDate = true;
						_rollSize = false;
						break;
					case RollingFileAppender.RollingMode.Composite:
						_rollDate = true;
						_rollSize = true;
						break;
				}
			}
		}

		public bool StaticLogFileName
		{
			get { return _staticLogFileName; }
			set { _staticLogFileName = value; }
		}

		#endregion

		#region Constructor(s)
		
		public PluginRollingFileAppender()
		{
			_dateTime = new DefaultDateTime();
		}

		public PluginRollingFileAppender(PluginRollingFileAppender baseAppender, IActivityLogPathProvider path,
		                                 string accountName, string profileName) : this()
		{
			_accountName = accountName;
			_profileName = profileName;
			Initialize(baseAppender, path);
		}

		#endregion

		private void Initialize(PluginRollingFileAppender baseAppender, IActivityLogPathProvider path)
		{
			Folder = baseAppender.Folder;
			AppendToFile = baseAppender.AppendToFile;
			CountDirection = baseAppender.CountDirection;
			DatePattern = CsvLayout.DATE_TIME_FORMAT;
			Encoding = baseAppender.Encoding;
			ErrorHandler = baseAppender.ErrorHandler;
			File = GetFileName(_accountName, _profileName, baseAppender.File.GetFileName(), path);
			ImmediateFlush = baseAppender.ImmediateFlush;
			Layout = new CsvLayout();
			LockingModel = new MinimalLock();
			MaxFileSize = baseAppender.MaxFileSize;
			MaxSizeRollBackups = baseAppender.MaxSizeRollBackups;
			MaximumFileSize = baseAppender.MaximumFileSize;
			Name = path.GetLogPathFor(_accountName, _profileName, baseAppender.Name.GetFileName());
			RollingStyle = baseAppender.RollingStyle;
			StaticLogFileName = baseAppender.StaticLogFileName;
			Threshold = baseAppender.Threshold;
		}

		private string GetFileName(string accountName, string profileName, string fileName, IActivityLogPathProvider path)
		{
			return Folder.Combine(path.GetLogPathFor(accountName, profileName, fileName));
		}

		public override void ActivateOptions()
		{
			if (IsTemplate)
			{
				return;
			}

			_now = _dateTime.Now;
			_rollPoint = ComputeCheckPeriod(ActivityLogFile.FileDatePattern);
			if (_rollPoint == RollPoint.InvalidRollPoint)
			{
				throw new ArgumentException("Invalid RollPoint, unable to parse [" + ActivityLogFile.FileDatePattern + "]");
			}
			_nextCheck = NextCheckDate(_now, _rollPoint);

			if (SecurityContext == null)
			{
				SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}

			using (SecurityContext.Impersonate(this))
			{
				File = ConvertToFullPath(File.Trim());

				_baseFileName = File;
			}

			if (_rollDate && File != null && _scheduledFilename == null)
			{
				_scheduledFilename = ActivityLogFile.ComposeFileName(File, _now);
			}

			ExistingInit();

			base.ActivateOptions();

			Folder = ConvertToFullPath(Folder);
			Closed = false;
		}

		protected override void OnClose()
		{
			base.OnClose();

			Closed = true;
		}

		protected override void SetQWForFiles(TextWriter writer)
		{
			QuietWriter = new CountingQuietTextWriter(writer, ErrorHandler);
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			AdjustFileBeforeAppend(loggingEvent.TimeStamp);

			base.Append(loggingEvent);
		}

		protected override void Append(LoggingEvent[] loggingEvents)
		{
			AdjustFileBeforeAppend(loggingEvents.OrderBy(x => x.TimeStamp).Select(x => x.TimeStamp).First());

			base.Append(loggingEvents);
		}

		protected virtual void AdjustFileBeforeAppend(DateTime timeStamp)
		{
			var now = timeStamp;

			if (_rollDate)
			{
				if (now >= _nextCheck)
				{
					_now = now;
					_nextCheck = NextCheckDate(_now, _rollPoint);
					RollOverTime(true);
				}
			}

			if (!_rollSize || (File == null || ((CountingQuietTextWriter) QuietWriter).Count < _maxFileSize))
			{
				return;
			}

			RollOverSize(timeStamp);
		}

		protected override void OpenFile(string fileName, bool append)
		{
			lock (this)
			{
				fileName = GetNextOutputFileName(fileName);

				var local0 = 0L;

				if (append)
				{
					using (SecurityContext.Impersonate(this))
					{
						if (System.IO.File.Exists(fileName))
						{
							local0 = new FileInfo(fileName).Length;
						}
					}
				}
				else if (LogLog.IsErrorEnabled && (_maxSizeRollBackups != 0 && FileExists(fileName)))
				{
					LogLog.Error("RollingFileAppender: INTERNAL ERROR. Append is False but OutputFile [" + fileName + "] already exists.");
				}

				if (!_staticLogFileName)
				{
					_scheduledFilename = fileName;
				}

				base.OpenFile(fileName, append);

				((CountingQuietTextWriter) QuietWriter).Count = local0;
			}
		}

		protected string GetNextOutputFileName(string fileName)
		{
			if (!_staticLogFileName)
			{
				fileName = fileName.Trim();
				if (_rollDate)
				{
					fileName = ActivityLogFile.ComposeFileName(fileName, _now);
				}
				if (_countDirection >= 0)
				{
					fileName = ActivityLogFile.ComposeFileName(fileName, _dateTime.Now) + '.' + _curSizeRollBackups;
				}
			}
			return fileName;
		}

		private void DetermineCurSizeRollBackups()
		{
			_curSizeRollBackups = 0;

			string str;
			string baseFile;

			using (SecurityContext.Impersonate(this))
			{
				str = Path.GetFullPath(_baseFileName);
				baseFile = Path.GetFileName(str);
			}

			var existingFiles = GetExistingFiles(str);

			InitializeRollBackups(baseFile, existingFiles);

			LogLog.Debug("RollingFileAppender: curSizeRollBackups starts at [" + _curSizeRollBackups + "]");
		}

		private IEnumerable<string> GetExistingFiles(string baseFilePath)
		{
			var list = new List<string>();
			string path;

			using (SecurityContext.Impersonate(this))
			{
				var fullPath = Path.GetFullPath(baseFilePath);

				path = Path.GetDirectoryName(fullPath);

				if (Directory.Exists(path))
				{
					var fileName = Path.GetFileName(fullPath);
					var files = Directory.GetFiles(path, ActivityLogFile.GetWildcartPatternFor(fileName));

					list.AddRange(files.Select(Path.GetFileName));
				}
			}

			LogLog.Debug("RollingFileAppender: Searched for existing files in [" + path + "]");

			return list;
		}

		private void RollOverIfDateBoundaryCrossing()
		{
			if (!_staticLogFileName || !_rollDate || !FileExists(_baseFileName))
			{
				return;
			}

			DateTime lastWriteTime;
			using (SecurityContext.Impersonate(this))
			{
				lastWriteTime = System.IO.File.GetLastWriteTime(_baseFileName);
			}

			LogLog.Debug("RollingFileAppender: [" + lastWriteTime.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo) + "] vs. [" + _now.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo) + "]");
			if (lastWriteTime.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo)
				.Equals(_now.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo)))
			{
				return;
			}

			_scheduledFilename = ActivityLogFile.ComposeFileName(_baseFileName, lastWriteTime);
			LogLog.Debug("RollingFileAppender: Initial roll over to [" + _scheduledFilename + "]");
			RollOverTime(false);
			LogLog.Debug("RollingFileAppender: curSizeRollBackups after rollOver at [" + _curSizeRollBackups + "]");
		}

		protected void ExistingInit()
		{
			DetermineCurSizeRollBackups();
			RollOverIfDateBoundaryCrossing();

			if (AppendToFile)
			{
				return;
			}

			bool flag;
			var nextOutputFileName = GetNextOutputFileName(_baseFileName);

			using (SecurityContext.Impersonate(this))
			{
				flag = System.IO.File.Exists(nextOutputFileName);
			}

			if (!flag)
			{
				return;
			}

			if (_maxSizeRollBackups == 0)
			{
				LogLog.Debug("RollingFileAppender: Output file [" + nextOutputFileName +
				             "] already exists. MaxSizeRollBackups is 0; cannot roll. Overwriting existing file.");
			}
			else
			{
				LogLog.Debug("RollingFileAppender: Output file [" + nextOutputFileName +
				             "] already exists. Not appending to file. Rolling existing file out of the way.");

				RollOverRenameFiles(nextOutputFileName, _dateTime.Now);
			}
		}

		private void InitializeFromOneFile(string baseFile, string curFileName)
		{
			if (!curFileName.StartsWith(baseFile.GetFileNameWithoutExtension()) || curFileName.Equals(baseFile))
			{
				return;
			}

			var num1 = curFileName.LastIndexOf(".");

			if (-1 == num1)
			{
				return;
			}

			if (_staticLogFileName)
			{
				var num2 = curFileName.Length - num1;
				var datePart = ActivityLogFile.GetDatePart(curFileName);

				if (baseFile.Length + num2 != curFileName.Length - datePart.Length - 1)
				{
					return;
				}
			}

			if (_rollDate && !_staticLogFileName)
			{
				if (!curFileName.StartsWith(baseFile + _dateTime.Now.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo)))
				{
					LogLog.Debug("RollingFileAppender: Ignoring file [" + curFileName + "] because it is from a different date period");
					return;
				}
			}

			try
			{
				int val;

				if (SystemInfo.TryParse(curFileName.Substring(num1 + 1), out val) && val > _curSizeRollBackups)
				{
					if (0 != _maxSizeRollBackups)
					{
						if (-1 == _maxSizeRollBackups)
						{
							_curSizeRollBackups = val;
						}
						else if (_countDirection >= 0)
						{
							_curSizeRollBackups = val;
						}
						else if (val <= _maxSizeRollBackups)
						{
							_curSizeRollBackups = val;
						}
					}

					LogLog.Debug("RollingFileAppender: File name [" + curFileName + "] moves current count to [" + _curSizeRollBackups + "]");
				}
			}
			catch (FormatException)
			{
				LogLog.Debug("RollingFileAppender: Encountered a backup file not ending in .x [" + curFileName + "]");
			}
		}

		private void InitializeRollBackups(string baseFile, IEnumerable<string> arrayFiles)
		{
			if (null == arrayFiles)
			{
				return;
			}

			var baseFile1 = baseFile.ToLower(CultureInfo.InvariantCulture);

			foreach (var str in arrayFiles)
			{
				InitializeFromOneFile(baseFile1, str.ToLower(CultureInfo.InvariantCulture));
			}
		}

		private RollPoint ComputeCheckPeriod(string datePattern)
		{
			var str1 = _date1970.ToString(datePattern, DateTimeFormatInfo.InvariantInfo);

			for (var index = 0; index <= 5; ++index)
			{
				var str2 = NextCheckDate(_date1970, (RollPoint)index).ToString(datePattern, DateTimeFormatInfo.InvariantInfo);

				LogLog.Debug("RollingFileAppender: Type = [" + index + "], r0 = [" + str1 + "], r1 = [" + str2 + "]");

				if (!str1.Equals(str2))
				{
					return (RollPoint)index;
				}
			}

			return RollPoint.InvalidRollPoint;
		}

		protected void RollOverTime(bool fileIsOpen)
		{
			if (_staticLogFileName)
			{
				var str = _now.ToString(ActivityLogFile.FileDatePattern, DateTimeFormatInfo.InvariantInfo);

				if (_scheduledFilename.Equals(File + str))
				{
					ErrorHandler.Error("Compare " + _scheduledFilename + " : " + File + str);
					return;
				}

				if (fileIsOpen)
				{
					CloseFile();
				}

				for (var index = 1; index <= _curSizeRollBackups; ++index)
				{
					RollFile(File + '.' + index, _scheduledFilename + '.' + index);
				}

				RollFile(File, _scheduledFilename);
			}

			_curSizeRollBackups = 0;
			_scheduledFilename = ActivityLogFile.ComposeFileName(File, _now);

			if (!fileIsOpen)
			{
				return;
			}

			SafeOpenFile(_baseFileName, false);
		}

		protected void RollFile(string fromFile, string toFile)
		{
			if (FileExists(fromFile))
			{
				DeleteFile(toFile);

				try
				{
					LogLog.Debug("RollingFileAppender: Moving [" + fromFile + "] -> [" + toFile + "]");

					using (SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fromFile, toFile);
					}
				}
				catch (Exception ex)
				{
					ErrorHandler.Error("Exception while rolling file [" + fromFile + "] -> [" + toFile + "]", ex, ErrorCode.GenericFailure);
				}
			}
			else
			{
				LogLog.Warn("RollingFileAppender: Cannot RollFile [" + fromFile + "] -> [" + toFile + "]. Source does not exist");
			}
		}

		protected bool FileExists(string path)
		{
			using (SecurityContext.Impersonate(this))
			{
				return System.IO.File.Exists(path);
			}
		}

		protected void DeleteFile(string fileName)
		{
			if (!FileExists(fileName))
			{
				return;
			}

			var path = fileName;
			var destFileName = string.Concat(new object[]
			                                    	{
			                                    		fileName,
			                                    		".",
			                                    		Environment.TickCount,
			                                    		".DeletePending"
			                                    	});
			try
			{
				using (SecurityContext.Impersonate(this))
				{
					System.IO.File.Move(fileName, destFileName);
				}

				path = destFileName;
			}
			catch (Exception ex)
			{
				LogLog.Debug("RollingFileAppender: Exception while moving file to be deleted [" + fileName + "] -> [" + destFileName + "]", ex);
			}
			try
			{
				using (SecurityContext.Impersonate(this))
				{
					System.IO.File.Delete(path);
				}

				LogLog.Debug("RollingFileAppender: Deleted file [" + fileName + "]");
			}
			catch (Exception ex)
			{
				if (path == fileName)
				{
					ErrorHandler.Error("Exception while deleting file [" + path + "]", ex, ErrorCode.GenericFailure);
				}
				else
				{
					LogLog.Debug("RollingFileAppender: Exception while deleting temp file [" + path + "]", ex);
				}
			}
		}

		protected void RollOverSize(DateTime timeStamp)
		{
			CloseFile();

			LogLog.Debug("RollingFileAppender: rolling over count [" + ((CountingQuietTextWriter)QuietWriter).Count + "]");
			LogLog.Debug("RollingFileAppender: maxSizeRollBackups [" + _maxSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: curSizeRollBackups [" + _curSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: countDirection [" + _countDirection + "]");

			RollOverRenameFiles(File, timeStamp);

			if (!_staticLogFileName && _countDirection >= 0)
			{
				++_curSizeRollBackups;
			}

			SafeOpenFile(_baseFileName, false);
		}

		protected void RollOverRenameFiles(string baseFileName, DateTime timeStamp)
		{
			if (_maxSizeRollBackups == 0)
			{
				return;
			}

			var directoryName = baseFileName.GetDirectoryName();
			var logFiles = new DirectoryInfo(directoryName)
				.GetFiles(ActivityLogFile.GetWildcartPatternFor(baseFileName))
				.OrderBy(x => ActivityLogFile.GetOrder(x.Name))
				.ToArray();
			var backupFileName = ActivityLogFile.ComposeFileName(baseFileName, timeStamp);

			if (_countDirection < 0)
			{
				if (_curSizeRollBackups == _maxSizeRollBackups)
				{
					DeleteFile(logFiles[_maxSizeRollBackups - 1].FullName);
					--_curSizeRollBackups;
				}

				for (var index = _curSizeRollBackups - 1; index >= 0; --index)
				{
					var file = logFiles[index];
					var newFileName = file.Name.GetFileNameWithoutExtension() + '.' + (index + 2);

					RollFile(file.FullName, directoryName.Combine(newFileName));
				}

				++_curSizeRollBackups;

				RollFile(baseFileName, directoryName.Combine(backupFileName + ".1"));
			}
			else
			{
				if (_curSizeRollBackups >= _maxSizeRollBackups && _maxSizeRollBackups > 0)
				{
					var num = _curSizeRollBackups - _maxSizeRollBackups;

					if (_staticLogFileName)
					{
						++num;
					}

					DeleteFile(logFiles[num].FullName);
				}

				if (_staticLogFileName)
				{
					++_curSizeRollBackups;

					RollFile(baseFileName, backupFileName + '.' + _curSizeRollBackups);
				}
			}
		}

		protected DateTime NextCheckDate(DateTime currentDateTime, RollPoint rollPoint)
		{
			var dateTime = currentDateTime;

			switch (rollPoint)
			{
				case RollPoint.TopOfMinute:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(1.0);
					break;
				case RollPoint.TopOfHour:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(-dateTime.Minute);
					dateTime = dateTime.AddHours(1.0);
					break;
				case RollPoint.HalfDay:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(-dateTime.Minute);
					if (dateTime.Hour < 12)
					{
						dateTime = dateTime.AddHours(12 - dateTime.Hour);
						break;
					}
					dateTime = dateTime.AddHours(-dateTime.Hour);
					dateTime = dateTime.AddDays(1.0);
					break;
				case RollPoint.TopOfDay:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(-dateTime.Minute);
					dateTime = dateTime.AddHours(-dateTime.Hour);
					dateTime = dateTime.AddDays(1.0);
					break;
				case RollPoint.TopOfWeek:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(-dateTime.Minute);
					dateTime = dateTime.AddHours(-dateTime.Hour);
					dateTime = dateTime.AddDays(7 - (int)dateTime.DayOfWeek);
					break;
				case RollPoint.TopOfMonth:
					dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
					dateTime = dateTime.AddSeconds(-dateTime.Second);
					dateTime = dateTime.AddMinutes(-dateTime.Minute);
					dateTime = dateTime.AddHours(-dateTime.Hour);
					dateTime = dateTime.AddDays(1 - dateTime.Day);
					dateTime = dateTime.AddMonths(1);
					break;
			}
			return dateTime;
		}

		private class DefaultDateTime : RollingFileAppender.IDateTime
		{
			public DateTime Now
			{
				get { return CurrentDate.Value; }
			}
		}

		protected enum RollPoint
		{
			InvalidRollPoint = -1,
			TopOfMinute = 0,
			TopOfHour = 1,
			HalfDay = 2,
			TopOfDay = 3,
			TopOfWeek = 4,
			TopOfMonth = 5,
		}
	}
}