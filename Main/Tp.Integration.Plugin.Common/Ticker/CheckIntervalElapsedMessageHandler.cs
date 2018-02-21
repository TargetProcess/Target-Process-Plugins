// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Threading;
using NServiceBus;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.Integration.Plugin.Common.Ticker
{
    public class CheckIntervalElapsedMessageHandler : IHandleMessages<CheckIntervalElapsedMessage>
    {
        private readonly IBus _bus;
        private readonly IPluginMetadata _pluginMetadata;
        private readonly IAccountCollection _collection;
        private readonly IMsmqTransport _transport;
        private readonly ITaskFactory _taskFactory;
        private static int _ticket;

        public CheckIntervalElapsedMessageHandler(IBus bus, IPluginMetadata pluginMetadata, IAccountCollection collection,
            IMsmqTransport transport, ITaskFactory taskFactory)
        {
            _bus = bus;
            _pluginMetadata = pluginMetadata;
            _collection = collection;
            _transport = transport;
            _taskFactory = taskFactory;
        }

        public void Handle(CheckIntervalElapsedMessage message)
        {
            if (Interlocked.CompareExchange(ref _ticket, 1, 0) == 0)
            {
                _taskFactory.StartNew(() =>
                {
                    try
                    {
                        if (!_pluginMetadata.IsSyncronizableProfile)
                        {
                            return;
                        }
                        foreach (var account in _collection)
                        {
                            try
                            {
                                if (account.Profiles.Empty()) continue;

                                var queueName = _transport.GetQueueName(account.Name.Value);
                                var queueIsNotOverloaded = MsmqHelper.QueueIsNotOverloaded(queueName,
                                    "Failed to count messages in queue for account '{0}'"
                                        .Fmt(account.Name.Value), 10);
                                if (!queueIsNotOverloaded)
                                {
                                    continue;
                                }
                                foreach (var profile in account.Profiles)
                                {
                                    var lastSyncDate = profile.Get<LastSyncDate>().FirstOrDefault();
                                    if (IsTimeToSyncronize(profile, lastSyncDate) && profile.Initialized)
                                    {
                                        _bus.SetOut(profile.ConvertToPluginProfile().Name);
                                        _bus.SetOut(account.Name);
                                        _bus.SendLocal(lastSyncDate != null
                                            ? new TickMessage(lastSyncDate.Value)
                                            : new TickMessage(null));
                                        ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType()).Info("TickMesage sent");
                                        profile.Get<LastSyncDate>().ReplaceWith(new LastSyncDate(CurrentDate.Value));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogManager.GetLogger(GetType())
                                    .Error(string.Format("Failed to send tick message for account '{0}'", account.Name.Value), e);
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _ticket);
                    }
                });
            }
        }

        private bool IsTimeToSyncronize(IProfileReadonly profile, LastSyncDate lastSyncDate)
        {
            var syncronizableProfile = profile.Settings as ISynchronizableProfile;
            return IsFirstSynchronization(lastSyncDate)
                || CurrentDate.Value.Subtract(lastSyncDate.Value).TotalMinutes >= syncronizableProfile.SynchronizationInterval;
        }

        private static bool IsFirstSynchronization(LastSyncDate lastSyncDate)
        {
            return lastSyncDate == null;
        }
    }
}
