// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	public abstract class SqlPersisterSpecBase
	{
		public const string PluginConnectionString = @"Data Source=(local);Initial Catalog=TargetProcessTest;user id=sa;password=sa";

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(InitializeContainer);
			SetupDB();
			OnInit();
		}

		protected virtual void OnInit()
		{
		}

		protected virtual void InitializeContainer(IInitializationExpression initializationExpression)
		{
			initializationExpression.AddRegistry<PluginStorageWithSqlPersisterMockRegistry>();
		}

		[TearDown]
		public void Destroy()
		{
			var configuration = ObjectFactory.GetInstance<IDatabaseConfiguration>();
			using (var context = new PluginDatabaseModelDataContext(configuration.ConnectionString))
			{
				foreach (var plugin in context.Plugins)
				{
					context.Plugins.DeleteOnSubmit(plugin);
				}
				context.SubmitChanges();
			}
		}

		protected void SetupDB()
		{
			ObjectFactory.GetInstance<IDatabaseConfiguration>().Stub(x => x.ConnectionString).Return(PluginConnectionString);
		}
	}
}