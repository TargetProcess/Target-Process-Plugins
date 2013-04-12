// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Messages;
using Tp.Search.Model;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus
{
	class SearcherPluginProfileInitializationSaga :
		NewProfileInitializationSaga<SearcherPluginProfileInitializationSagaData>,
		IHandleMessages<IndexExistingEntitiesDoneLocalMessage>,
		IHandleMessages<GeneralQueryResult>,
		IHandleMessages<CommentQueryResult>
	{
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IProfileReadonly _profileReadonly;
		private readonly IActivityLogger _log;

		public SearcherPluginProfileInitializationSaga()
		{
		}

		public SearcherPluginProfileInitializationSaga(IEntityTypeProvider entityTypeProvider, IProfileReadonly profileReadonly, IActivityLogger log)
		{
			_entityTypeProvider = entityTypeProvider;
			_profileReadonly = profileReadonly;
			_log = log;
		}

		protected override void OnStartInitialization()
		{
			_log.Info("Started building indexes");
			Send(new GeneralQuery
				{
					Hql =
						string.Format(
							"select g from General g left join g.ParentProject p where g.EntityType IN ({0}) and (p is null or p.DeleteDate is null) order by g desc skip 0 take 1",
							_entityTypeProvider.QueryableEntityTypesIdSqlString),
					IgnoreMessageSizeOverrunFailure = true,
					Params = new object[] { }
				});
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
			_profileReadonly.Get<IndexProgress>(typeof(IndexProgress).Name).Clear();
			MarkAsComplete();
		}

		public void Handle(GeneralQueryResult message)
		{
			if (message.Dtos.Any())
			{
				IStorage<IndexProgress> storage = _profileReadonly.Get<IndexProgress>(typeof(IndexProgress).Name);
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
						Params = new object[] {}
					});
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(CommentQueryResult message)
		{
			IStorage<IndexProgress> storage = _profileReadonly.Get<IndexProgress>(typeof(IndexProgress).Name);
			IndexProgress indexProgress = storage.FirstOrDefault();
			if (indexProgress != null)
			{
				indexProgress.LastCommentId = message.Dtos.Any() ? message.Dtos[0].ID.GetValueOrDefault() : 0;
				storage.Update(indexProgress, _ => true);
			}
			SendLocal(new IndexExistingEntitiesLocalMessage { OuterSagaId = Data.Id });
		}
	}

	public class SearcherPluginProfileInitializationSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}