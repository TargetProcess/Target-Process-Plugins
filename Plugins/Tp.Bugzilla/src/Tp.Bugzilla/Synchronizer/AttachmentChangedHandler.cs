// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Synchronizer
{
    public class AttachmentChangedHandler
        : EntityChangedHandler<AttachmentDTO>,
          IHandleMessages<AttachmentCreatedMessage>
    {
        private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;

        public AttachmentChangedHandler(IStorageRepository storage,
            IBugzillaInfoStorageRepository bugzillaInfoStorageRepository) : base(storage)
        {
            _bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
        }

        public void Handle(AttachmentCreatedMessage message)
        {
            Create(message.Dto);
        }

        protected override bool NeedToProcess(AttachmentDTO dto)
        {
            return _bugzillaInfoStorageRepository.GetBugzillaBug(dto.GeneralID) != null;
        }
    }
}
