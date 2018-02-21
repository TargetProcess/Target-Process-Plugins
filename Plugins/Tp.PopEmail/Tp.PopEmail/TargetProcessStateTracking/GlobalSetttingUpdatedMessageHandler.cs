// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.PopEmailIntegration.TargetProcessStateTracking
{
    public class GlobalSetttingUpdatedMessageHandler : IHandleMessages<GlobalSettingUpdatedMessage>
    {
        private readonly IStorageRepository _storageRepo;

        public GlobalSetttingUpdatedMessageHandler(IStorageRepository storageRepo)
        {
            _storageRepo = storageRepo;
        }

        public void Handle(GlobalSettingUpdatedMessage message)
        {
            var storage = _storageRepo.Get<GlobalSettingDTO>();
            storage.Clear();
            storage.Add(message.Dto);
        }
    }
}
