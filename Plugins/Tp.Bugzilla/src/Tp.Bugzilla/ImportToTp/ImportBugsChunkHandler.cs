// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using Tp.BugTracking.ImportToTp;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToTp
{
	public class ImportBugsChunkHandler : IHandleMessages<ImportBugsChunk>
	{
		private readonly ILocalBus _bus;
		private readonly IStorageRepository _storageRepository;
		private readonly IActivityLogger _logger;
		private readonly IBugzillaService _bugzillaService;

		public ImportBugsChunkHandler(ILocalBus bus, IStorageRepository storageRepository, IActivityLogger logger, IBugzillaService bugzillaService)
		{
			_bus = bus;
			_storageRepository = storageRepository;
			_logger = logger;
			_bugzillaService = bugzillaService;
		}

		public void Handle(ImportBugsChunk message)
		{
			bugCollection bugs;

			_logger.Info("Retrieving changed bugs");
			if (TryGetChangedBugsChunk(message.ThirdPartyBugsIds, out bugs))
			{
				_logger.InfoFormat("Bugs retrieved. Bugzilla Bug IDs: {0}", string.Join(", ", message.ThirdPartyBugsIds.Select(x => x.ToString()).ToArray()));
				CreateBugsInTargetProcess(bugs);
			}
			else
			{
				FailedChunks.Add(new FailedChunk(message.ThirdPartyBugsIds));
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
				_logger.ErrorFormat("Retrieving changed bugs (with ids: {0}) failed. Bugzilla IDs: {1}",
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
				_bus.SendLocal(new ImportBugToTargetProcessCommand<BugzillaBug> {ThirdPartyBug = new BugzillaBug(bug)});
			}
		}
	}
}