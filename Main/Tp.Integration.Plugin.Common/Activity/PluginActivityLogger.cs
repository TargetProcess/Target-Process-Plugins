// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class PluginActivityLogger : IActivityLogger
	{
		private readonly ILogManager _logManager;

		public PluginActivityLogger(ILogManager logManager)
		{
			_logManager = logManager;
		}

		public void Info(string message)
		{
			Log(log => log.Info(message));
		}

		public void Warn(string message)
		{
			Log(log => log.Warn(message));
		}

		public void Debug(string message)
		{
			Log(log => log.Debug(message));
		}

		public void Error(string message)
		{
			Log(log => log.Error(message));
		}

		public void Error(Exception ex)
		{
			Log(log => log.Error(ex.Message, ex));
		}

		public void Fatal(string message)
		{
			Log(log => log.Fatal(message));
		}

		public void Fatal(Exception ex)
		{
			Log(log => log.Fatal(ex.Message, ex));
		}

		public void WarnFormat(string format, params object[] args)
		{
			Warn(string.Format(format, args));
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Error(string.Format(format, args));
		}

		public void DebugFormat(string format, params object[] args)
		{
			Debug(string.Format(format, args));
		}

		public void Error(string message, Exception exception)
		{
			Log(log => log.Error(message, exception));
		}

		public void InfoFormat(string format, params object[] args)
		{
			Info(string.Format(format, args));
		}

		protected virtual void Log(Action<ILog> action)
		{
			foreach (var log in _logManager.GetActivityLoggers())
			{
				action(log);
			}
		}
	}
}