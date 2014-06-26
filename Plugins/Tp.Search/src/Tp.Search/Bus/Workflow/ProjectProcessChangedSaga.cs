// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Messages;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class ProjectProcessChangedSaga : TpSaga<ProjectProcessChangedSagaData>,
	                                         IAmStartedByMessages<ProjectProcessChangedLocalMessage>,
	                                         IHandleMessages<TargetProcessExceptionThrownMessage>,
	                                         IHandleMessages<AssignableQueryResult>
	{
		private readonly IEntityIndexer _entityIndexer;
		private readonly IEntityTypeProvider _entityTypesProvider;
		private readonly IActivityLogger _logger;
		private readonly AssignablesIndexing _assignablesIndexing;

		public ProjectProcessChangedSaga()
		{
		}

		public ProjectProcessChangedSaga(IEntityIndexer entityIndexer, IEntityTypeProvider entityTypesProvider, IActivityLogger logger)
		{
			_entityIndexer = entityIndexer;
			_entityTypesProvider = entityTypesProvider;
			_logger = logger;
			_assignablesIndexing = new AssignablesIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => MarkAsComplete(), q
			                                                                                                                        =>
				{
					q.ProjectId = Data.ProjectId;
					Send(q);
				}, _logger);
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<ProjectProcessChangedLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AssignableQueryResult>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(ProjectProcessChangedLocalMessage message)
		{
			if (message.ProjectId > 0)
			{
				Data.ProjectId = message.ProjectId;
				_assignablesIndexing.Start();
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(AssignableQueryResult message)
		{
			_assignablesIndexing.Handle(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Build indexes failed", new Exception(message.ExceptionString));
			MarkAsComplete();
		}
	}

	public class ProjectProcessChangedSagaData : ISagaEntity, IAssignableIndexingSagaData
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int ProjectId { get; set; }
		public int SkipGenerals { get; set; }
		public int AssignablesRetrievedCount { get; set; }
		public int AssignablesCurrentDataWindowSize { get; set; }
	}
}