// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Configuration;
using System.Xml;
using log4net;
using log4net.Config;

namespace Tp.Core
{
	public class TpLogManager : ITpLogManager
	{
		private static TpLogManager _instance;
		private static ILog _defaultLog;

		static TpLogManager()
		{
			var config = (XmlElement) ConfigurationManager.GetSection("log4net");
			XmlConfigurator.Configure(config);
		}

		public static ILog GetLogger(Type type)
		{
			return LogManager.GetLogger(type);
		}

		public static TpLogManager Instance
		{
			get { return _instance ?? (_instance = new TpLogManager()); }
		}

		public ILog GetLog(string loggerName)
		{
			return LogManager.GetLogger(loggerName);
		}

		public ILog DefaultLog
		{
			get { return _defaultLog ?? (_defaultLog = LogManager.GetLogger("General")); }
		}
	}
}