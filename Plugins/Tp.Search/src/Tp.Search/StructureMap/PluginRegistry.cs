// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;
using Tp.Integration.Messages.SerializationPatches;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus;
using Tp.Search.Bus.Serialization;
using Tp.Search.Bus.Utils;
using Tp.Search.Config;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;
using Tp.Search.Model.Optimization;
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
					var logger = c.GetInstance<IActivityLoggerFactory>();
					var setup = c.GetInstance<DocumentIndexSetup>();
					var metadata = c.GetInstance<DocumentIndexMetadata>();
					var optimizeHintFactory = c.GetInstance<DocumentIndexOptimizeHintFactory>();
					return new DocumentIndexProvider(logger, setup, metadata, optimizeHintFactory);
				});
			For<DocumentIndexSetup>().Singleton().Use(() =>
				{
					var config = new DocumentIndexSetupConfig();
					return config.Load();
				});
			For<QueryEntityTypeProvider>().Singleton().Use<QueryEntityTypeProvider>();
			Forward<QueryEntityTypeProvider, IQueryResultFactory>();
			Forward<QueryEntityTypeProvider, IEntityTypeProvider>();
			For<DocumentIndexMetadata>().Singleton().Use<DocumentIndexMetadata>();
			For<IDocumentIdFactory>().Singleton().Use<DocumentIdFactory>();
			For<IIndexDataFactory>().Singleton().Use<IndexDataFactory>();
			For<IEntityIndexer>().Singleton().Use<EntityIndexer>();
			For<AutomaticOnDemandProfileCreator>().Singleton().Use<AutomaticOnDemandProfileCreator>();
			Forward<AutomaticOnDemandProfileCreator, ITargetProcessMessageWhenNoProfilesHandler>();
			Forward<AutomaticOnDemandProfileCreator, ITargetProcessConditionalMessageRouter>();
			For<QueryParser>().Singleton().Use<QueryParser>();
			For<QueryPlanBuilder>().Use<QueryPlanBuilder>();
			For<IContextQueryPlanBuilder>().Use<ImpedimentContextQueryPlanBuilder>();
			For<ContextQueryPlanBuilder>().Use(c =>
			{
				var documentIndexProvider = c.GetInstance<IDocumentIndexProvider>();
				var indexDataFactory = c.GetInstance<IIndexDataFactory>();
				var pluginContext = c.GetInstance<IPluginContext>();
				var profile = c.GetInstance<IProfileReadonly>();
				var entityTypeProvider = c.GetInstance<IEntityTypeProvider>();
				var planBuilders = c.GetAllInstances<IContextQueryPlanBuilder>();
				return new ContextQueryPlanBuilder(documentIndexProvider, indexDataFactory, pluginContext, profile, entityTypeProvider, planBuilders);
			});
			Forward<ContextQueryPlanBuilder, IProjectContextQueryPlanBuilder>();
			For<QueryEngine>().Use<QueryEngine>();
			For<QueryRunner>().Use<QueryRunner>();
			For<TextOperations>().Singleton().Use<TextOperations>();
			For<DocumentIndexRebuilder>().Use<DocumentIndexRebuilder>();
			For<IPatchCollection>().Use<SearchPatchCollection>();
			For<SagaServices>().Use<SagaServices>();
		}
	}
}