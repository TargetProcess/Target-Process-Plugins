// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NUnit.Framework;
using StructureMap;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
	[TestFixture]
	public abstract class ActivityTestBase
	{
		[SetUp]
		public virtual void Init()
		{
			ObjectFactory.Configure(x => x.For<ActivityLoggingContext>().Singleton().Use(new ActivityLoggingContext()));
			OnInit();
		}

		protected virtual void OnInit()
		{
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			CurrentDate.SetCurrentDateStrategy(() => DateTime.Now);
		}

		[TearDown]
		public void TearDown()
		{
			var activityLoggingContext = ObjectFactory.GetInstance<ActivityLoggingContext>();
			activityLoggingContext.Loggers.Clear();
			activityLoggingContext.Activities.Clear();
			activityLoggingContext.SetLog4NetNativeFactory();
			ObjectFactory.GetInstance<Log4NetFileRepositoryMock>().RemovedFiles.Clear();
			ObjectFactory.GetInstance<Log4NetFileRepositoryMock>().RemovedFolders.Clear();
		}
	}
}