// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Bus;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;
using Tp.Search.Model.Query;
using Tp.Search.Model.Utils;

namespace Tp.Search.StructureMap
{
	public class PluginRegistry : Registry
	{
		public PluginRegistry()
		{
			For<IDocumentIndexProvider>().Singleton().Use(c =>
				{
					var logger = c.GetInstance<IActivityLogger>();
					var setup = c.GetInstance<DocumentIndexSetup>();
					return new DocumentIndexProvider(logger.Debug, setup);
				});
			For<DocumentIndexSetup>().Singleton().Use(() =>
				{
					var folder = new PluginDataFolder();
					return new DocumentIndexSetup(indexPath: folder.Path, minStringLengthToSearch: 2, maxStringLengthIgnore: 60, aliveTimeoutInMinutes:20);
				});
			For<QueryEntityTypeProvider>().Singleton().Use<QueryEntityTypeProvider>();
			Forward<QueryEntityTypeProvider, IQueryResultFactory>();
			Forward<QueryEntityTypeProvider, IEntityTypeProvider>();
			For<IDocumentIdFactory>().Singleton().Use<DocumentIdFactory>();
			For<IEntityIndexer>().Singleton().Use<EntityIndexer>();
			For<AutomaticOnDemandProfileCreator>().Singleton().Use<AutomaticOnDemandProfileCreator>();
			Forward<AutomaticOnDemandProfileCreator, ITargetProcessMessageWhenNoProfilesHandler>();
			Forward<AutomaticOnDemandProfileCreator, ITargetProcessConditionalMessageRouter>();
			For<QueryParser>().Singleton().Use<QueryParser>();
			For<QueryPlanBuilder>().Use<QueryPlanBuilder>();
			For<QueryPlanExecutor>().Use<QueryPlanExecutor>();
			For<QueryRunner>().Use<QueryRunner>();
			For<TextOperations>().Singleton().Use<TextOperations>();
		}
	}
}