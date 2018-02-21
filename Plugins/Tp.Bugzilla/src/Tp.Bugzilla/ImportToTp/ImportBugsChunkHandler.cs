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

            _logger.Info("Retrieving changed bugs");
            if (TryGetChangedBugsChunk(message.ThirdPartyBugsIds, out bugs))
            {
                _logger.InfoFormat("Bugs retrieved. Bugzilla Bug IDs: {0}",
                    string.Join(", ", message.ThirdPartyBugsIds.Select(x => x.ToString()).ToArray()));
                CreateBugsInTargetProcess(bugs);
            }
            else
            {
                FailedChunks.Add(new FailedChunk(message.ThirdPartyBugsIds));
            }
        }

        private IStorage<FailedChunk> FailedChunks => _storageRepository.Get<FailedChunk>();

        private bool TryGetChangedBugsChunk(int[] ids, out bugCollection bugs)
        {
            try
            {
                bugs = _bugzillaService.GetBugs(ids);
                return true;
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Retrieving changed bugs (with ids: {0}) failed. {2}: {1}",
                    string.Join(",", ids.Select(i => i.ToString()).ToArray()), e.Message, e.GetType());

                int currentid = 0;

                try
                {
                    bugs = new bugCollection();

                    foreach (var id in ids)
                    {
                        _logger.InfoFormat("Retrieving changed bug with id: '{0}'", id);
                        currentid = id;
                        var bug = _bugzillaService.GetBugs(new[] { id });
                        bugs.Add(bug[0]);
                    }
                    return true;
                }
                catch (System.Net.WebException webException)
                {
                    _logger.ErrorFormat("Status: {4} Retrieving changed bug with id: '{0}' failed. {2}: {1}, StackTrace: {3}",
                        currentid, webException.Message, webException.GetType(), webException.StackTrace, webException.Status);
                }
                catch (Exception e2)
                {
                    _logger.ErrorFormat("Retrieving changed bug with id: '{0}' failed. {2}: {1}",
                        currentid, e2.Message, e2.GetType());
                }

                bugs = new bugCollection();
                return false;
            }
        }

        private void CreateBugsInTargetProcess(bugCollection bugCollection)
        {
            var bugs = bugCollection.Cast<bug>().ToList();

            foreach (var bug in bugs)
            {
                _bus.SendLocal(new ImportBugToTargetProcessCommand<BugzillaBug> { ThirdPartyBug = new BugzillaBug(bug) });
            }
        }
    }
}
