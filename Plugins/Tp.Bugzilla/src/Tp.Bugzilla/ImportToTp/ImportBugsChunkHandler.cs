// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToTp
{
	public class ImportBugsChunkHandler : IHandleMessages<ImportBugsChunk>
	{
		private readonly ILocalBus _bus;
		private readonly IStorageRepository _storageRepository;
		private readonly IActivityLogger _logger;
		private readonly IBugzillaService _bugzillaService;

		public ImportBugsChunkHandler(ILocalBus bus, IStorageRepository storageRepository, IActivityLogger logger,
		                              IBugzillaService bugzillaService)
		{
			_bus = bus;
			_storageRepository = storageRepository;
			_logger = logger;
			_bugzillaService = bugzillaService;
		}

		public void Handle(ImportBugsChunk message)
		{
			bugCollection bugs;

			if (TryGetChangedBugsChunk(message.BugzillaBugsIds, out bugs))
			{
				CreateBugsInTargetProcess(bugs);
			}
			else
			{
				FailedChunks.Add(new FailedChunk(message.BugzillaBugsIds));
			}
		}

		private IStorage<FailedChunk> FailedChunks
		{
			get { return _storageRepository.Get<FailedChunk>(); }
		}

		private bool TryGetChangedBugsChunk(int[] ids, out bugCollection bugs)
		{
			try
			{
				bugs = _bugzillaService.GetBugs(ids);
				return true;
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("Retrieving changed bugs (with ids: {0}) from Bugzilla failed for the reason : '{1}'",
				                    string.Join(",", ids.Select(i => i.ToString()).ToArray()), e.Message);
				bugs = new bugCollection();
				return false;
			}
		}

		private void CreateBugsInTargetProcess(bugCollection bugCollection)
		{
			var bugs = bugCollection.Cast<bug>().ToList();

			foreach (var bug in bugs)
			{
				_bus.SendLocal(new ImportBugToTargetProcessCommand {BugzillaBug = new BugzillaBug(bug)});
			}
		}
	}
}