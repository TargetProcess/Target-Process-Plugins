// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Messages;
using Tp.Search.Model;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	public class ProjectProcessChangedSaga : TpSaga<ProjectProcessChangedSagaData>,
	                                         IAmStartedByMessages<ProjectProcessChangedLocalMessage>,
	                                         IHandleMessages<TargetProcessExceptionThrownMessage>,
	                                         IHandleMessages<AssignableQueryResult>
	{
		private const string AssignablesHql = "from Assignable a where a.Project = ? order by a skip {0} take {1}";
		private const int PageSize = 10;

		private readonly IEntityIndexer _entityIndexer;
		private readonly IActivityLogger _logger;

		public ProjectProcessChangedSaga()
		{
		}

		public ProjectProcessChangedSaga(IEntityIndexer entityIndexer, IActivityLogger logger)
		{
			_entityIndexer = entityIndexer;
			_logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<ProjectProcessChangedLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AssignableQueryResult>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(ProjectProcessChangedLocalMessage message)
		{
			Data.ProjectId = message.ProjectId;
			Data.SkipGenerals = 0;

			if (message.ProjectId > 0)
			{
				Send(new AssignableQuery { Hql = string.Format(AssignablesHql, Data.SkipGenerals, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = new object[] { message.ProjectId } });
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(AssignableQueryResult message)
		{
			if (message.Dtos.Any())
			{
				Data.SkipGenerals += PageSize;
				foreach (var assignable in message.Dtos)
				{
					_entityIndexer.UpdateAssignableIndex(assignable, new Collection<AssignableField> { AssignableField.EntityStateID }, isIndexing:false);
				}
				_entityIndexer.OptimizeAssignableIndex();
				Send(new AssignableQuery { Hql = string.Format(AssignablesHql, Data.SkipGenerals, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = new object[] { Data.ProjectId } });
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Build indexes failed", new Exception(message.ExceptionString));
			MarkAsComplete();
		}
	}

	public class ProjectProcessChangedSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int ProjectId { get; set; }
		public int SkipGenerals { get; set; }
	}
}