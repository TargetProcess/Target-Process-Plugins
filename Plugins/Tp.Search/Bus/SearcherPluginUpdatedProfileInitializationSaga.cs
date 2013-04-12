// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Messages;
using Tp.Search.Model;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus
{
	class SearcherPluginUpdatedProfileInitializationSaga :
		UpdatedProfileInitializationSaga<SearcherPluginProfileUpdatedInitializationData>,
		IHandleMessages<IndexExistingEntitiesDoneLocalMessage>,
		IHandleMessages<GeneralQueryResult>,
		IHandleMessages<CommentQueryResult>
	{
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IPluginContext _pluginContext;
		private readonly ISagaPersister _sagaPersister;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IActivityLogger _log;

		public SearcherPluginUpdatedProfileInitializationSaga()
		{
		}

		public SearcherPluginUpdatedProfileInitializationSaga(IEntityTypeProvider entityTypeProvider, IPluginContext pluginContext, ISagaPersister sagaPersister, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider, IActivityLogger log)
		{
			_entityTypeProvider = entityTypeProvider;
			_pluginContext = pluginContext;
			_sagaPersister = sagaPersister;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_log = log;
		}

		protected override void OnStartInitialization()
		{
			try
			{
				foreach (var tpSagaEntity in ObjectFactory.GetInstance<IStorageRepository>().Get<ISagaEntity>())
				{
					if (!(tpSagaEntity is SearcherPluginProfileInitializationSagaData ||
					    tpSagaEntity is SearcherPluginProfileUpdatedInitializationData)) continue;

					if (tpSagaEntity.Id != Data.Id)
						_sagaPersister.Complete(tpSagaEntity);
				}
			}
			catch (Exception e)
			{
				_log.ErrorFormat("Failed to complete Running Saga When start to rebuild indexed, error: ", e.Message);
			}

			Send(new GeneralQuery
			{
				Hql =
					string.Format(
						"select g from General g left join g.ParentProject p where g.EntityType IN ({0}) and (p is null or p.DeleteDate is null) order by g desc skip 0 take 1",
						_entityTypeProvider.QueryableEntityTypesIdSqlString),
				IgnoreMessageSizeOverrunFailure = true,
				Params = new object[] { }
			});
			_log.Info("Started rebuilding indexes");
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<GeneralQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<IndexExistingEntitiesDoneLocalMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(IndexExistingEntitiesDoneLocalMessage message)
		{
			_log.Info("Finished rebuilding indexes");
			_profile.Get<IndexProgress>(typeof(IndexProgress).Name).Clear();
			MarkAsComplete();
		}

		public void Handle(GeneralQueryResult message)
		{
			if (message.Dtos.Any())
			{
				IStorage<IndexProgress> storage = _profile.Get<IndexProgress>(typeof(IndexProgress).Name);
				storage.Clear();
				storage.Add(new IndexProgress
				{
					LastGeneralId = message.Dtos[0].ID.GetValueOrDefault(),
				});
				Send(new CommentQuery
				{
					Hql =
						string.Format(
							"select c from Comment c join c.General g left join g.ParentProject p where g.EntityType IN ({0}) and (p is null or p.DeleteDate is null) order by c desc skip 0 take 1",
							_entityTypeProvider.QueryableEntityTypesIdSqlString),
					IgnoreMessageSizeOverrunFailure = true,
					Params = new object[] { }
				});
			}
			else
			{
				_documentIndexProvider.ShutdownDocumentIndexes(_pluginContext.AccountName, new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true));
				_log.Info("Finished rebuilding indexes");
				MarkAsComplete();
			}
		}

		public void Handle(CommentQueryResult message)
		{
			IStorage<IndexProgress> storage = _profile.Get<IndexProgress>(typeof(IndexProgress).Name);
			IndexProgress indexProgress = storage.FirstOrDefault();
			if (indexProgress != null)
			{
				indexProgress.LastCommentId = message.Dtos.Any() ? message.Dtos[0].ID.GetValueOrDefault() : 0;
				storage.Update(indexProgress, _ => true);
			}
			SendLocal(new IndexExistingEntitiesLocalMessage { OuterSagaId = Data.Id });
		}
	}

	public class SearcherPluginProfileUpdatedInitializationData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}