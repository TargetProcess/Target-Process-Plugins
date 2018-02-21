// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using Tp.BugTracking.ImportToTp;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToBugzilla
{
    public class BugDeletedHandler : IHandleMessages<BugDeletedMessage>
    {
        private readonly IStorageRepository _storage;
        private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;
        private readonly IActivityLogger _log;

        public BugDeletedHandler(IStorageRepository storage, IBugzillaInfoStorageRepository bugzillaInfoStorageRepository,
            IActivityLogger logger)
        {
            _storage = storage;
            _bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
            _log = logger;
        }

        public void Handle(BugDeletedMessage message)
        {
            try
            {
                var bugzillaBug = _bugzillaInfoStorageRepository.RemoveBugzillaBug(message.Dto.BugID);
                if (bugzillaBug == null)
                {
                    return;
                }

                _storage.Get<TeamDTO>().Remove(t => t.AssignableID == message.Dto.BugID);
                _storage.Get<TargetProcessBugId>(bugzillaBug.Id).ReplaceWith(new TargetProcessBugId
                {
                    Value = message.Dto.BugID.GetValueOrDefault(),
                    Deleted = true
                });

                _log.Info(
                    $"Removed BugzillaBugInfo after deleting TargetProcess Bug ID: {message.Dto.ID}; Bugzilla Bug ID: {bugzillaBug.Id}");
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }
    }
}
