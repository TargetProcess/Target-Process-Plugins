// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Plugin.Common.Tests.Common.PluginCommand;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
	public class ActivityLoggingContext
	{
		private ILoggerFactory _log4NetFactory;

		public ActivityLoggingContext()
		{
			Loggers = new List<ILog>();
			Activities = new List<ActivityDto>();

			TransportMock = TransportMock.CreateWithoutStructureMapClear(typeof(SampleProfileSerialized).Assembly,
																			 new List<Assembly> { typeof(ExceptionThrownLocalMessage).Assembly },
																			 new[] { typeof(WhenAddANewProfileSpecs).Assembly });
			InitializeLogging();
			ObjectFactory.Configure(InitializeActivityLogging);

			var now = DateTime.Now;

			CurrentDate.Setup(() => now);
		}

		public void InitializeActivityLoggingMock()
		{
			ObjectFactory.Configure(x =>
			                        	{
			                        		x.For<FakeActivityLogger>().HybridHttpOrThreadLocalScoped().Use<FakeActivityLogger>();
			                        		x.Forward<FakeActivityLogger, IActivityLogger>();
			                        	});
		}

		protected void InitializeLogging()
		{
			var assembly = Assembly.GetAssembly(GetType());
			var configName = assembly.GetManifestResourceNames()
				.Where(x => x.EndsWith("Activity.log4net.cfg.xml", StringComparison.OrdinalIgnoreCase))
				.Single();
			using (var stream = assembly.GetManifestResourceStream(configName))
			{
				XmlConfigurator.Configure(stream);
			}

			var repository = ((Hierarchy)LoggerManager.GetRepository("log4net-default-repository"));
			_log4NetFactory = repository.LoggerFactory;
			new ActivityLogInitialization().Init();
		}

		public void SetLog4NetNativeFactory()
		{
			var repository = ((Hierarchy)LoggerManager.GetRepository("log4net-default-repository"));
			repository.LoggerFactory = _log4NetFactory;
		}

		protected virtual void InitializeActivityLogging(ConfigurationExpression x)
		{
			x.For<IActivityLogPathProvider>().HybridHttpOrThreadLocalScoped().Use<ActivityLogPathProvider>();
			x.For<ILogManager>().HybridHttpOrThreadLocalScoped().Use<TpLogManager>();
			x.For<Log4NetFileRepositoryMock>().Singleton().Use<Log4NetFileRepositoryMock>();
			x.Forward<Log4NetFileRepositoryMock, ILog4NetFileRepository>();
		}

		public TransportMock TransportMock { get; private set; }

		public IList<ILog> Loggers { get; set; }

		public IList<ActivityDto> Activities { get; set; }
	}
}
