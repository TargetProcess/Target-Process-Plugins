// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.Unicast;
using StructureMap;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class ActivityLogInitialization : IWantCustomInitialization
	{
		private static IUnicastBus Bus
		{
			get { return ObjectFactory.GetInstance<IUnicastBus>(); }
		}

		private static IActivityLogger Log
		{
			get { return ObjectFactory.GetInstance<IActivityLogger>(); }
		}

		public void Init()
		{
			ConfigureLogging();

			ConfigureUnhandledExceptionsLogging();
		}

		private static void ConfigureUnhandledExceptionsLogging()
		{
			Bus.UnhandledExceptionCaught += (sender, e) => Log.Error(e.Exception);
		}

		private static void ConfigureLogging()
		{
			if (LogManager.GetAllRepositories().Length > 0)
			{
				var repository = ((Hierarchy) LoggerManager.GetRepository("log4net-default-repository"));

				repository.LoggerFactory = new ActivityLoggerFactory(repository.LoggerFactory);
			}
		}
	}
}