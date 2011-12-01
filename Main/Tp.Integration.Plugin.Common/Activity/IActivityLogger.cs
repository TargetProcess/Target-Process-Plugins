// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Plugin.Common.Activity
{
	public interface IActivityLogger
	{
		void Debug(string message);

		void DebugFormat(string format, params object[] args);

		void Info(string message);

		void InfoFormat(string format, params object[] args);

		void Warn(string message);

		void WarnFormat(string format, params object[] args);

		void Error(string message);

		void Error(Exception ex);

		void Error(string message, Exception exception);

		void ErrorFormat(string format, params object[] args);

		void Fatal(string message);

		void Fatal(Exception ex);
	}
}