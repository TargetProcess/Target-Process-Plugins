// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model;
using Tp.Search.Model.Query;

namespace Tp.Search.Bus.Commands
{
	class SearchCommand : IPluginCommand
	{
		private readonly QueryRunner _queryRunner;
		private readonly IProfile _profile;

		public SearchCommand(QueryRunner queryRunner, IActivityLogger log, IProfile profile)
		{
			_queryRunner = queryRunner;
			_profile = profile;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			if (_profile.IsNull)
			{
				return new PluginCommandResponseMessage
				{
					ResponseData = "Indexing was not started yet.",
					PluginCommandStatus = PluginCommandStatus.Error
				};
			}
			//var w = Stopwatch.StartNew();
			var searchData = args.Deserialize<QueryData>();
			QueryResult result = _queryRunner.Run(searchData);
			//_log.InfoFormat("PARALLEL Search time elapsed = {0} ms", w.Elapsed.TotalMilliseconds);
			//w.Stop();
			var queryResultData = CreateQueryResultData(result);
			return new PluginCommandResponseMessage
			{
				ResponseData = queryResultData.Serialize(),
				PluginCommandStatus = PluginCommandStatus.Succeed
			};
		}

		private QueryResultData CreateQueryResultData(QueryResult result)
		{
			return new QueryResultData
				{
					GeneralIds = result.GeneralIds.ToArray(),
					AssignableIds = result.AssignableIds.ToArray(),
					TestCaseIds = result.TestCaseIds.ToArray(),
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
			IStorage<IndexProgress> indexProgressStorage = _profile.Get<IndexProgress>(typeof (IndexProgress).Name);
			IndexProgress indexProgress = indexProgressStorage.FirstOrDefault();
			if (indexProgress == null)
			{
				return new IndexProgressData
					{
						CompleteInPercents = 0
					};
			}
			int allEntities = indexProgress.LastGeneralId + indexProgress.LastCommentId;
			double completeInPercents = allEntities != 0 ? (double)((result.LastIndexedEntityId + result.LastIndexedCommentId)*100)/allEntities : 0;
			return new IndexProgressData { CompleteInPercents = Math.Round(completeInPercents, 2) };
		}

		public string Name
		{
			get { return "Search"; }
		}
	}
}