// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model;
using Tp.Search.Model.Exceptions;
using Tp.Search.Model.Document;
using Tp.Search.Model.Query;

namespace Tp.Search.Bus.Commands
{
	class SearchCommand : IPluginCommand
	{
		private readonly QueryRunner _queryRunner;
		private readonly IProfile _profile;
		private readonly DocumentIndexRebuilder _documentIndexRebuilder;
		private readonly IActivityLogger _logger;

		public SearchCommand(QueryRunner queryRunner, IProfile profile, DocumentIndexRebuilder documentIndexRebuilder, IActivityLogger logger)
		{
			_queryRunner = queryRunner;
			_profile = profile;
			_documentIndexRebuilder = documentIndexRebuilder;
			_logger = logger;
		}

		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			if (_profile.IsNull)
			{
				return new PluginCommandResponseMessage
				{
					ResponseData = "Indexing was not started yet.",
					PluginCommandStatus = PluginCommandStatus.Fail
				};
			}
			try
			{
				if(_documentIndexRebuilder.RebuildIfNeeded())
				{
					return new PluginCommandResponseMessage
					{
						ResponseData = "Index rebuild is in progress.",
						PluginCommandStatus = PluginCommandStatus.Fail
					};
				}
				var searchData = args.Deserialize<QueryData>();
				QueryResult queryResult = _queryRunner.Run(searchData);
				QueryResultData queryResultData = CreateQueryResultData(queryResult);
				return new PluginCommandResponseMessage
				{
					ResponseData = queryResultData.Serialize(),
					PluginCommandStatus = PluginCommandStatus.Succeed
				};
			}
			catch (DocumentIndexConcurrentAccessException e)
			{
				_logger.Error(e);
				return new PluginCommandResponseMessage
				{
					ResponseData = "Search failed because of concurrent access exception. Try again, please",
					PluginCommandStatus = PluginCommandStatus.Error
				};
			}
		}

		private QueryResultData CreateQueryResultData(QueryResult result)
		{
			return new QueryResultData
				{
					GeneralIds = result.GeneralIds.ToArray(),
					AssignableIds = result.AssignableIds.ToArray(),
					TestStepIds = result.TestStepIds.ToArray(),
					ImpedimentIds = result.ImpedimentIds.ToArray(),
					CommentIds = result.CommentIds,
					QueryString = result.QueryString,
					Total = result.Total,
					IndexProgressData = CreateIndexingProgress(result)
				};
		}

		private IndexProgressData CreateIndexingProgress(QueryResult result)
		{
			if (_profile.Initialized)
			{
				return new IndexProgressData
				{
					CompleteInPercents = 100
				};
			}
			IStorage<IndexProgress> indexProgressStorage = _profile.Get<IndexProgress>();
			IndexProgress indexProgress = indexProgressStorage.FirstOrDefault();
			if (indexProgress == null)
			{
				return new IndexProgressData
					{
						CompleteInPercents = 0
					};
			}
			int allEntities = indexProgress.LastGeneralId + indexProgress.LastCommentId + indexProgress.LastTestStepId;
			double completeInPercents = allEntities != 0 ? (double)((result.LastIndexedEntityId + result.LastIndexedCommentId + result.LastIndexedTestStepId) * 100) / allEntities : 0;
			return new IndexProgressData { CompleteInPercents = Math.Round(completeInPercents, 2) };
		}

		public string Name
		{
			get { return "Search"; }
		}
	}
}
