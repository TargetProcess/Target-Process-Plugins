// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus
{
    public class RunAtStartStopInitializer : IWantToRunAtStartup
    {
        private readonly IDocumentIndexProvider _documentIndexProvider;
        private readonly IActivityLogger _log;
        private readonly DocumentIndexRebuilder _documentIndexRebuilder;
        private readonly IPluginContext _pluginContext;
        private readonly bool _isOnSite;

        public RunAtStartStopInitializer()
        {
            _documentIndexProvider = ObjectFactory.GetInstance<IDocumentIndexProvider>();
            _log = ObjectFactory.GetInstance<IActivityLogger>();
            _isOnSite = ObjectFactory.GetInstance<IMsmqTransport>().RoutableTransportMode == RoutableTransportMode.OnSite;
            _documentIndexRebuilder = ObjectFactory.GetInstance<DocumentIndexRebuilder>();
            _pluginContext = ObjectFactory.GetInstance<IPluginContext>();
        }

        public void Run()
        {
            if (_isOnSite)
            {
                _documentIndexRebuilder.RebuildIfNeeded(shouldRebuildIfNoProfile: true);
            }
            _log.Info("Started Search Plugin");
        }

        public void Stop()
        {
            _log.Debug("Save and free Hoot storages memory at stopping Search Plugin");
            var accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
            if (accountCollection != null && accountCollection.Any())
            {
                accountCollection.Where(x => x.Profiles.Any())
                    .AsParallel()
                    .ForEach(a =>
                    {
                        _log.DebugFormat("Shutting down indexes for {0} during stopping service", a.Name);
                        _documentIndexProvider.ShutdownDocumentIndexesIfRunning(
                            new PluginContextSnapshot(a.Name, a.Profiles.First().Name, _pluginContext.PluginName),
                            new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: false), _log);
                        _log.DebugFormat("Indexes shutted down for {0} during stopping service", a.Name);
                    });
            }
            _log.Debug("Finished to save and free Hoot storages memory at stopping Search Plugin");
        }
    }
}
