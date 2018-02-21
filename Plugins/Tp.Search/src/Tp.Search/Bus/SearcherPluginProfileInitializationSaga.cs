// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Messages;
using Tp.Search.Model;

namespace Tp.Search.Bus
{
    class SearcherPluginProfileInitializationSaga
        :
            NewProfileInitializationSaga<SearcherPluginProfileInitializationSagaData>,
            IHandleMessages<IndexExistingEntitiesDoneLocalMessage>
    {
        private readonly IProfileReadonly _profileReadonly;
        private readonly IActivityLogger _log;

        public SearcherPluginProfileInitializationSaga()
        {
        }

        public SearcherPluginProfileInitializationSaga(IProfileReadonly profileReadonly, IActivityLogger log)
        {
            _profileReadonly = profileReadonly;
            _log = log;
        }

        protected override void OnStartInitialization()
        {
            _log.Info("Started building indexes");
            var storage = _profileReadonly.Get<IndexProgress>();
            storage.Clear();
            storage.Add(new IndexProgress());
            SendLocal(new IndexExistingEntitiesLocalMessage { OuterSagaId = Data.Id });
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<IndexExistingEntitiesDoneLocalMessage>(saga => saga.Id, message => message.SagaId);
        }

        public void Handle(IndexExistingEntitiesDoneLocalMessage message)
        {
            _log.Info("Finished rebuilding indexes");
            _profileReadonly.Get<IndexProgress>().Clear();
            MarkAsComplete();
        }
    }

    public class SearcherPluginProfileInitializationSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
    }
}
