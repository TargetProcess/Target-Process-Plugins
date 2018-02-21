// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Integration.Plugin.Common.Handlers
{
    public class SyncNowMessageHandler : IHandleMessages<SyncNowMessage>
    {
        private readonly ILocalBus _localBus;
        private readonly IStorageRepository _storage;

        public SyncNowMessageHandler(ILocalBus localBus, IStorageRepository storage)
        {
            _localBus = localBus;
            _storage = storage;
        }

        public void Handle(SyncNowMessage message)
        {
            var logger = ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType());
            logger.Info("SyncNowMessage received");

            var lastSyncDates = _storage.Get<LastSyncDate>();
            var lastSyncDate = lastSyncDates.FirstOrDefault();

            _localBus.SendLocal(lastSyncDate != null
                ? new TickMessage(lastSyncDate.Value)
                : new TickMessage(null));

            logger.Info("TickMessage sent");

            lastSyncDates.ReplaceWith(new LastSyncDate(CurrentDate.Value));
        }
    }
}
